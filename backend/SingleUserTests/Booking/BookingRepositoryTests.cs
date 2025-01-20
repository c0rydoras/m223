using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;

namespace SingleUserTests.Booking;

[Collection("test")]
public class BookingRepositoryTests
{
    private readonly IBookingRepository _bookingRepository;

    public BookingRepositoryTests(
        ITestDatabaseSeeder testDatabaseSeeder,
        IBookingRepository bookingRepository
    )
    {
        _bookingRepository = bookingRepository;
        testDatabaseSeeder.Initialize();
        testDatabaseSeeder.Seed();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetBookingsForLedger_LedgerExists(int ledgerId)
    {
        var allBookings = _bookingRepository.GetAllBookings().ToList();
        var bookingsFromLedger = _bookingRepository.GetBookingsForLedger(ledgerId);

        Assert.Equivalent(
            allBookings.Where(b => b.SourceId == ledgerId || b.DestinationId == ledgerId),
            bookingsFromLedger
        );
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(99)]
    [InlineData(39)]
    public void GetBookingsForLedger_LedgerNotFound(int ledgerId)
    {
        var list = _bookingRepository.GetBookingsForLedger(ledgerId);
        Assert.Empty(list);
    }
}
