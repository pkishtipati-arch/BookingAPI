@GetBooking
Feature: Get Booking

  @Smoke @Positive
  Scenario: Get booking by valid ID
    Given I have a valid booking ID
    When I request the booking details
    Then the response status code should be 200
    And the response should contain all required fields
      | field         |
      | firstname     |
      | lastname      |
      | totalprice    |
      | depositpaid   |
      | bookingdates  |

  @Regression @Negative
  Scenario: Get booking with non-existent ID
    Given I have an invalid booking ID "999999"
    When I request the booking details with invalid ID
    Then the response status code should be 404

  @Regression @Negative
  Scenario Outline: Get booking with malformed ID
    Given I have an invalid booking ID "<invalidId>"
    When I request the booking details with invalid ID
    Then the response status code should be 404

    Examples:
      | invalidId |
      | -1        |
      | abc       |
