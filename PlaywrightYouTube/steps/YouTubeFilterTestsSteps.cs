using TechTalk.SpecFlow;
using Microsoft.Playwright;
using PlaywrightYouTube.Pages;

[Binding]
public class YouTubeFilterTestsSteps
{
    private IPage? _page;
    private YouTubeSearchResults? _searchResults;

    public YouTubeFilterTestsSteps(IPage page) 
    {
        _page = page;
        _searchResults = new YouTubeSearchResults(_page);
    }

    [When(@"I click on the Filters button")]
    public async Task WhenIClickOnTheButton()
    {
        await _searchResults!.ClickFiltersButton();
    }

    [Then(@"I should see filter options such as ""(.*)"", ""(.*)"", ""(.*)"", and ""(.*)""")]
    public async Task ThenIShouldSeeFilterOptions(string option1, string option2, string option3, string option4)
    {
        await _searchResults!.WaitForMultipleFilterOptions(option1, option2, option3, option4);
    }

    [When(@"I select the ""(.*)"" filter")]
    public async Task WhenISelectTheFilter(string filterType)
    {
        await _searchResults!.SelectFilter(filterType);
        await _searchResults!.VerifyUrlContains("&sp=");
    }

    [Then(@"the search results should be updated to reflect the ""(.*)"" filter")]
    public async Task ThenTheSearchResultsShouldBeUpdatedToReflectTheAppliedFilter(string filterType)
    {
        if (filterType.Equals("Under 3 minutes", StringComparison.OrdinalIgnoreCase))
        {
            await _searchResults!.VerifyUrlContains("sp=EgIYBA%253D%253D");
            await _searchResults.WaitForProgressBarToDisappear();
            await _searchResults.VerifyVideoDuration();
        }
        else if (filterType.Equals("Movies", StringComparison.OrdinalIgnoreCase))
        {
            await _searchResults!.VerifyUrlContains("sp=EgIQBA%253D%253D");
            await _searchResults.WaitForProgressBarToDisappear();
            await _searchResults.VerifyMovieLabel();
        }
        else
        {
            throw new ArgumentException($"Unknown filter type: {filterType}");
        }
    }
}