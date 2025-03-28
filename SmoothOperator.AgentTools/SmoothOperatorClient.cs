using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmoothOperator.AgentTools.Models;
using System.IO;
using System.Reflection;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;

namespace SmoothOperator.AgentTools
{
    /// <summary>
    /// Main client for the Smooth Operator Agent Tools API
    /// </summary>
    public class SmoothOperatorClient : IDisposable
    {
        private const bool LOG_TIMING = true;

        private readonly HttpClient _httpClient;
        private string _baseUrl;
        private bool _disposed = false;
        private Process _serverProcess;
        private static readonly Task<string> installationFolderTask;

        /// <summary>
        /// Screenshot and analysis operations
        /// </summary>
        public ScreenshotApi Screenshot { get; }

        /// <summary>
        /// System operations
        /// </summary>
        public SystemApi System { get; }

        /// <summary>
        /// Mouse operations
        /// </summary>
        public MouseApi Mouse { get; }

        /// <summary>
        /// Keyboard operations
        /// </summary>
        public KeyboardApi Keyboard { get; }

        /// <summary>
        /// Chrome browser operations
        /// </summary>
        public ChromeApi Chrome { get; }

        /// <summary>
        /// Windows automation operations
        /// </summary>
        public AutomationApi Automation { get; }

        /// <summary>
        /// Code execution operations
        /// </summary>
        public CodeApi Code { get; }

        static SmoothOperatorClient()
        {
            installationFolderTask = Task.Run(EnsureInstalledInternal);
        }

        /// <summary>
        /// Creates a new instance of the SmoothOperatorClient
        /// </summary>
        /// <param name="apiKey">Optional: API key for authentication. Most methods don't require an API Key, but for some, especially the ones that use AI, you need to provide a Screengrasp.com API Key</param>
        /// <param name="baseUrl">Optional: Base URL of the API. By Default the url is automatically determined by calling StartServer(), alternatively you can also just point to an already running Server instance by providing its base url here.</param>
        public SmoothOperatorClient(string apiKey = null, string baseUrl = null)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey ?? "no_api_key_specified");

            // Initialize API categories
            Screenshot = new ScreenshotApi(this);
            System = new SystemApi(this);
            Mouse = new MouseApi(this);
            Keyboard = new KeyboardApi(this);
            Chrome = new ChromeApi(this);
            Automation = new AutomationApi(this);
            Code = new CodeApi(this);
        }

        /// <summary>
        /// Starts the Smooth Operator Agent Tools Server
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when server is already running or base URL is already set manually</exception>
        /// <exception cref="IOException">Thrown when server files cannot be extracted or accessed</exception>
        public async Task StartServerAsync()
        {
            if (_baseUrl != null)
            {
                throw new InvalidOperationException("Cannot start server when base URL has been already set.");
            }
            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Starting server...");

            installationFolderTask.Wait(); //ensure installation (in background) has completed
            var installationFolder = installationFolderTask.Result;

            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Installation is completed.");

            // Generate random port number filename
            Random random = new Random();
            string portNumberFileName = $"portnr_{random.Next(1000000, 100000000)}.txt";
            string portNumberFilePath = Path.Combine(installationFolder, portNumberFileName);

            // Delete the port number file if it exists from a previous run
            if (File.Exists(portNumberFilePath))
            {
                File.Delete(portNumberFilePath);
            }

            // Start the server process
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(installationFolder, "smooth-operator-server.exe"),
                Arguments = $"/silent /close-with-parent-process /managed-by-lib /apikey=no_api_key_provided /portnrfile={portNumberFileName}",
                WorkingDirectory = installationFolder,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            _serverProcess = Process.Start(startInfo);
            if (_serverProcess == null)
            {
                throw new InvalidOperationException("Failed to start the server process.");
            }
            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Server process started.");

            // Wait for the port number file to be created
            int maxWaitTimeMs = 30000; // 30 seconds max wait
            int waitedMs = 0;
            while (!File.Exists(portNumberFilePath) && waitedMs < maxWaitTimeMs)
            {
                await Task.Delay(100);
                waitedMs += 100;
            }

            if (!File.Exists(portNumberFilePath))
            {
                StopServer();
                throw new TimeoutException("Server failed to report port number within the timeout period.");
            }

            // Read the port number
            string portNumber = File.ReadAllText(portNumberFilePath).Trim();
            _baseUrl = $"http://localhost:{portNumber}";
            File.Delete(portNumberFilePath);

            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Server reported back it is running at port {portNumber}.");

            //check if server is running
            waitedMs = 0;
            while (true) {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var result = await GetAsync<string>("/tools-api/ping");
                    if (result == "pong")
                    {
                        break; //now the server is ready for requests
                    }
                }
                catch
                {
                    //no problem, just means server is not ready, yet
                }
                stopwatch.Stop();
                waitedMs += (int)stopwatch.ElapsedMilliseconds;
                if (waitedMs > maxWaitTimeMs) throw new TimeoutException("Server failed to become responsive within the timeout period.");
                await Task.Delay(100);
                waitedMs += 100;
                if (waitedMs > maxWaitTimeMs) throw new TimeoutException("Server failed to become responsive within the timeout period.");
            }
            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Server ping successful, server is running.");
        }

        /// <summary>
        /// Makes sure the Smooth Operator Agent Tools Server is installed. 
        /// The installation happens only once, ever. And it happens automatically in the background. 
        /// This method does not need to be called to use the Smooth Operator Agent Tools, it's optional. It is only offered because installation can take a while (up to 30 seconds),
        /// thus you may want to do it in a place of the process (e.g. the installer) where it doesn't annoy the user.
        /// </summary>
        public async Task EnsureInstalledAsync()
        {
            await installationFolderTask;
        }

        private static string EnsureInstalledInternal()
        {
            var sw = Stopwatch.StartNew();
            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Ensuring installation...");

            // Get the installation folder path
            var installationFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SmoothOperator",
                "AgentToolsServer");

            // Create the installation folder if it doesn't exist
            if (!Directory.Exists(installationFolder))
            {
                Directory.CreateDirectory(installationFolder);
            }

            // Check if installedversion.txt exists and read its content
            string installedVersionPath = Path.Combine(installationFolder, "installedversion.txt");
            string installedVersionContent = null;
            bool needsExtraction = true;

            if (File.Exists(installedVersionPath))
            {
                installedVersionContent = File.ReadAllText(installedVersionPath);
                
                // Get the embedded version content
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("SmoothOperator.AgentTools.installedversion.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string embeddedVersionContent = reader.ReadToEnd();
                            
                            // If versions match, no need to extract
                            if (installedVersionContent == embeddedVersionContent)
                            {
                                needsExtraction = false;
                            }
                        }
                    }
                }
            }

            // Extract the server files if needed
            if (needsExtraction)
            {
                Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Starting to extract server files after {sw.ElapsedMilliseconds}ms ...");

                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("SmoothOperator.AgentTools.smooth-operator-server.zip"))
                {
                    if (stream == null)
                    {
                        throw new IOException("Could not find embedded server package.");
                    }

                    // Extract the zip file contents
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            string destinationPath = Path.Combine(installationFolder, entry.FullName);
                            
                            // Create directory if needed
                            string destinationDir = Path.GetDirectoryName(destinationPath);
                            if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
                            {
                                Directory.CreateDirectory(destinationDir);
                            }

                            // Skip directories
                            if (string.IsNullOrEmpty(entry.Name))
                                continue;

                            // Extract the file, overwriting if exists
                            entry.ExtractToFile(destinationPath, true);
                        }
                    }
                }

                // Also extract the installedversion.txt
                using (var stream = assembly.GetManifestResourceStream("SmoothOperator.AgentTools.installedversion.txt"))
                {
                    if (stream != null)
                    {
                        using (var fileStream = new FileStream(installedVersionPath, FileMode.Create))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }                
            }

            sw.Stop();

            Debug.WriteLineIf(LOG_TIMING, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Installation ensured after {sw.ElapsedMilliseconds}ms ...");

            return installationFolder;
        }

        /// <summary>
        /// Stops the Smooth Operator Agent Tools Server if it was started by this client
        /// </summary>
        public void StopServer()
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                try
                {
                    _serverProcess.Kill();
                    _serverProcess.WaitForExit(5000); // Wait up to 5 seconds for the process to exit
                }
                catch (Exception)
                {
                    // Ignore errors when trying to kill the process
                }
                finally
                {
                    _serverProcess.Dispose();
                    _serverProcess = null;
                }
            }
        }

        /// <summary>
        /// Sends a GET request to the specified endpoint
        /// </summary>
        /// <typeparam name="T">Type to deserialize the response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <returns>Deserialized response</returns>
        internal async Task<T> GetAsync<T>(string endpoint)
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not set. You must call StartServer() first, or provide a baseUrl in the constructor.");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Sends a POST request to the specified endpoint
        /// </summary>
        /// <typeparam name="T">Type to deserialize the response to</typeparam>
        /// <param name="endpoint">API endpoint</param>
        /// <param name="data">Request data</param>
        /// <returns>Deserialized response</returns>
        internal async Task<T> PostAsync<T>(string endpoint, object data = null)
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not set. You must call StartServer() first, or provide a baseUrl in the constructor.");
            }

            var content = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                : new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Disposes the HTTP client
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the HTTP client
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopServer();
                    _httpClient?.Dispose();
                }

                _disposed = true;
            }
        }

        ~SmoothOperatorClient()
        {
            Dispose(false);
        }
    }
}
