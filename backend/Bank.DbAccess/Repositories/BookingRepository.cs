using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.Extensions.Options;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(IOptions<DatabaseSettings> settings, AppDbContext context) : IBookingRepository
{
    private DatabaseSettings _settings = settings.Value;

    public void AddBooking(int sourceId, int destinationId, decimal amount)
    {
        var newBooking = new Booking();
        newBooking.Amount = amount;
        newBooking.SourceId = sourceId;
        newBooking.DestinationId = destinationId;
        context.Bookings.Add(newBooking);
        context.SaveChanges();
    }
}