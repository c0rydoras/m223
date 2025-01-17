using Bank.Core.Models;

namespace Bank.DbAccess.Repositories;

public interface IBookingRepository
{
    IEnumerable<Booking> GetAllBookings();
    IEnumerable<Booking> GetBookingsForLedger(int id);
    void AddBooking(int sourceId, int destinationId, Decimal amount);
}