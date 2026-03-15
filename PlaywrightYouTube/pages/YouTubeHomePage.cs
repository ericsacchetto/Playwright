using Microsoft.Playwright;

namespace PlaywrightYouTube.Pages
{
    public class YouTubeHomePage
    {
        // Using 'readonly' to ensure the page state cannot be accidentally reassigned 
        private readonly IPage _page;
        private const string Url = "https://youtube.com";

        public YouTubeHomePage(IPage page)
        {
            _page = page;
        }

        public async Task Navigate()
        {
            await _page.GotoAsync(Url);
        }

        public async Task SearchFor(string searchTerm)
        {
            // Using GetByPlaceholder targets user-facing accessibility attributes. 
            // If developers change the underlying DOM structure or CSS framework,
            // this locator survives because the user-facing UI hasn't changed.
            var searchInput = _page.GetByPlaceholder("Search").First;
            await searchInput.ClickAsync();
            await searchInput.FillAsync(searchTerm);
            await searchInput.PressAsync("Enter");
        }

    }
}
