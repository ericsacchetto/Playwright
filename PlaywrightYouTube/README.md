# YouTube Playwright Test Automation Framework

This repository contains a robust, scalable UI test automation framework designed to validate search and filtering functionality on YouTube. 

It is built using **C#**, **Playwright**, and **SpecFlow** (BDD), demonstrating modern test architecture, state management, and resilient locator strategies.

## Key Architectural Highlights

* **Page Object Model (POM):** UI interactions are abstracted away from test logic, ensuring high maintainability when the DOM changes.
* **Dependency Injection:** Utilizes SpecFlow's BoDi container to inject a single Playwright `IPage` instance across multiple step definition files, eliminating duplicate browser sessions.
* **Web-First Assertions:** Replaces manual exceptions and brittle waits with Playwright's native auto-polling assertions (e.g., `Expect(locator).ToBeVisibleAsync()`).
* **Accessibility-First Locators:** Prioritizes user-facing attributes (`GetByRole`, `GetByPlaceholder`) over volatile CSS classes for maximum test stability.

## Debugging & Tracing (CI/CD Ready)

To optimize CI/CD storage and streamline debugging, this framework utilizes **Playwright Tracing** dynamically:
* Traces are captured in the background during test execution.
* The framework evaluates the test outcome at the end of the scenario. **A trace `.zip` file is only saved to the disk if the test fails.**
* This trace captures full DOM snapshots, console logs, network traffic, and C# source code execution, allowing developers to "time-travel" through the exact moment of failure without needing to reproduce it locally.

**How to view a trace:**
1. Navigate to the `Traces/` directory in your build output folder (e.g., `bin/Debug/net8.0/Traces`).
2. Open your browser and go to [trace.playwright.dev](https://trace.playwright.dev).
3. Drag and drop the generated `.zip` file into the window to inspect the failure.

## Prerequisites

Before running the tests, ensure you have the following installed on your machine:
* [.NET SDK](https://dotnet.microsoft.com/download) (Version 6.0 or higher)
* An IDE such as Visual Studio, JetBrains Rider, or VS Code.

## How to Install

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/ericsacchetto/Playwright.git](https://github.com/ericsacchetto/Playwright.git)
   cd PlaywrightYouTube
   ```

2. **Restore NuGet packages:**
  ```bash
  dotnet restore
  ```

3. **Build the project:**
  ```bash
  dotnet build
  ```

4. **Install Playwright Browsers:**
  ```bash
  pwsh bin/Debug/net8.0/playwright.ps1 install
  ```

## Project Structure

The framework is strictly organized by separation of concerns:
PlaywrightYouTube/
│
├── Features/              # Gherkin (.feature) files defining test scenarios (BDD)
│   ├── YouTubeSearchTests.feature
│   └── YouTubeFilterTests.feature
│
├── Steps/                 # SpecFlow bindings translating Gherkin into C# execution
│   ├── YouTubeSearchTestsSteps.cs
│   ├── YouTubeFilterTestsSteps.cs
│   └── Hooks.cs           # Global setup/teardown
│
└── Pages/                 # Page Object Model classes mapping UI elements and actions
    ├── YouTubeHomePage.cs
    ├── YouTubeSearchResults.cs
    └── YouTubeVideoPlayer.cs

## How to Run It

**Option 1: Using the .NET CLI (Headless Mode)**
To run the entire test suite from your terminal:
  ```bash
  dotnet test
  ```

**Option 2: Using Visual Studio Test Explorer (UI Mode)**
If you want to watch the browser execute the tests (headed mode):
1. Open the solution in Visual Studio.
2. Open the Test Explorer (`Test` > `Test Explorer`).
3. Ensure the `Headless = false` flag is set in the `Hooks.cs` file.
4. Click Run All Tests (or right-click specific scenarios to run them individually).