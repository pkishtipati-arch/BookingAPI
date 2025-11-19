namespace BookingAPI.Models
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public int Totalprice { get; set; }
        public bool Depositpaid { get; set; }
        public BookingDates? Bookingdates { get; set; }
        public string? Additionalneeds { get; set; }
    }
}
