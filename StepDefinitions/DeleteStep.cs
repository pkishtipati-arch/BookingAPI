using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using BookingAPI.Services;
using BookingAPI.Helpers;

namespace BookingAPI.StepDefinitions
{
    [Binding]
    public class DeleteBookingSteps
    {
        private RestResponse? _response;
        private readonly ScenarioContext _context;
        private readonly IBookingApiClient _bookingClient;

        public DeleteBookingSteps(ScenarioContext context)
        {
            _context = context;
            _bookingClient = new BookingApiClient($"{Config.BaseUrl}/booking");
        }

        [When(@"I delete the booking")]
        public async Task WhenIDeleteBooking()
        {
            var bookingId = int.Parse(_context["BookingId"].ToString()!);
            var token = _context["AuthToken"].ToString()!;
            
            _context["ApiRequest"] = $"DELETE /booking/{bookingId}";
            _response = await _bookingClient.DeleteBookingAsync(bookingId, token);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I delete the booking without auth")]
        public async Task WhenIDeleteBookingWithoutAuth()
        {
            var bookingId = int.Parse(_context["BookingId"].ToString()!);
            _context["ApiRequest"] = $"DELETE /booking/{bookingId}\nAuth: none";
            _response = await _bookingClient.DeleteBookingAsync(bookingId, string.Empty);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the response status code for delete request should be 201")]
        public void ThenStatusCodeShouldBe201()
        {
            Assert.That(_response!.StatusCode, Is.EqualTo((HttpStatusCode)201));
        }

        [Then(@"the response body should be ""(.*)""")]
        public void ThenResponseBodyShouldBe(string expectedBody)
        {
            var response = _context.Get<RestResponse>("Response");
            Assert.That(response.Content, Is.EqualTo(expectedBody));
        }

        [Then(@"getting the deleted booking should return 404")]
        public async Task ThenDeletedBookingReturns404()
        {
            var bookingId = int.Parse(_context["BookingId"].ToString()!);
            var response = await _bookingClient.GetBookingAsync(bookingId);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
