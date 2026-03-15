Feature: YouTube Search Functionality

  Background: 
    Given I am on search results for "Playwright testing"

  Scenario: Search results are displayed
    Then I should see search results
  
  Scenario: Playing a video from search results
    When I click on the first video in the search results
    Then the video should start playing
    And the URL should contain "/watch"
    And the video player container should be visible

  Scenario: Returning to last page keeps the search term in the search bar
    When I click on the first video in the search results
    And I navigate back to the last page
    Then the search bar should still contain "Playwright testing"