@GetBookingIds
Feature: Get Booking IDs

  @Smoke @Positive
  Scenario: Get all booking IDs
    When I request all booking IDs
    Then the response status code should be 200
    And the response should contain a list of booking IDs

  @Regression @Positive
  Scenario Outline: Filter bookings by name
    When I request booking IDs filtered by firstname "<firstname>" and lastname "<lastname>"
    Then the response status code should be 200
    And the response should contain filtered booking IDs

    Examples:
      | firstname | lastname |
      | Sally     | Brown    |
      | John      | Smith    |

  @Regression @Negative
  Scenario: Filter bookings with invalid date format
    When I request booking IDs with invalid checkin date "invalid-date"
    Then the response status code should be 500
