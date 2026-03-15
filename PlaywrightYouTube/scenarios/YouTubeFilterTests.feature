Feature: YouTube Filter Tests

  Background: 
    Given I am on search results for "Powershield"

  Scenario: Opening filters shows filter options
    When I click on the "Filters" button
    Then I should see filter options such as "Upload date", "Type", "Duration", and "Features"

  Scenario Outline: Applying a filter narrows down results
    When I click on the "Filters" button
    And I select the "<FilterType>" filter
    Then the search results should be updated to reflect the applied filter
    
    Examples:
      | FilterType      |
      | Videos          |
      | Under 3 minutes |
