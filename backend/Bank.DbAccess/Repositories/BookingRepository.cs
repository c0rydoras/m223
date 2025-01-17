using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    public void AddBooking(int sourceId, int destinationId, decimal amount)
    {
        var newBooking = new Booking();
        newBooking.Amount = amount;
        newBooking.SourceId = sourceId;
        newBooking.DestinationId = destinationId;
        context.Bookings.Add(newBooking);
        context.SaveChanges();
    }

    public IEnumerable<Booking> GetAllBookings()
    {
        return context.Bookings;
    }


    public IEnumerable<Booking> GetBookingsForLedger(int id)
    {
        return context.Bookings.Include(b => b.Source).Include(b => b.Destination).Where(b => b.SourceId == id || b.DestinationId == id);
    }
}