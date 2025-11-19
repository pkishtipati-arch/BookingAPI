@CreateBooking
Feature: Create Booking

  @Smoke @Positive
  Scenario Outline: Create booking with valid data
    Given I have booking details "<firstname>", "<lastname>", <totalprice>, <depositpaid>, "<checkin>", "<checkout>", "<additionalneeds>"
    When I submit the booking request
    Then the response status code for create request should be 200
    And the response should include the booking with firstname "<firstname>" and lastname "<lastname>"

    Examples:
      | firstname | lastname | totalprice | depositpaid | checkin    | checkout   | additionalneeds |
      | Jim       | Brown    | 111        | true        | 2018-01-01 | 2019-01-01 | Breakfast       |
      | Sally     | Green    | 222        | false       | 2021-05-01 | 2021-05-10 | Lunch           |
      | John      | Doe      | 500        | true        | 2024-01-15 | 2024-01-20 | WiFi            |

  @Regression @Negative
  Scenario Outline: Create booking with missing fields
    Given I have incomplete booking details with missing "<missingField>"
    When I submit the incomplete booking request
    Then the response status code should be 500 or 400

    Examples:
      | missingField |
      | firstname    |
      | lastname     |
      | totalprice   |

  @Regression @Negative
  Scenario Outline: Create booking with invalid data
    Given I have booking with invalid "<field>" value "<invalidValue>"
    When I submit the invalid booking request
    Then the response status code should be 200 or 400 or 500

    Examples:
      | field      | invalidValue |
      | totalprice | -100         |
      | checkin    | invalid-date |
      | checkout   | 2024-13-45   |

  @Regression @Positive
  Scenario Outline: Create booking with special characters
    Given I have booking details "<firstname>", "<lastname>", 100, true, "2024-01-01", "2024-01-05", "None"
    When I submit the booking request
    Then the response status code for create request should be 200

    Examples:
      | firstname | lastname |
      | O'Brien   | Smith    |
      | Jean-Luc  | Picard   |
      | José      | García   |
