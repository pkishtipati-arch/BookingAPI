using RestSharp;
using System.Threading.Tasks;
using BookingAPI.Models;

namespace BookingAPI.Services
{
    public interface IBookingApiClient
    {
        Task<RestResponse> CreateBookingAsync(BookingRequest request);
        Task<RestResponse> GetBookingAsync(int bookingId);
        Task<RestResponse> GetBookingIdsAsync();
        Task<RestResponse> GetBookingIdsAsync(string? firstname = null, string? lastname = null, string? checkin = null, string? checkout = null);
        Task<RestResponse> UpdateBookingAsync(int bookingId, BookingRequest request, string token);
        Task<RestResponse> PartialUpdateBookingAsync(int bookingId, object partialData, string token);
        Task<RestResponse> DeleteBookingAsync(int bookingId, string token);
    }
}