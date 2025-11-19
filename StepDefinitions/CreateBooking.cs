using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using BookingAPI.Services;
using BookingAPI.Models;
using BookingAPI.Helpers;

namespace BookingAPI.StepDefinitions
{
    [Binding, Scope(Feature = "Create Booking")]
    public class CreateBookingSteps
    {
        private RestResponse? _response;
        private BookingRequest? _bookingRequest;
        private readonly ScenarioContext _context;
        private readonly IBookingApiClient _bookingClient;

        public CreateBookingSteps(ScenarioContext context)
        {
            _context = context;
            _bookingClient = new BookingApiClient($"{Config.BaseUrl}/booking");
        }

        [Given(@"I have booking details ""(.*)"", ""(.*)"", (.*), (.*), ""(.*)"", ""(.*)"", ""(.*)""")]
        public void GivenIHaveBookingDetails(string firstname, string lastname, int totalprice, 
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

            _context["ApiRequest"] = $"POST /booking\n{JsonConvert.SerializeObject(_bookingRequest)}";
        }

        [When(@"I submit the booking request")]
        public async Task WhenISubmitTheBookingRequest()
        {
            _response = await _bookingClient.CreateBookingAsync(_bookingRequest!);
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the response status code for create request should be 200")]
        public void ThenTheResponseStatusCodeShouldBe200()
        {
            Assert.That(_response?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the response should include the booking with firstname ""(.*)"" and lastname ""(.*)""")]
        public void ThenTheResponseShouldIncludeBooking(string expectedFirstname, string expectedLastname)
        {
            dynamic? responseBody = JsonConvert.DeserializeObject(_response!.Content!);

            string? actualFirstname = responseBody?.booking?.firstname;
            string? actualLastname = responseBody?.booking?.lastname;

            Assert.That(actualFirstname, Is.EqualTo(expectedFirstname));
            Assert.That(actualLastname, Is.EqualTo(expectedLastname));
            Assert.That((int)responseBody?.booking?.totalprice, Is.GreaterThan(0));
        }
    }
}
