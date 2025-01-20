using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Bank.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SingleUserTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ITestDatabaseSeeder, TestDatabaseSeeder>();
        services.AddTransient<ILedgerRepository, LedgerRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IBookingRepository, BookingRepository>();
        services.AddTransient<IBookingService, BookingService>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(
                "Filename=test",
                sqlOptions => sqlOptions.MigrationsAssembly("Bank.Web")
            )
        );
    }
}
