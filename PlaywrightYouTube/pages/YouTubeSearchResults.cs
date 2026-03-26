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

        public async Task VerifyUrlContains(string urlPart)
        {
            // Using Playwright's native Expect().ToHaveURLAsync. This automatically polls the browser 
            // until the asynchronous network request updates the URL, eliminating flakiness.
            await Expect(_page).ToHaveURLAsync(new Regex($".*{Regex.Escape(urlPart)}.*"));
        }

        public async Task VerifyVideoDuration()
        {
            // Get all video results and find the first one with a valid duration
            var allVideos = _page.Locator(SearchResultsSelector);
            int videoCount = await allVideos.CountAsync();
            
            if (videoCount == 0)
            {
                throw new Exception("No videos found in search results");
            }
            
            // Try the first few videos (in case there are sponsored/ads at the top)
            string? durationText = null;
            int foundVideoIndex = -1;
            for (int i = 0; i < Math.Min(videoCount, 3); i++)
            {
                var video = allVideos.Nth(i);
                var textElements = await video.Locator("xpath=//*[contains(text(), ':')]").AllTextContentsAsync();
                
                foreach (var text in textElements)
                {
                    if (Regex.IsMatch(text.Trim(), @"^\d{1,2}:\d{2}$"))
                    {
                        durationText = text.Trim();
                        foundVideoIndex = i;
                        break;
                    }
                }
                
                if (durationText != null)
                    break;
            }
            
            if (durationText == null)
            {
                throw new Exception("Video duration not found in first few search results");
            }

            // Extract and print video title for debugging
            var foundVideo = allVideos.Nth(foundVideoIndex);
            var titleElement = foundVideo.Locator("a#video-title");
            var videoTitle = await titleElement.GetAttributeAsync("title");
            Console.WriteLine($"[DEBUG] Found video at index {foundVideoIndex}: '{videoTitle}' with duration {durationText}");

            var parts = durationText.Split(':');
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);
            int totalSeconds = minutes * 60 + seconds;
            if (totalSeconds >= 180)
            {
            throw new Exception($"Video duration {durationText} is 3 minutes or longer");
            }
        }

        public async Task VerifyMovieLabel()
        {
            // Verify the YouTube Movies label is visible
            await _page.GetByText("YouTube Movies").First.IsVisibleAsync();
            
            // Get the first movie result and extract its title for debugging
            // Movies are in ytd-movie-renderer, not ytd-video-renderer
            var firstMovie = _page.Locator("ytd-movie-renderer").First;
            var titleElement = firstMovie.Locator("a#video-title");
            var movieTitle = await titleElement.GetAttributeAsync("title");
            Console.WriteLine($"[DEBUG] Found movie: '{movieTitle}'");
        }

        public async Task WaitForProgressBarToDisappear()
        {
            await Expect(_page.Locator("yt-page-navigation-progress")).ToBeHiddenAsync();
        }
    }
}
