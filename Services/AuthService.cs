using RestSharp;
using Newtonsoft.Json.Linq;

namespace BookingAPI.Services
{
    public class AuthService
    {
        private readonly RestClient _client;

        public AuthService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<string> GetToken(string username = "admin", string password = "password123")
        {
            var request = new RestRequest("/auth", Method.Post);
            request.AddJsonBody(new { username, password });

            var response = await _client.ExecuteAsync(request);
            
            if (!response.IsSuccessful)
                throw new Exception($"Auth failed: {response.StatusCode}");

            var json = JObject.Parse(response.Content!);
            return json["token"]!.ToString();
        }
    }
}
