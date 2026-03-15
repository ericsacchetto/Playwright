using Microsoft.Playwright;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightYouTube.Pages
{
    public class YouTubeVideoPlayer
    {
        private IPage _page;

        public YouTubeVideoPlayer(IPage page)
        {
            _page = page;
        }

        public async Task WaitForVideoPlayer()
        {
            await _page.Locator("video").First.WaitForAsync();
        }

        public async Task VerifyVideoPlayerVisible()
        {
            var videoPlayer = _page.Locator("video").First;

            // ToBeVisibleAsync doesn't just check if the element exists in the DOM; 
            // it verifies that the element has a non-zero bounding box, is not hidden by CSS, 
            // and is actually interactable/visible to the end user.
            await Expect(videoPlayer).ToBeVisibleAsync();
        }

        public async Task VerifyURLContains(string urlPart)
        {
            // Regex allows for flexible URL matching regardless of dynamic session tokens 
            // or tracking parameters that YouTube might append to the URL.
            await Expect(_page).ToHaveURLAsync(new Regex($".*{urlPart}.*"));
        }
    }
}