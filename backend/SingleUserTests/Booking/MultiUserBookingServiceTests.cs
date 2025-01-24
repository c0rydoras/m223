using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Bank.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SingleUserTests.Booking
{
    [Collection("test")]
    public class MultiUserBookingServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public MultiUserBookingServiceTests(
            AppDbContext context,
            ITestDatabaseSeeder testDatabaseSeeder,
            IServiceProvider serviceProvider
        )
        {
            _context = context;
            _serviceProvider = serviceProvider;
            testDatabaseSeeder.Initialize();
            testDatabaseSeeder.Seed();
        }

        //Prüft die Transaktionssicherheit
        [Fact]
        public void TestBookingParallel()
        {
            const int numberOfBookings = 1000;
            const int users = 10;
            var ledgers = _context.Ledgers.ToList();

            // Implementieren Sie hier die parallelen Buchungen
            Task[] tasks = new Task[users];

            void UserAction()
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var bookingRepository = new BookingRepository(dbContext);
                var ledgerRepository = new LedgerRepository(dbContext);
                var bookingService = new BookingService(
                    bookingRepository,
                    ledgerRepository,
                    dbContext
                );
                Random random = new Random();
                for (int i = 0; i < numberOfBookings; i++)
                {
                    // Implementieren Sie hier die parallelen Buchungen
                    // Bestimmen sie zwei zufällige Ledgers

                    var ledgerSource = ledgers[random.Next(0, ledgers.Count)];
                    var ledgerDestination = ledgers[random.Next(0, ledgers.Count)];
                    bookingService.Book(ledgerSource.Id, ledgerDestination.Id, 1);
                }
            }

            for (int i = 0; i < users; i++)
            {
                tasks[i] = Task.Run(() => UserAction());
            }

            Task.WaitAll(tasks);
            Assert.Equal(3000, _context.Ledgers.Sum(l => l.Balance));
        }
    }
}
