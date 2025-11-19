using RestSharp;
using BookingAPI.Models;
using Newtonsoft.Json;

namespace BookingAPI.Services
{
    public class BookingApiClient : IBookingApiClient
    {
        private readonly string _baseUrl;

        public BookingApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<RestResponse> CreateBookingAsync(BookingRequest request)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest { Method = Method.Post };
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddStringBody(JsonConvert.SerializeObject(request), DataFormat.Json);
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> GetBookingAsync(int bookingId)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest($"/{bookingId}", Method.Get);
            restRequest.AddHeader("Accept", "application/json");
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> GetBookingIdsAsync()
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest("", Method.Get);
            restRequest.AddHeader("Accept", "application/json");
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> GetBookingIdsAsync(string? firstname = null, string? lastname = null, string? checkin = null, string? checkout = null)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest("", Method.Get);
            restRequest.AddHeader("Accept", "application/json");
            
            if (!string.IsNullOrEmpty(firstname))
                restRequest.AddParameter("firstname", firstname);
            if (!string.IsNullOrEmpty(lastname))
                restRequest.AddParameter("lastname", lastname);
            if (!string.IsNullOrEmpty(checkin))
                restRequest.AddParameter("checkin", checkin);
            if (!string.IsNullOrEmpty(checkout))
                restRequest.AddParameter("checkout", checkout);
            
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> UpdateBookingAsync(int bookingId, BookingRequest request, string token)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest($"/{bookingId}", Method.Put);
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Cookie", $"token={token}");
            restRequest.AddStringBody(JsonConvert.SerializeObject(request), DataFormat.Json);
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> PartialUpdateBookingAsync(int bookingId, object partialData, string token)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest($"/{bookingId}", Method.Patch);
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Cookie", $"token={token}");
            restRequest.AddStringBody(JsonConvert.SerializeObject(partialData), DataFormat.Json);
            return await client.ExecuteAsync(restRequest);
        }

        public async Task<RestResponse> DeleteBookingAsync(int bookingId, string token)
        {
            var client = new RestClient(_baseUrl);
            var restRequest = new RestRequest($"/{bookingId}", Method.Delete);
            restRequest.AddHeader("Cookie", $"token={token}");
            return await client.ExecuteAsync(restRequest);
        }
    }
}
