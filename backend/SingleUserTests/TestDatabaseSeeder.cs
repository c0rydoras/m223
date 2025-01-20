using Bank.Core.Helper;
using Bank.Core.Models;
using Bank.DbAccess.Data;

namespace SingleUserTests;

public class TestDatabaseSeeder(AppDbContext context) : ITestDatabaseSeeder
{
    private void SeedLedgers()
    {
        var seedLedgers = new List<Ledger>
        {
            new() { Name = "Manitu AG", Balance = 1000 },
            new() { Name = "Chrysalkis GmbH", Balance = 1000 },
            new() { Name = "Smith & Co KG", Balance = 1000 },
        };

        context.Ledgers.AddRange(seedLedgers);
        context.SaveChanges();
    }

    private void SeedUsers()
    {
        var seedUsers = new List<User>
        {
            new()
            {
                Username = "admin",
                PasswordHash = PasswordHelper.HashAndSaltPassword("adminpass"),
                Role = Roles.Administrators,
            },
            new()
            {
                Username = "testuser",
                PasswordHash = PasswordHelper.HashAndSaltPassword("testuserpass"),
                Role = Roles.Users,
            },
        };

        context.Users.AddRange(seedUsers);
        context.SaveChanges();
    }

    public void Initialize()
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public void Seed()
    {
        SeedLedgers();
        SeedUsers();
    }
}
