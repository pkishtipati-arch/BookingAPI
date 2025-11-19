using Newtonsoft.Json;

namespace BookingAPI.Models
{
    public class BookingRequest
    {
        [JsonProperty("firstname")]
        public string? Firstname { get; set; }
        
        [JsonProperty("lastname")]
        public string? Lastname { get; set; }
        
        [JsonProperty("totalprice")]
        public int TotalPrice { get; set; }
        
        [JsonProperty("depositpaid")]
        public bool DepositPaid { get; set; }
        
        [JsonProperty("bookingdates")]
        public BookingDates? BookingDates { get; set; }
        
        [JsonProperty("additionalneeds")]
        public string? AdditionalNeeds { get; set; }
    }

    public class BookingDates
    {
        [JsonProperty("checkin")]
        public string? CheckIn { get; set; }
        
        [JsonProperty("checkout")]
        public string? CheckOut { get; set; }
    }
}
