using Bank.Core.Models;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SingleUserTests.Booking;

[Collection("test")]
public class LedgerRepositoryTest
{
    private readonly AppDbContext _context;
    private readonly ILedgerRepository _ledgerRepository;
    private readonly IBookingRepository _bookingRepository;

    public LedgerRepositoryTest(
        ILedgerRepository ledgerRepository,
        ITestDatabaseSeeder testDatabaseSeeder,
        AppDbContext context,
        IBookingRepository bookingRepository
    )
    {
        _context = context;
        _ledgerRepository = ledgerRepository;
        _bookingRepository = bookingRepository;
        testDatabaseSeeder.Initialize();
        testDatabaseSeeder.Seed();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Delete_ShouldDeleteLedgerAndSetReferenceInBookingsToNull(int ledgerId)
    {
        var ledger = _ledgerRepository.SelectOne(ledgerId);
        var ledgerBookings = _bookingRepository.GetBookingsForLedger(ledgerId);
            
        Assert.IsType<Ledger>(ledger);
        
        _ledgerRepository.Delete(ledger);

        Assert.Null(_ledgerRepository.SelectOne(ledgerId));
        
        foreach (var ledgerBooking in ledgerBookings)
        {
            _context.Bookings.Entry(ledgerBooking).Reload();
            Assert.True(ledgerBooking.SourceId == null || ledgerBooking.DestinationId == null);
        }
    }

    [Theory]
    [InlineData(99)]
    [InlineData(39)]
    public void Delete_LedgerNotFound(int ledgerId)
    {
        Assert.Throws<DbUpdateConcurrencyException>(()=>_ledgerRepository.Delete(new Ledger { Id = ledgerId }));
    }

    [Fact]
    public void Delete_LedgerNull()
    {
        Assert.Throws<ArgumentNullException>(()=>_ledgerRepository.Delete(null));
    }
}
