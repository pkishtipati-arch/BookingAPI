@UpdateBooking
Feature: Update Booking

  Background:
    Given I have a valid booking ID
    And I have a valid auth token

  @Smoke @Positive
  Scenario: Update booking with valid auth token
    Given I have updated booking details "Jane", "Doe", 150, true, "2024-02-01", "2024-02-05", "Parking"
    When I submit the update request
    Then the response status code should be 200
    And the booking should be updated with "Jane", "Doe", 150, true

  @Regression @Negative
  Scenario Outline: Update booking with authentication failures
    Given I have updated booking details "Jane", "Doe", 150, true, "2024-02-01", "2024-02-05", "Parking"
    When I submit the update request with <authType>
    Then the response status code should be 403
    And the response should contain error message "Forbidden"

    Examples:
      | authType      |
      | no auth       |
      | invalid token |

  @Regression @Negative
  Scenario: Update non-existent booking
    Given I have a non-existent booking ID 999999
    And I have updated booking details "Jane", "Doe", 150, true, "2024-02-01", "2024-02-05", "Parking"
    When I submit the update request
    Then the response status code should be 405 or 404
