@DeleteBooking
Feature: Delete Booking
  
  @Smoke @Positive
  Scenario: Delete an existing booking
    Given I have a valid booking ID
    And I have a valid auth token
    When I delete the booking
    Then the response status code for delete request should be 201
    And the response body should be "Created"
    And getting the deleted booking should return 404

  @Negative
  Scenario: Delete booking without auth token
    Given I have a valid booking ID
    When I delete the booking without auth
    Then the response status code should be 403
    And the response should contain error message "Forbidden"
