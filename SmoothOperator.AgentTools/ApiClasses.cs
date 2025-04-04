using System;
using System.Drawing;
using System.Threading.Tasks;
using SmoothOperator.AgentTools.Models;

namespace SmoothOperator.AgentTools
{
    /// <summary>
    /// API endpoints for screenshot and analysis operations
    /// </summary>
    public class ScreenshotApi
    {
        private readonly SmoothOperatorClient _client;

        internal ScreenshotApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Captures a screenshot of the entire screen as Base64-encoded image
        /// </summary>
        /// <returns>Screenshot response containing ImageBase64 property</returns>
        public Task<ScreenshotResponse> TakeAsync()
        {
            return _client.GetAsync<ScreenshotResponse>("/tools-api/screenshot");
        }

        /// <summary>
        /// Uses AI to find the x/y coordinate of a UI element based on text description. Takes a fresh screenshot each time.
        /// </summary>
        /// <param name="userElementDescription">Text description of the element to find</param>
        /// <param name="mechanism">The AI mechanism to use for finding the element (defaults to ScreenGrasp2).</param>
        /// <returns>Response with X/Y coordinates</returns>
        public Task<ScreenGrasp2Response> FindUiElementAsync(string userElementDescription, MechanismType mechanism = MechanismType.ScreenGrasp2)
        {
            // Server expects TaskDescription and Mechanism
            return _client.PostAsync<ScreenGrasp2Response>("/tools-api/screenshot/find-ui-element", new { taskDescription = userElementDescription, Mechanism = mechanism.GetDescription() });
        }

        /// <summary>
        /// Returns a string representation of the ScreenshotApi class.
        /// </summary>
        /// <returns>The string "ScreenshotApi".</returns>
        public override string ToString() => nameof(ScreenshotApi);
    }

    /// <summary>
    /// API endpoints for system operations
    /// </summary>
    public class SystemApi
    {
        private readonly SmoothOperatorClient _client;

        internal SystemApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets detailed overview of computer state including open applications and windows
        /// </summary>
        /// <returns>OverviewResponse with FocusInfo, Windows array and Chrome details</returns>
        public Task<OverviewResponse> GetOverviewAsync()
        {
            // Server returns OverviewResponse
            return _client.PostAsync<OverviewResponse>("/tools-api/system/overview", new { });
        }

        /// <summary>
        /// Gets detailed UI automation information for a window
        /// </summary>
        /// <param name="windowId">Window ID from GetOverviewAsync</param>
        /// <returns>WindowDetailInfosDTO with element hierarchy and properties</returns>
        public Task<WindowDetailInfosDTO> GetWindowDetailsAsync(string windowId)
        {
            // Server returns WindowDetailInfosDTO
            return _client.PostAsync<WindowDetailInfosDTO>("/tools-api/automation/get-details", new { windowId });
        }

        /// <summary>
        /// Opens Chrome browser (Playwright-managed instance)
        /// </summary>
        /// <param name="url">Optional URL to navigate to immediately</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> OpenChromeAsync(string url = null, string strategy = null)
        {
            // Server returns SimpleResponse and accepts strategy
            return _client.PostAsync<SimpleResponse>("/tools-api/system/open-chrome", new { url, strategy });
        }

        /// <summary>
        /// Launches an application by path or name
        /// </summary>
        /// <param name="appNameOrPath">Full path to executable or application name, alternatively exe name if in path (e.g. notepad, calc). For chrome don't use this, use OpenChromeAsync instead.</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> OpenApplicationAsync(string appNameOrPath)
        {
            // Server expects AppNameOrPath and returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/system/open-application", new { appNameOrPath });
        }

        /// <summary>
        /// Returns a string representation of the SystemApi class.
        /// </summary>
        /// <returns>The string "SystemApi".</returns>
        public override string ToString() => nameof(SystemApi);
    }

    /// <summary>
    /// API endpoints for mouse operations
    /// </summary>
    public class MouseApi
    {
        private readonly SmoothOperatorClient _client;

        internal MouseApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Performs left mouse click at screen coordinates (0,0 is top-left)
        /// </summary>
        /// <param name="x">Horizontal pixel coordinate</param>
        /// <param name="y">Vertical pixel coordinate</param>
        /// <returns>Action response with success status</returns>
        public Task<ActionResponse> ClickAsync(int x, int y)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/click", new { x, y });
        }

        /// <summary>
        /// Perform a double click at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> DoubleClickAsync(int x, int y)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/doubleclick", new { x, y });
        }

        /// <summary>
        /// Perform a right mouse button click at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> RightClickAsync(int x, int y)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/rightclick", new { x, y });
        }

        /// <summary>
        /// Move the mouse cursor to the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> MoveAsync(int x, int y)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/move", new { x, y });
        }

        /// <summary>
        /// Perform a mouse drag operation from start coordinates to end coordinates
        /// </summary>
        /// <param name="startX">Start X coordinate</param>
        /// <param name="startY">Start Y coordinate</param>
        /// <param name="endX">End X coordinate</param>
        /// <param name="endY">End Y coordinate</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> DragAsync(int startX, int startY, int endX, int endY)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/drag", new { startX, startY, endX, endY });
        }

        /// <summary>
        /// Scrolls mouse wheel at specified coordinates
        /// </summary>
        /// <param name="x">Horizontal pixel coordinate</param>
        /// <param name="y">Vertical pixel coordinate</param>
        /// <param name="clicks">Number of scroll clicks (positive for down, negative for up)</param>
        /// <param name="direction">Direction to scroll ("up" or "down"). Overrides clicks sign if provided.</param>
        /// <returns>Action response with success status</returns>
        public Task<ActionResponse> ScrollAsync(int x, int y, int clicks, string direction = null) // Changed parameters to match server
        {
            // Server expects clicks and optional direction
            // Map clicks to server's expectation (positive=down, negative=up) if direction is null
            if (direction == null)
            {
                direction = clicks > 0 ? "down" : "up";
                clicks = Math.Abs(clicks);
            }
            else
            {
                clicks = Math.Abs(clicks); // Ensure clicks is positive when direction is specified
            }
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/scroll", new { x, y, clicks, direction });
        }

        /// <summary>
        /// Uses AI vision to find and click a UI element based on description (consumes 50-100 tokens)
        /// </summary>
        /// <param name="userElementDescription">Natural language description of element (be specific and include unique identifiers)</param>
        /// <param name="mechanism">The AI mechanism to use for finding the element (defaults to ScreenGrasp2).</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use ClickAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> ClickByDescriptionAsync(string userElementDescription, MechanismType mechanism = MechanismType.ScreenGrasp2)
        {
            // Server expects TaskDescription and Mechanism
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/click-by-description", new { taskDescription = userElementDescription, Mechanism = mechanism.GetDescription() });
        }

        /// <summary>
        /// Uses AI vision to find and double-click a UI element based on description (consumes 50-100 tokens)
        /// </summary>
        /// <param name="description">Natural language description of element (be specific and include unique identifiers)</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use DoubleClickAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> DoubleClickByDescriptionAsync(string taskDescription) // Changed parameter name
        {
            // Server expects TaskDescription
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/doubleclick-by-description", new { taskDescription });
        }

        /// <summary>
        /// Uses AI vision to find and double-click a UI element based on description (consumes 50-100 tokens)
        /// </summary>
        /// <param name="userElementDescription">Natural language description of element (be specific and include unique identifiers)</param>
        /// <param name="mechanism">The AI mechanism to use for finding the element (defaults to ScreenGrasp2).</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use DoubleClickAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> DoubleClickByDescriptionAsync(string userElementDescription, MechanismType mechanism = MechanismType.ScreenGrasp2)
        {
            // Server expects TaskDescription and Mechanism
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/doubleclick-by-description", new { taskDescription = userElementDescription, Mechanism = mechanism.GetDescription() });
        }

        /// <summary>
        /// Uses AI vision to drag from source to target elements based on descriptions (consumes 100-200 tokens)
        /// </summary>
        /// <param name="sourceDescription">Natural language description of source element</param>
        /// <param name="targetDescription">Natural language description of target element</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use DragAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> DragByDescriptionAsync(string startElementDescription, string endElementDescription) // Changed parameter names
        {
            // Server expects StartElementDescription, EndElementDescription
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/drag-by-description", new { startElementDescription, endElementDescription });
        }

        /// <summary>
        /// Uses AI vision to find and right-click a UI element based on description (consumes 50-100 tokens)
        /// </summary>
        /// <param name="userElementDescription">Natural language description of element (be specific and include unique identifiers)</param>
        /// <param name="mechanism">The AI mechanism to use for finding the element (defaults to ScreenGrasp2).</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use RightClickAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> RightClickByDescriptionAsync(string userElementDescription, MechanismType mechanism = MechanismType.ScreenGrasp2)
        {
            // Server expects TaskDescription and Mechanism
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/rightclick-by-description", new { taskDescription = userElementDescription, Mechanism = mechanism.GetDescription() });
        }

        /// <summary>
        /// Returns a string representation of the MouseApi class.
        /// </summary>
        /// <returns>The string "MouseApi".</returns>
        public override string ToString() => nameof(MouseApi);
    }

    /// <summary>
    /// API endpoints for keyboard operations
    /// </summary>
    public class KeyboardApi
    {
        private readonly SmoothOperatorClient _client;

        internal KeyboardApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Type text at the current cursor position
        /// </summary>
        /// <param name="text">Text to type</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> TypeAsync(string text)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/keyboard/type", new { text });
        }

        /// <summary>
        /// Presses key or hotkey combination (e.g. "Ctrl+C" or "Alt+F4")
        /// </summary>
        /// <param name="key">Key name or combination</param>
        /// <returns>Action response with success status</returns>
        public Task<ActionResponse> PressAsync(string key)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/keyboard/press", new { key });
        }

        /// <summary>
        /// Find a UI element based on a text description and type text into it
        /// </summary>
        /// <param name="elementDescription">Text description of the UI element</param>
        /// <param name="textToType">Text to type</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> TypeAtElementAsync(string elementDescription, string textToType) // Changed parameter names
        {
            // Server expects ElementDescription, TextToType
            return _client.PostAsync<ActionResponse>("/tools-api/keyboard/type-at-element", new { elementDescription, textToType });
        }

        /// <summary>
        /// Returns a string representation of the KeyboardApi class.
        /// </summary>
        /// <returns>The string "KeyboardApi".</returns>
        public override string ToString() => nameof(KeyboardApi);
    }

    /// <summary>
    /// API endpoints for Chrome browser operations
    /// </summary>
    public class ChromeApi
    {
        private readonly SmoothOperatorClient _client;

        internal ChromeApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Opens Chrome browser (Playwright-managed instance)
        /// </summary>
        /// <param name="url">Optional URL to navigate to immediately</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> OpenChromeAsync(string url = null, string strategy = null) // Changed return type, added strategy
        {
            // Server returns SimpleResponse and accepts strategy
            return _client.PostAsync<SimpleResponse>("/tools-api/system/open-chrome", new { url, strategy });
        }

        /// <summary>
        /// Gets detailed analysis of current Chrome tab including interactive elements
        /// </summary>
        /// <returns>ChromeTabDetails with CSS selectors for key elements</returns>
        /// <remarks>Requires Chrome to be opened via OpenChromeAsync first</remarks>
        public Task<ChromeTabDetails> ExplainCurrentTabAsync()
        {
            return _client.PostAsync<ChromeTabDetails>("/tools-api/chrome/current-tab/explain", new { });
        }

        /// <summary>
        /// Navigate to a URL in the current Chrome tab
        /// </summary>
        /// <param name="url">URL to navigate to</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> NavigateAsync(string url)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/navigate", new { url });
        }

        /// <summary>
        /// Reload the current Chrome tab
        /// </summary>
        /// <returns>Action response</returns>
        public Task<ActionResponse> ReloadAsync()
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/reload", new { });
        }

        /// <summary>
        /// Open a new Chrome tab
        /// </summary>
        /// <param name="url">Optional URL to navigate to</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> NewTabAsync(string url = null)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/new-tab", new { url });
        }

        /// <summary>
        /// Uses AI vision to move mouse cursor to element based on description (consumes 50-100 tokens)
        /// </summary>
        /// <param name="userElementDescription">Natural language description of element (be specific and include unique identifiers)</param>
        /// <param name="mechanism">The AI mechanism to use for finding the element (defaults to ScreenGrasp2).</param>
        /// <returns>Action response with success status and coordinates</returns>
        /// <remarks>
        /// If you know the exact coordinates, use MoveAsync instead for faster operation.
        /// Can optionally include a base64-encoded screenshot in the request body.
        /// </remarks>
        public Task<ActionResponse> MoveByDescriptionAsync(string userElementDescription, MechanismType mechanism = MechanismType.ScreenGrasp2)
        {
            // Server expects TaskDescription and Mechanism
            return _client.PostAsync<ActionResponse>("/tools-api/mouse/move-by-description", new { taskDescription = userElementDescription, Mechanism = mechanism.GetDescription() });
        }

        /// <summary>
        /// Clicks element in Chrome tab using CSS selector
        /// </summary>
        /// <param name="selector">CSS selector from ExplainCurrentTabAsync</param>
        /// <returns>Action response with success status</returns>
        public Task<ActionResponse> ClickElementAsync(string selector)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/click-element", new { selector });
        }

        /// <summary>
        /// Navigate back in the current Chrome tab
        /// </summary>
        /// <returns>Action response</returns>
        public Task<ActionResponse> GoBackAsync()
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/go-back", new { });
        }

        /// <summary>
        /// Simulate input in an element in the current Chrome tab
        /// </summary>
        /// <param name="selector">CSS selector of the element to input text into</param>
        /// <param name="text">Text to input</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> SimulateInputAsync(string selector, string text)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/simulate-input", new { selector, text });
        }

        /// <summary>
        /// Get the DOM of the current Chrome tab
        /// </summary>
        /// <returns>Action response with DOM content</returns>
        public Task<ActionResponse> GetDomAsync()
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/get-dom", new { });
        }

        /// <summary>
        /// Get the text content of the current Chrome tab
        /// </summary>
        /// <returns>Action response with text content</returns>
        public Task<ActionResponse> GetTextAsync()
        {
            return _client.PostAsync<ActionResponse>("/tools-api/chrome/get-text", new { });
        }

        /// <summary>
        /// Executes JavaScript in Chrome tab and returns result
        /// </summary>
        /// <param name="script">JavaScript code to run</param>
        /// <returns>ChromeScriptResponse with execution result</returns>
        public Task<ChromeScriptResponse> ExecuteScriptAsync(string script) // Changed parameter name and return type
        {
            // Server expects Script and returns ChromeScriptResponse
            return _client.PostAsync<ChromeScriptResponse>("/tools-api/chrome/execute-script", new { script });
        }

        /// <summary>
        /// Generate and execute JavaScript based on a description
        /// </summary>
        /// <param name="taskDescription">Description of what the JavaScript should do</param>
        /// <returns>ChromeScriptResponse with execution result</returns>
        public Task<ChromeScriptResponse> GenerateAndExecuteScriptAsync(string taskDescription) // Changed parameter name and return type
        {
            // Server expects TaskDescription and returns ChromeScriptResponse
            return _client.PostAsync<ChromeScriptResponse>("/tools-api/chrome/generate-and-execute-script", new { taskDescription });
        }

        /// <summary>
        /// Returns a string representation of the ChromeApi class.
        /// </summary>
        /// <returns>The string "ChromeApi".</returns>
        public override string ToString() => nameof(ChromeApi);
    }

    /// <summary>
    /// API endpoints for Windows automation operations
    /// </summary>
    public class AutomationApi
    {
        private readonly SmoothOperatorClient _client;

        internal AutomationApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Launches an application by path or name
        /// </summary>
        /// <param name="appNameOrPath">Full path to executable or application name, alternatively exe name if in path (e.g. notepad, calc)</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> OpenApplicationAsync(string appNameOrPath) // Changed parameter name and return type
        {
            // Server expects AppNameOrPath and returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/system/open-application", new { appNameOrPath });
        }

        /// <summary>
        /// Invokes default action on Windows UI element (e.g. click button) by Element ID (get from: Element ID from GetOverviewAsync/GetDetailsAsync/GetWindowDetailsAsync)
        /// </summary>
        /// <param name="elementId">Element ID from GetOverviewAsync/GetDetailsAsync/GetWindowDetailsAsync</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> InvokeAsync(string elementId) // Removed action parameter, changed return type
        {
            // Server ignores client 'action' parameter, hardcodes "invoke", and returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/automation/invoke", new { elementId });
        }

        /// <summary>
        /// Set the value of a UI element by Element ID (get from: Element ID from GetOverviewAsync/GetDetailsAsync/GetWindowDetailsAsync)
        /// </summary>
        /// <param name="elementId">ID of the UI element (get from: Element ID from GetOverviewAsync/GetDetailsAsync/GetWindowDetailsAsync)</param>
        /// <param name="value">Value to set</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> SetValueAsync(string elementId, string value) // Changed return type
        {
            // Server returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/automation/set-value", new { elementId, value });
        }

        /// <summary>
        /// Set focus to a UI element
        /// </summary>
        /// <param name="elementId">ID of the UI element</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> SetFocusAsync(string elementId) // Changed return type
        {
            // Server returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/automation/set-focus", new { elementId });
        }

        /// <summary>
        /// Gets detailed UI automation information for a window
        /// </summary>
        /// <param name="windowId">Window ID from GetOverviewAsync</param>
        /// <returns>WindowDetailInfosDTO with element hierarchy and properties</returns>
        public Task<WindowDetailInfosDTO> GetWindowDetailsAsync(string windowId) // Changed return type from object
        {
            // Server returns WindowDetailInfosDTO
            return _client.PostAsync<WindowDetailInfosDTO>("/tools-api/automation/get-details", new { windowId });
        }

        /// <summary>
        /// Bring a window to the front
        /// </summary>
        /// <param name="windowId">ID of the window</param>
        /// <returns>SimpleResponse indicating success or failure</returns>
        public Task<SimpleResponse> BringToFrontAsync(string windowId) // Changed return type
        {
            // Server returns SimpleResponse
            return _client.PostAsync<SimpleResponse>("/tools-api/automation/bring-to-front", new { windowId });
        }

        /// <summary>
        /// Find and click a Windows UI element by description
        /// </summary>
        /// <param name="description">Description of the UI element</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> ClickElementAsync(string description)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/automation/click-element", new { description });
        }

        /// <summary>
        /// Find a Windows UI element by description and type text into it
        /// </summary>
        /// <param name="description">Description of the UI element</param>
        /// <param name="text">Text to type</param>
        /// <returns>Action response</returns>
        public Task<ActionResponse> TypeInElementAsync(string description, string text)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/automation/type-in-element", new { description, text });
        }

        /// <summary>
        /// Get text from a Windows UI element by description
        /// </summary>
        /// <param name="description">Description of the UI element</param>
        /// <returns>Action response with element text</returns>
        public Task<ActionResponse> GetElementTextAsync(string description)
        {
            return _client.PostAsync<ActionResponse>("/tools-api/automation/get-element-text", new { description });
        }

        /// <summary>
        /// Returns a string representation of the AutomationApi class.
        /// </summary>
        /// <returns>The string "AutomationApi".</returns>
        public override string ToString() => nameof(AutomationApi);
    }

    /// <summary>
    /// API endpoints for code execution operations
    /// </summary>
    public class CodeApi
    {
        private readonly SmoothOperatorClient _client;

        internal CodeApi(SmoothOperatorClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Executes C# code on server and returns output
        /// </summary>
        /// <param name="code">C# code to run</param>
        /// <returns>CSharpCodeResponse with execution result</returns>
        public Task<CSharpCodeResponse> ExecuteCSharpAsync(string code) // Changed return type
        {
            // Server returns CSharpCodeResponse
            return _client.PostAsync<CSharpCodeResponse>("/tools-api/code/csharp", new { code });
        }

        /// <summary>
        /// Generate and execute C# code based on a description
        /// </summary>
        /// <param name="taskDescription">Description of what the C# code should do, include error feedback if a previous try wasn't successful</param>
        /// <returns>CSharpCodeResponse with execution result</returns>
        public Task<CSharpCodeResponse> GenerateAndExecuteCSharpAsync(string taskDescription) // Changed parameter name and return type
        {
            // Server expects TaskDescription and returns CSharpCodeResponse
            return _client.PostAsync<CSharpCodeResponse>("/tools-api/code/csharp/generate-and-execute", new { taskDescription });
        }

        /// <summary>
        /// Returns a string representation of the CodeApi class.
        /// </summary>
        /// <returns>The string "CodeApi".</returns>
        public override string ToString() => nameof(CodeApi);
    }
}
