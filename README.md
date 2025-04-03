# Smooth Operator Agent Tools - C# Library

This is the official C# library implementation for Smooth Operator Agent Tools, a state-of-the-art toolkit for programmers developing Computer Use Agents on Windows systems.

## Overview

The Smooth Operator Agent Tools are a powerful toolkit that handles the complex tasks of interacting with the Windows Automation Tree and Playwright browser control, while providing advanced AI functions such as identifying UI elements through screenshots and textual descriptions.

This C# library provides a convenient wrapper around the Smooth Operator Tools Server API, allowing you to easily integrate these capabilities into your .NET applications.

## Installation

```bash
dotnet add package SmoothOperator.AgentTools
```

Or using the NuGet Package Manager:

```
Install-Package SmoothOperator.AgentTools
```

## Prerequisites

### Google Chrome

The Smooth Operator Agent Tools library requires Google Chrome (or a compatible Chromium-based browser) to be installed on the system for browser automation features to work.

## Usage

```csharp
using SmoothOperator.AgentTools;
using SmoothOperator.AgentTools.Models;

// Initialize the client with your API key, get it for free at https://screengrasp.com/api.html
var client = new SmoothOperatorClient("YOUR_API_KEY");

// Start the Server - this takes a moment (especially the first time)
await client.StartServerAsync();

// Take a screenshot
var screenshot = await client.Screenshot.TakeAsync();

// Get system overview
var overview = await client.System.GetOverviewAsync();

// Perform a mouse click
await client.Mouse.ClickAsync(500, 300);

// Find and click a UI element by description
await client.Mouse.ClickByDescriptionAsync("Submit button");

// Type text
await client.Keyboard.TypeAsync("Hello, world!");

// Control Chrome browser
await client.Chrome.OpenChromeAsync("https://www.example.com");
await client.Chrome.GetDomAsync();

//you can also use the .ToJsonString() method on lots of these objects
//to get a json string that can easily be used in a prompt to a LLM
//to utilize AI even more for automated decision making
```

## Features

- **Screenshot and Analysis**: Capture screenshots and analyze UI elements
- **Mouse Control**: Precise mouse operations using coordinates or AI-powered element detection
- **Keyboard Input**: Type text and send key combinations
- **Chrome Browser Control**: Navigate, interact with elements, and execute JavaScript
- **Windows Automation**: Interact with Windows applications and UI elements
- **System Operations**: Open applications and manage system state
- **MCP Server**: once client.EnsureInstalledAsync() has been called, you find it at %appdata%\smoothoperator\AgentToolsServer\smooth-operator-server.exe

## Documentation

For detailed API documentation, visit:
[https://smooth-operator.online/agent-tools-api-docs/toolserverdocs](https://smooth-operator.online/agent-tools-api-docs/toolserverdocs)
*   **[Usage Guide (docs/usage_guide.md)](docs/usage_guide.md):** Detailed examples and explanations for common use cases.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
