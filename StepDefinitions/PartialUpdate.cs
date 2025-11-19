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
    [Binding, Scope(Feature = "PartialUpdateBooking")]
    public class PartialUpdateSteps
    {
        private readonly IBookingApiClient _bookingClient;
        private readonly AuthService _authService;
        private RestResponse? _response;
        private int _bookingId;
        private string? _token;
        private readonly ScenarioContext _context;

        public PartialUpdateSteps(ScenarioContext context)
        {
            _context = context;
            _bookingClient = new BookingApiClient($"{Config.BaseUrl}/booking");
            _authService = new AuthService(Config.BaseUrl);
        }

        [Given(@"I have a valid token")]
        public async Task GivenIHaveValidToken()
        {
            _token = await _authService.GetToken();
        }

        [Given(@"I create a booking with ""(.*)"" and ""(.*)""")]
        public async Task GivenICreateBooking(string firstname, string lastname)
        {
            var booking = new BookingRequest
            {
                Firstname = firstname,
                Lastname = lastname,
                TotalPrice = 123,
                DepositPaid = true,
                BookingDates = new BookingDates
                {
                    CheckIn = "2020-01-01",
                    CheckOut = "2020-01-10"
                },
                AdditionalNeeds = "None"
            };

            var response = await _bookingClient.CreateBookingAsync(booking);
            
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                throw new Exception($"Booking creation failed: {response.StatusCode}");
            
            var json = JObject.Parse(response.Content);
            _bookingId = json["bookingid"]!.Value<int>();
        }

        [When(@"I patch the booking with new firstname ""(.*)"" and new lastname ""(.*)""")]
        public async Task WhenIPatchBooking(string newFirstname, string newLastname)
        {
            var patchData = new { firstname = newFirstname, lastname = newLastname };
            _context["ApiRequest"] = $"PATCH /booking/{_bookingId}\n{JsonConvert.SerializeObject(patchData)}";
            _response = await _bookingClient.PartialUpdateBookingAsync(_bookingId, patchData, _token!);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I patch the booking with (no auth|invalid token)$")]
        public async Task WhenIPatchBookingWith(string authType)
        {
            var patchData = new { firstname = "Updated", lastname = "Name" };
            _context["ApiRequest"] = $"PATCH /booking/{_bookingId}\n{JsonConvert.SerializeObject(patchData)}\nAuth: {authType}";
            var token = authType == "invalid token" ? "invalid_token_123" : string.Empty;
            _response = await _bookingClient.PartialUpdateBookingAsync(_bookingId, patchData, token);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the response status code for partialupdate call should be 200")]
        public void ThenStatusCodeShouldBe200()
        {
            Assert.That(_response!.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the updated booking should contain ""(.*)"" and ""(.*)""")]
        public void ThenUpdatedBookingContains(string expectedFirst, string expectedLast)
        {
            var json = JObject.Parse(_response!.Content!);

            Assert.That(json["firstname"]!.ToString(), Is.EqualTo(expectedFirst));
            Assert.That(json["lastname"]!.ToString(), Is.EqualTo(expectedLast));
        }
    }
}
