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

    [When(@"I click on the ""(.*)"" button")]
    public async Task WhenIClickOnTheButton(string buttonName)
    {
        if (buttonName == "Filters")
        {
            await _searchResults!.ClickFiltersButton();
        }
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
    }

    [Then(@"the search results should be updated to reflect the applied filter")]
    public async Task ThenTheSearchResultsShouldBeUpdatedToReflectTheAppliedFilter()
    {
        // Verifies the page updated after applying the filter
        await _searchResults!.VerifyResultsUpdated();
    }
}