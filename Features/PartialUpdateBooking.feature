Feature: PartialUpdateBooking

  @Positive
  Scenario Outline: Partially update an existing booking
    Given I have a valid token
    And I create a booking with "<firstname>" and "<lastname>"
    When I patch the booking with new firstname "<patchFirstname>" and new lastname "<patchLastname>"
    Then the response status code for partialupdate call should be 200
    And the updated booking should contain "<patchFirstname>" and "<patchLastname>"

    Examples:
      | firstname | lastname | patchFirstname | patchLastname |
      | Alice     | Blue     | Alicia         | Red           |
      | Tom       | Smith    | Thomas         | White         |

  @Negative
  Scenario Outline: Partial update with authentication failures
    Given I create a booking with "John" and "Doe"
    When I patch the booking with <authType>
    Then the response status code should be 403

    Examples:
      | authType      |
      | no auth       |
      | invalid token |
