## Server Installation

The Smooth Operator client library includes a server component that needs to be installed in your application data directory. The server files (`smooth-operator-server.zip` and `installedversion.txt`) are embedded as resources in the library and will be automatically extracted on first use.

### First-Time Execution

When you first use the library, it will automatically:
1. Create the directory `%APPDATA%\SmoothOperator\AgentToolsServer` (or the equivalent on your OS)
2. Extract the server files from the embedded resources
3. Place the `installedversion.txt` file to track the installed version

This process may take a few seconds on first execution. Subsequent uses will be faster as the server files are already installed.

### For Application Installers

If you're building an application installer that includes this library, you may want to pre-install the server files during your application's installation process for better user experience. The server files are embedded as resources in the library:

- `SmoothOperator.AgentTools.Resources.smooth-operator-server.zip`: Contains the server executable and dependencies
- `SmoothOperator.AgentTools.Resources.installedversion.txt`: Contains the version number of the server

You can access these resources using:
```csharp
using System.Reflection;

// Get the assembly containing the resources
var assembly = Assembly.GetExecutingAssembly();

// Get the resource names
var serverZipResource = "SmoothOperator.AgentTools.Resources.smooth-operator-server.zip";
var versionFileResource = "SmoothOperator.AgentTools.Resources.installedversion.txt";

// Extract the resources
using (var stream = assembly.GetManifestResourceStream(serverZipResource))
{
    // Copy to %APPDATA%\SmoothOperator\AgentToolsServer
}

using (var stream = assembly.GetManifestResourceStream(versionFileResource))
{
    // Copy to %APPDATA%\SmoothOperator\AgentToolsServer
}
```

You can then copy these files to `%APPDATA%\SmoothOperator\AgentToolsServer` during your application's installation process. 