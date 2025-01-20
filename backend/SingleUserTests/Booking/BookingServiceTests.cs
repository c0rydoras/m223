using System.Data;
using Bank.DbAccess.Data;
using Bank.Web.Services;

namespace SingleUserTests.Booking
{
    [Collection("test")]
    public class BookingServiceTests
    {
        private readonly IBookingService _bookingService;
        private readonly AppDbContext _context;

        public BookingServiceTests(
            IBookingService bookingService,
            ITestDatabaseSeeder testDatabaseSeeder,
            AppDbContext context
        )
        {
            _context = context;
            _bookingService = bookingService;
            testDatabaseSeeder.Initialize();
            testDatabaseSeeder.Seed();
        }

        [Theory]
        [InlineData(1, 2, 100)]
        [InlineData(1, 3, 25)]
        [InlineData(3, 1, 1)]
        [InlineData(1, 3, 35.233434)]
        [InlineData(3, 1, 1000)]
        public void Book_SuccessfulBooking(int sourceId, int destinationId, decimal amount)
        {
            var exception = Record.Exception(
                () => _bookingService.Book(sourceId, destinationId, amount)
            );
            Assert.Null(exception);
            Assert.Equal(1000 - amount, _context.Ledgers.Find(sourceId)!.Balance);
            Assert.Equal(1000 + amount, _context.Ledgers.Find(destinationId)!.Balance);
        }

        [Theory]
        [InlineData(2, 1, -10)]
        [InlineData(3, 1, -9999999999999999)]
        [InlineData(1, 2, 0)]
        public void Book_AmountLessThan0_ShouldThrowException(
            int sourceId,
            int destinationId,
            decimal amount
        )
        {
            var exception = Record.Exception(
                () => _bookingService.Book(sourceId, destinationId, amount)
            );
            Assert.Equal("amount must be greater then 0", exception.Message);
            Assert.IsType<ConstraintException>(exception);
            Assert.Equal(1000, _context.Ledgers.Find(sourceId)!.Balance);
            Assert.Equal(1000, _context.Ledgers.Find(destinationId)!.Balance);
        }

        [Theory]
        [InlineData(1, 2, 1001)]
        [InlineData(2, 1, 909990909909)]
        [InlineData(1, 3, 1000.00000001)]
        public void Book_AmountMoreThanSource(int sourceId, int destinationId, decimal amount)
        {
            var exception = Record.Exception(
                () => _bookingService.Book(sourceId, destinationId, amount)
            );
            Assert.Equal(
                $"amount must be smaller then or equal source balance: ({1000})",
                exception.Message
            );
            Assert.IsType<ConstraintException>(exception);
            Assert.Equal(1000, _context.Ledgers.Find(sourceId)!.Balance);
            Assert.Equal(1000, _context.Ledgers.Find(destinationId)!.Balance);
        }
    }
}
