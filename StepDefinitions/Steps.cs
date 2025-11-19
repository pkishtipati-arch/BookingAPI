using Newtonsoft.Json;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using System.Net;
using BookingAPI.Models;
using BookingAPI.Services;
using BookingAPI.Helpers;

namespace BookingAPI.StepDefinitions
{
    [Binding]
    public class CommonSteps
    {
        private RestResponse? _response;
        private string? _bookingId;
        private readonly ScenarioContext _context;
        private readonly IBookingApiClient _bookingClient;
        private readonly AuthService _authService;

        public CommonSteps(ScenarioContext context)
        {
            _context = context;
            _bookingClient = new BookingApiClient($"{Config.BaseUrl}/booking");
            _authService = new AuthService(Config.BaseUrl);
        }

        [Given(@"I have a valid auth token")]
        public async Task GivenIHaveValidAuthToken()
        {
            var token = await _authService.GetToken();
            _context["AuthToken"] = token;
        }

        [Given(@"I have a valid booking ID")]
        public async Task GivenIHaveValidBookingId()
        {
            var response = await _bookingClient.GetBookingIdsAsync();
            var ids = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(response.Content!);
            _bookingId = ids![0]["bookingid"].ToString();
            _context["BookingId"] = _bookingId;
        }

        [Given(@"I have an invalid booking ID ""(.*)""")]
        public void GivenIHaveInvalidBookingId(string invalidId)
        {
            _bookingId = invalidId;
        }

        [Given(@"I have a non-existent booking ID (.*)")]
        public void GivenIHaveNonExistentBookingId(int id)
        {
            _bookingId = id.ToString();
            _context["BookingId"] = _bookingId;
        }

        [When(@"I request the booking details")]
        public async Task WhenIRequestBookingDetails()
        {
            _context["ApiRequest"] = $"GET /booking/{_bookingId}";
            _response = await _bookingClient.GetBookingAsync(int.Parse(_bookingId!));
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I request the booking details with invalid ID")]
        public async Task WhenIRequestBookingDetailsWithInvalidId()
        {
            _context["ApiRequest"] = $"GET /booking/{_bookingId}";
            
            if (int.TryParse(_bookingId, out int bookingId))
            {
                _response = await _bookingClient.GetBookingAsync(bookingId);
            }
            else
            {
                var client = new RestClient(Config.BaseUrl);
                var request = new RestRequest($"/booking/{_bookingId}", Method.Get);
                request.AddHeader("Accept", "application/json");
                _response = await client.ExecuteAsync(request);
            }
            
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I request all booking IDs")]
        public async Task WhenIRequestAllBookingIds()
        {
            _context["ApiRequest"] = "GET /booking";
            _response = await _bookingClient.GetBookingIdsAsync();
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I request booking IDs filtered by firstname ""(.*)"" and lastname ""(.*)""")]
        public async Task WhenIRequestFilteredBookingIds(string firstname, string lastname)
        {
            _context["ApiRequest"] = $"GET /booking?firstname={firstname}&lastname={lastname}";
            _response = await _bookingClient.GetBookingIdsAsync(firstname, lastname);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [When(@"I request booking IDs with invalid checkin date ""(.*)""")]
        public async Task WhenIRequestBookingIdsWithInvalidDate(string invalidDate)
        {
            _context["ApiRequest"] = $"GET /booking?checkin={invalidDate}";
            _response = await _bookingClient.GetBookingIdsAsync(checkin: invalidDate);
            _context["Response"] = _response;
            _context["ApiResponse"] = $"Status: {(int)_response.StatusCode}, Body: {_response.Content}";
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenStatusCodeShouldBe(int expectedCode)
        {
            var response = _context.ContainsKey("Response") 
                ? _context.Get<RestResponse>("Response") 
                : _response!;
            
            Assert.That((int)response.StatusCode, Is.EqualTo(expectedCode));
        }

        [Then(@"the response should contain all required fields")]
        public void ThenResponseShouldContainAllRequiredFields(Table table)
        {
            var booking = JsonConvert.DeserializeObject<BookingResponse>(_response!.Content!);
            Assert.That(booking, Is.Not.Null);

            foreach (var row in table.Rows)
            {
                var field = row["field"];
                switch (field)
                {
                    case "firstname":
                        Assert.That(booking!.Firstname, Is.Not.Empty, "Firstname is missing");
                        break;
                    case "lastname":
                        Assert.That(booking!.Lastname, Is.Not.Empty, "Lastname is missing");
                        break;
                    case "totalprice":
                        Assert.That(booking!.Totalprice, Is.GreaterThan(0), "Totalprice is invalid");
                        break;
                    case "depositpaid":
                        Assert.That(booking!.Depositpaid, Is.Not.Null, "Depositpaid is missing");
                        break;
                    case "bookingdates":
                        Assert.That(booking!.Bookingdates, Is.Not.Null, "Bookingdates is missing");
                        Assert.That(booking.Bookingdates!.CheckIn, Is.Not.Empty, "CheckIn is missing");
                        Assert.That(booking.Bookingdates.CheckOut, Is.Not.Empty, "CheckOut is missing");
                        break;
                }
            }
        }

        [Then(@"the response should contain a list of booking IDs")]
        public void ThenResponseShouldContainBookingIds()
        {
            var ids = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(_response!.Content!);
            Assert.That(ids, Is.Not.Null);
            Assert.That(ids!.Count, Is.GreaterThan(0));
        }

        [Then(@"the response should contain filtered booking IDs")]
        public void ThenResponseShouldContainFilteredIds()
        {
            var ids = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(_response!.Content!);
            Assert.That(ids, Is.Not.Null);
        }
    }
}
