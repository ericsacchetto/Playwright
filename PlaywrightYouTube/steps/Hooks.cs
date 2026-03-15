using TechTalk.SpecFlow;
using Microsoft.Playwright;
using BoDi;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightYouTube.Steps
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;

        public Hooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            
            // 1. Create an isolated Browser Context
            _context = await _browser.NewContextAsync();

            // 2. Start Tracing on that context before opening the page
            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            // 3. Open the page from the context, not the browser
            var page = await _context.NewPageAsync();

            // 4. Register the page for Dependency Injection
            _container.RegisterInstanceAs<IPage>(page); 
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_context != null)
            {
                // Check if the scenario failed
                if (_scenarioContext.TestError != null)
                {
                    // Create a safe filename based on the scenario title
                    var scenarioTitle = string.Join("_", _scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()));
                    var tracePath = Path.Combine(Directory.GetCurrentDirectory(), "Traces", $"trace_{scenarioTitle}.zip");

                    Console.WriteLine($"Test failed. Saving trace to: {tracePath}");

                    // Stop tracing and save the file
                    await _context.Tracing.StopAsync(new TracingStopOptions
                    {
                        Path = tracePath
                    });
                }
                else
                {
                    // If the test passed, just stop tracing without saving to save disk space
                    await _context.Tracing.StopAsync();
                }

                await _context.CloseAsync();
            }

            if (_browser != null) await _browser.CloseAsync();
            if (_playwright != null) _playwright.Dispose();
        }
    }
}