# Installation and Usage Guide for the Smooth Operator Agent Tools C# Library

## Installation

### Using .NET CLI

The Smooth Operator Agent Tools C# library can be installed using the .NET CLI:

```bash
dotnet add package SmoothOperator.AgentTools
```

### Using NuGet Package Manager

Alternatively, you can install it using the NuGet Package Manager console in Visual Studio:

```
Install-Package SmoothOperator.AgentTools
```

This will automatically install the library and all its dependencies, including the server executable.

## Basic Usage

### Initializing the Client

```csharp
using SmoothOperator.AgentTools;
using SmoothOperator.AgentTools.Models;
using System;
using System.Threading.Tasks;

public class Example
{
    public static async Task Main(string[] args)
    {
        // Initialize the client with your API key
        // Get API key for free at https://screengrasp.com/api.html
        var client = new SmoothOperatorClient("YOUR_API_KEY");

        try
        {
            // Start the server (ensure it's installed first if needed)
            // await client.EnsureInstalledAsync(); // Uncomment if you haven't run this before
            await client.StartServerAsync();
            Console.WriteLine("Server started.");

            // Use the client here...
            // Example: Get system overview
            var overview = await client.System.GetOverviewAsync();
            Console.WriteLine($"Got overview. Focused window: {overview?.FocusInfo?.FocusedElementParentWindow?.Title}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            // Stop the server when done
            await client.StopServerAsync();
            Console.WriteLine("Server stopped.");
        }
    }
}
```

You can also use the client with `using` for automatic disposal (which calls `StopServerAsync`):

```csharp
using SmoothOperator.AgentTools;
using SmoothOperator.AgentTools.Models;
using System;
using System.Threading.Tasks;

public class ExampleUsing
{
    public static async Task Main(string[] args)
    {
        // Get API key for free at https://screengrasp.com/api.html
        using (var client = new SmoothOperatorClient("YOUR_API_KEY"))
        {
            try
            {
                await client.StartServerAsync();
                Console.WriteLine("Server started.");

                // Use the client here...
                var overview = await client.System.GetOverviewAsync();
                Console.WriteLine($"Got overview. Focused window: {overview?.FocusInfo?.FocusedElementParentWindow?.Title}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            // Server will be automatically stopped when exiting the using block
        }
         Console.WriteLine("Server stopped (implicitly via using).");
    }
}
```

### Taking Screenshots

```csharp
// Take a screenshot - returns image data
var screenshot = await client.Screenshot.TakeAsync();

if (screenshot.Success)
{
    // Access the screenshot data
    byte[] imageBytes = screenshot.ImageBytes;
    string imageBase64 = screenshot.ImageBase64;
    Console.WriteLine($"Screenshot taken successfully at {screenshot.Timestamp}. Size: {imageBytes.Length} bytes.");
}
else
{
    Console.WriteLine($"Failed to take screenshot: {screenshot.Message}");
}
```

### Mouse Operations

```csharp
// Click at coordinates
var clickResponse = await client.Mouse.ClickAsync(500, 300);
Console.WriteLine($"Click success: {clickResponse.Success}");

// Right-click at coordinates
var rightClickResponse = await client.Mouse.RightClickAsync(500, 300);
Console.WriteLine($"Right-click success: {rightClickResponse.Success}");

// Double-click at coordinates
var doubleClickResponse = await client.Mouse.DoubleClickAsync(500, 300);
Console.WriteLine($"Double-click success: {doubleClickResponse.Success}");

// Drag from one position to another
var dragResponse = await client.Mouse.DragAsync(100, 100, 200, 200);
Console.WriteLine($"Drag success: {dragResponse.Success}");

// Scroll at coordinates
var scrollDownResponse = await client.Mouse.ScrollAsync(500, 300, 5); // Scroll down 5 clicks
Console.WriteLine($"Scroll down success: {scrollDownResponse.Success}");

var scrollUpResponse = await client.Mouse.ScrollAsync(500, 300, -5); // Scroll up 5 clicks
Console.WriteLine($"Scroll up success: {scrollUpResponse.Success}");
```

### AI-Powered UI Interaction (Mouse)

```csharp
// Find and click a UI element by description
var clickDescResponse = await client.Mouse.ClickByDescriptionAsync("the Submit button");
Console.WriteLine($"Click by description success: {clickDescResponse.Success} - {clickDescResponse.Message}");

// Find and right-click a UI element by description
var rightClickDescResponse = await client.Mouse.RightClickByDescriptionAsync("the Context menu icon");
Console.WriteLine($"Right-click by description success: {rightClickDescResponse.Success} - {rightClickDescResponse.Message}");

// Find and double-click a UI element by description
var doubleClickDescResponse = await client.Mouse.DoubleClickByDescriptionAsync("the File icon");
Console.WriteLine($"Double-click by description success: {doubleClickDescResponse.Success} - {doubleClickDescResponse.Message}");

// Drag from one element to another by description
var dragDescResponse = await client.Mouse.DragByDescriptionAsync("the invoice pdf file", "the 'invoices' folder");
Console.WriteLine($"Drag by description success: {dragDescResponse.Success} - {dragDescResponse.Message}");
```

### Keyboard Operations

```csharp
// Type text
var typeResponse = await client.Keyboard.TypeAsync("Hello, world!");
Console.WriteLine($"Type success: {typeResponse.Success}");

// Press a key combination
var pressCtrlC = await client.Keyboard.PressAsync("Ctrl+C");
Console.WriteLine($"Press Ctrl+C success: {pressCtrlC.Success}");
var pressAltF4 = await client.Keyboard.PressAsync("Alt+F4");
Console.WriteLine($"Press Alt+F4 success: {pressAltF4.Success}");

// Type text in a UI element (identified by description)
var typeAtElementResponse = await client.Keyboard.TypeAtElementAsync("the Username field", "user123");
Console.WriteLine($"Type at element success: {typeAtElementResponse.Success} - {typeAtElementResponse.Message}");
```

### Chrome Browser Control

```csharp
// Open Chrome browser to a specific URL
var openResponse = await client.Chrome.OpenChromeAsync("https://www.example.com");
Console.WriteLine($"Open Chrome success: {openResponse.Success}");

// Navigate to a different URL
var navigateResponse = await client.Chrome.NavigateAsync("https://www.google.com");
Console.WriteLine($"Navigate success: {navigateResponse.Success}");

// Get information about the current tab
// Can be used to find likely interactable elements in the page
// Marks all html elements with robust CSS selectors for use
// in functions like ClickElementAsync() or SimulateInputAsync()
// Response can also be passed to LLM to pick the right selector
var tabDetails = await client.Chrome.ExplainCurrentTabAsync();
Console.WriteLine($"Explained tab: {tabDetails.Title}. Found {tabDetails.Elements?.Count} elements.");
// Example: Find the selector for the search button (hypothetical)
// var searchButtonSelector = tabDetails.Elements?.FirstOrDefault(el => el.ContainsKey("innerText") && el["innerText"].ToString().Contains("Search"))?["cssSelector"]?.ToString();

// Click an element using CSS selector (replace with actual selector)
if (!string.IsNullOrEmpty(tabDetails?.Elements?[0]?.CssSelector)) // Use a real selector
{
    var clickElementResponse = await client.Chrome.ClickElementAsync(tabDetails.Elements[0].CssSelector);
    Console.WriteLine($"Click element success: {clickElementResponse.Success}");
}

// Input text into a form field (replace with actual selector)
if (!string.IsNullOrEmpty(tabDetails?.Elements?[1]?.CssSelector)) // Use a real selector
{
    var inputResponse = await client.Chrome.SimulateInputAsync(tabDetails.Elements[1].CssSelector, "search query");
    Console.WriteLine($"Simulate input success: {inputResponse.Success}");
}

// Execute JavaScript
var scriptResponse = await client.Chrome.ExecuteScriptAsync("return document.title;");
if (scriptResponse.Success)
{
    Console.WriteLine($"Executed script. Result: {scriptResponse.Result}");
}

// Generate and execute JavaScript based on a description
var genScriptResponse = await client.Chrome.GenerateAndExecuteScriptAsync("Extract all links from the page");
if (genScriptResponse.Success)
{
    Console.WriteLine($"Generated and executed script. Result: {genScriptResponse.Result}");
}
```

### System Operations

```csharp
// Get system overview
// Contains list of windows, available apps on the system,
// detailed infos about the currently focused ui element and window.
// Can be used as a source of ui element ids for use in automation functions
// like InvokeAsync() (=click) or SetValueAsync().
// Can be used as a source of window ids for GetWindowDetailsAsync(windowId).
// Consider sending the JSON serialized form of this result to a LLM, together
// with a task description, the form is chosen to be LLM friendly, the LLM
// should be able to find the relevant ui element ids and windows ids like that.
var overview = await client.System.GetOverviewAsync();
Console.WriteLine($"System overview obtained. Found {overview?.Windows?.Count} windows.");

// Open an application (e.g., Notepad)
var openAppResponse = await client.System.OpenApplicationAsync("notepad");
Console.WriteLine($"Open Notepad success: {openAppResponse.Success}");

// Get window details - contains the ui automation tree of elements.
// Consider using the response in a LLM prompt.
string windowId = overview?.Windows?.FirstOrDefault()?.Id; // Get ID of the first window
if (!string.IsNullOrEmpty(windowId))
{
    var windowDetails = await client.System.GetWindowDetailsAsync(windowId);
    Console.WriteLine($"Got details for window ID {windowId}. Root element: {windowDetails?.UserInterfaceElements?.Name}");
    // You can serialize windowDetails to JSON using windowDetails.ToJsonString()
}
```

### Windows Automation

```csharp
// Need an element ID first, e.g., from GetOverviewAsync or GetWindowDetailsAsync
var overviewForAutomation = await client.System.GetOverviewAsync();
var targetElement = overviewForAutomation?.FocusInfo?.FocusedElement; // Example: Use focused element
string elementId = targetElement?.Id;
string windowIdForAutomation = overviewForAutomation?.FocusInfo?.FocusedElementParentWindow?.Id;


if (!string.IsNullOrEmpty(elementId))
{
    // Click (Invoke) a UI element by its ID
    if (targetElement?.SupportsInvoke ?? false)
    {
        var invokeResponse = await client.Automation.InvokeAsync(elementId);
        Console.WriteLine($"Invoke element success: {invokeResponse.Success}");
    }

    // Type text in a UI element by its ID
    if (targetElement?.SupportsSetValue ?? false)
    {
        var setValueResponse = await client.Automation.SetValueAsync(elementId, "some text");
        Console.WriteLine($"Set value success: {setValueResponse.Success}");
    }
}

if (!string.IsNullOrEmpty(windowIdForAutomation))
{
    // Bring a window to the front
    var bringToFrontResponse = await client.Automation.BringToFrontAsync(windowIdForAutomation);
    Console.WriteLine($"Bring to front success: {bringToFrontResponse.Success}");
}
```

### Code Execution

```csharp
// Execute specific C# code
var execResponse = await client.Code.ExecuteCSharpAsync("return System.DateTime.Now.ToString();");
if (execResponse.Success)
{
    Console.WriteLine($"Executed C#. Result: {execResponse.Result}");
}

// Generate and execute C# code based on a description - example 1
var genExecResponse1 = await client.Code.GenerateAndExecuteCSharpAsync("Calculate the factorial of 5");
if (genExecResponse1.Success)
{
    Console.WriteLine($"Generated/Executed Factorial. Result: {genExecResponse1.Result}");
}

// Generate and execute C# code based on a description - example 2
var genExecResponse2 = await client.Code.GenerateAndExecuteCSharpAsync(@"Return content of the biggest file in folder C:\\temp");
if (genExecResponse2.Success)
{
    Console.WriteLine($"Generated/Executed Find Biggest File. Result: {genExecResponse2.Result}");
}
else
{
    Console.WriteLine($"Failed Generate/Execute Find Biggest File: {genExecResponse2.Message}");
}

// Generate and execute C# code based on a description - example 3
var genExecResponse3 = await client.Code.GenerateAndExecuteCSharpAsync("Connect to Outlook via Interop and return subject and date of the latest email from 'test@example.com'");
if (genExecResponse3.Success)
{
    Console.WriteLine($"Generated/Executed Outlook Email. Result: {genExecResponse3.Result}");
}
else
{
     Console.WriteLine($"Failed Generate/Execute Outlook Email: {genExecResponse3.Message}");
}
```

## Advanced Usage

### Using Different AI Mechanisms

For AI-vision powered operations (e.g., `ClickByDescriptionAsync`), you can specify different AI mechanisms provided by ScreenGrasp.com:

```csharp
// Use a different AI mechanism (e.g., OpenAI's model)
var clickDescResponse = await client.Mouse.ClickByDescriptionAsync(
    "the Submit button",
    mechanism: MechanismType.OpenAIComputerUse // Specify the desired mechanism
);
Console.WriteLine($"Click by description (OpenAI) success: {clickDescResponse.Success} - {clickDescResponse.Message}");
```

Refer to the `MechanismType` enum in `SmoothOperator.AgentTools.Models` for available options.

### Converting Responses to JSON - Use LLMs to Analyze

Most response objects have a `ToJsonString()` method that converts the response to a formatted JSON string. This is highly recommended for passing state information to Large Language Models (LLMs) for analysis or decision-making.

```csharp
// Get a response, e.g., system overview
var overview = await client.System.GetOverviewAsync();

// Convert to JSON string
string jsonStr = overview?.ToJsonString(); // Use null-conditional operator

// Use the JSON string (e.g., pass it to a language model)
if (jsonStr != null)
{
    Console.WriteLine("\n--- System Overview JSON ---");
    Console.WriteLine(jsonStr);
    Console.WriteLine("--- End System Overview JSON ---\n");

    // Example prompt idea for an LLM:
    // string prompt = $"Given the following system overview:\n{jsonStr}\n\nIdentify the element ID of the 'Save' button in the focused window.";
    // Send prompt to LLM API...
}
```

It is a recommended pattern to use these JSON strings with LLMs to analyze the current system or application state.

For example, you can prompt GPT-4o to extract the CSS selector of "the UI element that can be clicked to submit the form" by providing a textual instruction and the JSON string from `client.Chrome.ExplainCurrentTabAsync().ToJsonString()` in a prompt.

Use the LLM's JSON mode (or structured output feature) if available to ensure it answers in a format you can easily parse in your C# code.

## Platform Support

The Smooth Operator Agent Tools C# library is designed primarily for **Windows** platforms, as the underlying server executable relies on Windows-specific APIs (UI Automation, etc.). 