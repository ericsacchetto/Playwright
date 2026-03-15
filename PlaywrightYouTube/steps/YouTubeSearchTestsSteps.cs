using TechTalk.SpecFlow;
using Microsoft.Playwright;
using PlaywrightYouTube.Pages;
using static Microsoft.Playwright.Assertions;

[Binding]
public class YouTubeStepDefinitions
{
    private IPage? _page;
    private YouTubeHomePage? _homePage;
    private YouTubeSearchResults? _searchResults;
    private YouTubeVideoPlayer? _videoPlayer;

    public YouTubeStepDefinitions(IPage page) 
    {
        _page = page;
        _homePage = new YouTubeHomePage(_page);
        _searchResults = new YouTubeSearchResults(_page);
        _videoPlayer = new YouTubeVideoPlayer(_page);
    }

    [Given(@"I am on search results for ""(.*)""")]
    public async Task GivenIAmOnSearchResultsFor(string searchTerm)
    {
        await _homePage!.Navigate();
        await _homePage!.SearchFor(searchTerm);
    }

    [Then(@"I should see search results")]
    public async Task ThenIShouldSeeSearchResults()
    {
        await _searchResults!.WaitForSearchResults();
    }

    [When(@"I click on the first video in the search results")]
    public async Task WhenIClickOnTheFirstVideoInSearchResults()
    {
        await _searchResults!.ClickFirstVideo();
    }

    [Then(@"the video should start playing")]
    public async Task ThenTheVideoShouldStartPlaying()
    {
        await _videoPlayer!.WaitForVideoPlayer();
    }

    [Then(@"the URL should contain ""(.*)""")]
    public async Task ThenTheURLShouldContain(string urlPart)
    {
        await _videoPlayer!.VerifyURLContains(urlPart);
    }

    [Then(@"the video player container should be visible")]
    public async Task ThenTheVideoPlayerContainerShouldBeVisible()
    {
        await _videoPlayer!.VerifyVideoPlayerVisible();
    }

    [When(@"I navigate back to the last page")]
    public async Task WhenINavigateBackToTheHomePage()
    {
        await _page!.GoBackAsync();
    }

    [Then(@"the search bar should still contain ""(.*)""")]
    public async Task ThenTheSearchBarShouldStillContain(string expectedSearchTerm)
    {
        var searchInput = _page!.GetByPlaceholder("Search").First;
        await Expect(searchInput).ToHaveValueAsync(expectedSearchTerm);
    }
}