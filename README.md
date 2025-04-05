# Smooth Operator Agent Tools - C# Library

[![NuGet version](https://badge.fury.io/nu/SmoothOperator.AgentTools.svg)](https://badge.fury.io/nu/SmoothOperator.AgentTools)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This is the official C# library implementation for [Smooth Operator Agent Tools](https://smooth-operator.online/agent-tools-api-docs/toolserverdocs), a toolkit designed for developers creating Computer Use Agents on Windows systems. It provides programmatic access to control and interact with the Windows environment, including:

*   **System Operations:** Opening applications, getting window details.
*   **UI Automation:** Retrieving UI element trees (Automation Tree).
*   **Mouse & Keyboard:** Simulating user input (typing, clicking).
*   **ScreenGrasp Integration:** Using AI vision (via ScreenGrasp API) to interact with UI elements based on visual descriptions.
*   **Chrome Automation:** Interacting with Google Chrome.
*   **Code Execution:** Running code snippets.

This library provides a convenient wrapper around the Smooth Operator Tools Server API, allowing you to easily integrate these capabilities into your .NET applications.

## Installation

Using the .NET CLI:
```bash
dotnet add package SmoothOperator.AgentTools
```

Or using the NuGet Package Manager console:
```powershell
Install-Package SmoothOperator.AgentTools
```

## Prerequisites

### Google Chrome

The Smooth Operator Agent Tools library requires Google Chrome (or a compatible Chromium-based browser) to be installed on the system for browser automation features to work.

## Server Installation

The Smooth Operator client library includes a server component (`smooth-operator-server.exe` and associated files) that needs to be installed in your application data directory (`%APPDATA%\SmoothOperator\AgentToolsServer`). The necessary server files are embedded as resources within the `SmoothOperator.AgentTools` library.

### Automatic Installation

When you call `client.StartServerAsync()` or `client.EnsureInstalledAsync()` for the first time, the library will automatically:
1. Check if the server is already installed and up-to-date by comparing version files (`installedversion.txt`).
2. If necessary, create the directory `%APPDATA%\SmoothOperator\AgentToolsServer`.
3. Extract the embedded server files into this directory.
4. Start the server process (if using `StartServerAsync`).

This ensures the correct server version is always available without manual installation steps.

## Features

*   **Fluent API:** Provides a clear and easy-to-use API categorized by functionality (System, Mouse, Keyboard, Screenshot, Automation, Chrome, Code).
*   **Automatic Server Management:** The client library can automatically start the background server process when needed.
*   **Cross-Platform Compatibility (Client):** The client library itself runs on any platform supported by .NET.
*   **Windows Server Requirement:** The underlying `smooth-operator-server.exe` currently requires a Windows environment to perform the automation tasks.
*   **Strong Typing:** Full C# type safety and IntelliSense support.

## Basic Usage

```csharp
using SmoothOperator.AgentTools;
using SmoothOperator.AgentTools.Models;

async Task Main()
{
    // Initialize the client with your API key (required for ScreenGrasp features)
    // Get a free key at https://screengrasp.com/api.html
    var client = new SmoothOperatorClient("YOUR_SCREENGRASP_API_KEY");

    try
    {
        // Start the Server - this also handles installation/updates and takes a moment
        await client.StartServerAsync();
        Console.WriteLine("Smooth Operator Server connected.");

        // Example: Open Calculator
        Console.WriteLine("Opening Calculator...");
        await client.System.OpenApplicationAsync("calc.exe");
        await Task.Delay(2000); // Wait for app to open

        // Example: Type '5*6'
        Console.WriteLine("Typing '5*6'...");
        await client.Keyboard.TypeAsync("5*6");
        await Task.Delay(500);

        // Example: Click the 'equals' button using ScreenGrasp
        Console.WriteLine("Clicking 'equals' button...");
        var clickResult = await client.Mouse.ClickByDescriptionAsync("equals button");
        if (!clickResult.Success)
        {
            Console.WriteLine($"Failed to click 'equals' button: {clickResult.Message}");
        }
        await Task.Delay(1000); // Wait for calculation

        // Example: Get the UI overview of the focused window (Calculator)
        Console.WriteLine("Getting UI overview...");
        var overviewResult = await client.System.GetOverviewAsync();
        if (overviewResult.Success)
        {
            Console.WriteLine("Calculator UI Overview (Automation Tree):");
            Console.WriteLine(overviewResult.Overview.ToJsonString());
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    finally
    {
        // Stop the server process if it was started by this client instance
        client.StopServer();
        Console.WriteLine("Smooth Operator Server stopped.");
    }
}
```

*(See the `example-csharp` project in the repository for a more complete, runnable example.)*

## Documentation

For detailed API documentation, please refer to:

*   **[Usage Guide](docs/usage_guide.md):** Detailed examples and explanations for common use cases.
*   **[Example Project](https://github.com/fstandhartinger/smooth-operator-example-csharp):** Download, follow step-by-step instructions, and have your first automation running in minutes.
*   **[API Documentation](https://smooth-operator.online/agent-tools-api-docs/toolserverdocs):** Describes the endpoints of the local tools server the C# client library communicates with.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
