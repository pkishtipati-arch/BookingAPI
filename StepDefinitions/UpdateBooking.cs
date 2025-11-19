using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using BookingAPI.Services;
using BookingAPI.Models;
using BookingAPI.Helpers;

namespace BookingAPI.StepDefinitions
{
    [Binding]
    public class UpdateBookingSteps
    {
        private RestResponse? _response;
        private string? _token;
        private BookingRequest? _bookingRequest;
        private readonly ScenarioContext _context;
        private readonly IBookingApiClient _bookingClient;

        public UpdateBookingSteps(ScenarioContext context)
        {
            _context = context;
            _bookingClient = new BookingApiClient($"{Config.BaseUrl}/booking");
        }

        [Given(@"I have an invalid auth token")]
        public void GivenIHaveInvalidAuthToken()
        {
            _token = "invalid_token_123";
            _context["AuthToken"] = _token;
        }

        [Given(@"I have updated booking details ""(.*)"", ""(.*)"", (.*), (.*), ""(.*)"", ""(.*)"", ""(.*)""")]
        public void GivenIHaveUpdatedBookingDetails(string firstname, string lastname, int totalprice,
            bool depositpaid, string checkin, string checkout, string additionalneeds)
        {
            _bookingRequest = new BookingRequest
            {
                Firstname = firstname,
                Lastname = lastname,
                TotalPrice = totalprice,
                DepositPaid = depositpaid,
                BookingDates = new BookingDates
                {
                    CheckIn = checkin,
                    CheckOut = checkout
                },
                AdditionalNeeds = additionalneeds
            };
        }

        [When(@"I submit the update request")]
        public async Task WhenISubmitUpdateRequest()
        {
            var bookingId = int.Parse(_context["BookingId"].ToString()!);
            var token = _context.ContainsKey("AuthToken") ? _context["AuthToken"].ToString()! : _token!;
            
            _context["ApiRequest"] = $"PUT /booking/{bookingId}\n{JsonConvert.SerializeObject(_bookingRequest)}";
            _response = await _bookingClient.UpdateBookingAsync(bookingId, _bookingRequest!, token);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I submit the update request with (.*)")]
        public async Task WhenISubmitUpdateRequestWith(string authType)
        {
            var bookingId = int.Parse(_context["BookingId"].ToString()!);
            var token = authType == "invalid token" ? "invalid_token_123" : _token!;
            
            _context["ApiRequest"] = $"PUT /booking/{bookingId}\n{JsonConvert.SerializeObject(_bookingRequest)}\nAuth: {authType}";
            _response = await _bookingClient.UpdateBookingAsync(bookingId, _bookingRequest!, token);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the booking should be updated with ""(.*)"", ""(.*)"", (.*), (.*)")]
        public void ThenBookingShouldBeUpdated(string expectedFirstname, string expectedLastname, 
            int expectedPrice, bool expectedDepositpaid)
        {
            var response = _context.Get<RestResponse>("Response");
            var json = JsonConvert.DeserializeObject<JObject>(response.Content!);
            
            Assert.That(json!["firstname"]!.ToString(), Is.EqualTo(expectedFirstname));
            Assert.That(json["lastname"]!.ToString(), Is.EqualTo(expectedLastname));
            Assert.That(json["totalprice"]!.Value<int>(), Is.EqualTo(expectedPrice));
            Assert.That(json["depositpaid"]!.Value<bool>(), Is.EqualTo(expectedDepositpaid));
        }

        [Then(@"the response should contain error message ""(.*)""")]
        public void ThenResponseShouldContainErrorMessage(string expectedMessage)
        {
            var response = _context.Get<RestResponse>("Response");
            Assert.That(response.Content, Does.Contain(expectedMessage).IgnoreCase);
        }
    }
}
