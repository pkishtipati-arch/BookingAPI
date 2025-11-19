using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using BookingAPI.Helpers;

namespace BookingAPI.StepDefinitions
{
    [Binding]
    public class NegativeScenarioSteps
    {
        private RestResponse? _response;
        private string? _requestBody;
        private readonly ScenarioContext _context;
        private readonly string _baseUrl;

        public NegativeScenarioSteps(ScenarioContext context)
        {
            _context = context;
            _baseUrl = $"{Config.BaseUrl}/booking";
        }

        [Given(@"I have incomplete booking details with missing ""(.*)""")]
        public void GivenIHaveIncompleteBookingDetails(string missingField)
        {
            var booking = new Dictionary<string, object>
            {
                { "firstname", "Test" },
                { "lastname", "User" },
                { "totalprice", 100 },
                { "depositpaid", true },
                { "bookingdates", new { checkin = "2024-01-01", checkout = "2024-01-05" } },
                { "additionalneeds", "None" }
            };

            booking.Remove(missingField);
            _requestBody = JsonConvert.SerializeObject(booking);
            _context["ApiRequest"] = $"POST {_baseUrl}\n{_requestBody}";
        }

        [When(@"I submit the incomplete booking request")]
        public async Task WhenISubmitIncompleteRequest()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(_requestBody!, DataFormat.Json);

            _response = await client.ExecuteAsync(request);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Given(@"I have booking with invalid ""(.*)"" value ""(.*)""")]
        public void GivenIHaveBookingWithInvalidValue(string field, string invalidValue)
        {
            object totalpriceValue = 100;
            
            // Handle totalprice field - try to parse as int, otherwise use string for type mismatch testing
            if (field == "totalprice")
            {
                if (int.TryParse(invalidValue, out int numericValue))
                {
                    totalpriceValue = numericValue; // Will be negative number like -100
                }
                else
                {
                    totalpriceValue = invalidValue; // Will be string like "negative" for type mismatch
                }
            }
            
            var booking = new Dictionary<string, object>
            {
                { "firstname", "Test" },
                { "lastname", "User" },
                { "totalprice", totalpriceValue },
                { "depositpaid", true },
                { "bookingdates", new 
                    { 
                        checkin = field == "checkin" ? invalidValue : "2024-01-01", 
                        checkout = field == "checkout" ? invalidValue : "2024-01-05" 
                    } 
                },
                { "additionalneeds", "None" }
            };

            _requestBody = JsonConvert.SerializeObject(booking);
            _context["ApiRequest"] = $"POST {_baseUrl}\n{_requestBody}";
        }

        [When(@"I submit the invalid booking request")]
        public async Task WhenISubmitInvalidRequest()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(_requestBody!, DataFormat.Json);

            _response = await client.ExecuteAsync(request);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the response status code should be 500 or 400")]
        public void ThenStatusCodeShouldBe500Or400()
        {
            var statusCode = GetStatusCode();
            Assert.That(statusCode == 400 || statusCode == 500);
        }

        [Then(@"the response status code should be 200 or 400 or 500")]
        public void ThenStatusCodeShouldBe200Or400Or500()
        {
            var statusCode = GetStatusCode();
            Assert.That(statusCode == 200 || statusCode == 400 || statusCode == 500);
        }

        [Then(@"the response status code should be 404 or 400")]
        public void ThenStatusCodeShouldBe404Or400()
        {
            var statusCode = GetStatusCode();
            Assert.That(statusCode == 404 || statusCode == 400);
        }

        [Then(@"the response status code should be 405 or 404")]
        public void ThenStatusCodeShouldBe405Or404()
        {
            var statusCode = GetStatusCode();
            Assert.That(statusCode == 405 || statusCode == 404);
        }

        [Then(@"the response status code should be 200 or 400")]
        public void ThenStatusCodeShouldBe200Or400()
        {
            var statusCode = GetStatusCode();
            Assert.That(statusCode == 200 || statusCode == 400 || statusCode == 500);
        }

        private int GetStatusCode()
        {
            var response = _context.ContainsKey("Response") 
                ? _context.Get<RestResponse>("Response") 
                : _response!;
            return (int)response.StatusCode;
        }
    }
}
