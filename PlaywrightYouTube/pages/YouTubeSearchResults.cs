using Microsoft.Playwright;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightYouTube.Pages
{
    public class YouTubeSearchResults
    {
        private readonly IPage _page;
        // We target the custom HTML tag 'ytd-video-renderer' directly.
        // This is more resilient than relying on class names or other attributes that may change frequently.
        private const string SearchResultsSelector = "ytd-video-renderer";

        public YouTubeSearchResults(IPage page)
        {
            _page = page;
        }

        public async Task WaitForSearchResults()
        {
            // Locator provide built-in auto-waiting, so we don't need to add extra waits or try/catch blocks.            // Locators do not evaluate immediately; they provide strictness and built-in auto-waiting 
            // to eliminate race conditions and NullReferenceExceptions during fast page loads.
            await _page.Locator(SearchResultsSelector).First.WaitForAsync();
        }

        public async Task WaitForFilterOption(string optionName)
        {
            await _page.WaitForSelectorAsync($"text='{optionName}'");
        }

        public async Task WaitForMultipleFilterOptions(params string[] optionNames)
        {
            foreach (var option in optionNames)
            {
                await WaitForFilterOption(option);
            }
        }

        public async Task ClickFiltersButton()
        {
            await _page.GetByRole(AriaRole.Button, new() { Name = "Search filters" }).ClickAsync();
        }

        public async Task ClickFirstVideo()
        {
            // We chain locators to find the specific clickable title inside the video container.
            // The '.First' property prevents strict-mode violations when multiple videos load on the page.
            var firstVideoLink = _page.Locator($"{SearchResultsSelector} a#video-title").First;
            await firstVideoLink.ClickAsync();
        }
    
        public async Task SelectFilter(string optionName)
        {
            await _page.GetByRole(AriaRole.Link, new() { Name = optionName }).ClickAsync();
        }

        public async Task VerifyResultsUpdated()
        {
            // Using Playwright's native Expect().ToHaveURLAsync. This automatically polls the browser 
            // until the asynchronous network request updates the URL, eliminating flakiness.
            await Expect(_page).ToHaveURLAsync(new Regex(".*&sp=.*"));
        }
    }
}
