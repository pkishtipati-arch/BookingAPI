using Microsoft.Extensions.Configuration;

namespace BookingAPI.Helpers
{
    public static class Config
    {
        private static IConfiguration? _config;

        private static IConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    _config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true)
                        .Build();
                }
                return _config;
            }
        }

        public static string BaseUrl => Configuration["ApiSettings:BaseUrl"] ?? "https://restful-booker.herokuapp.com";
        public static string Username => Configuration["Auth:Username"] ?? "admin";
        public static string Password => Configuration["Auth:Password"] ?? "password123";
    }
}
