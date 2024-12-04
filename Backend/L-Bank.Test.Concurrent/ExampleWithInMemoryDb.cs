using L_Bank_W_Backend.Core.Models;
using L_Bank_W_Backend.DbAccess;
using L_Bank_W_Backend.DbAccess.Data;
using L_Bank_W_Backend.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;

namespace L_Bank.Test.Concurrent;

public class ExampleWithInMemoryDb
{
    private readonly Mock<IOptions<DatabaseSettings>> _databaseSettingsMock;
    
    public ExampleWithInMemoryDb()
    {
        _databaseSettingsMock = new Mock<IOptions<DatabaseSettings>>();
        _databaseSettingsMock.Setup(x => x.Value).Returns(new DatabaseSettings
        {
            ConnectionString = "Server=localhost,1433;Database=l_bank_backend;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
        });
    }

    [Fact]
    public async Task TestLedgerRepository()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .ConfigureWarnings(x => 
                x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        context.Ledgers.Add(new Ledger { Name = "Test Ledger" });
        await context.SaveChangesAsync();
        var ledgerRepository = new LedgerRepository(_databaseSettingsMock.Object, context);
        
        // Act
        var ledgers = await ledgerRepository.GetAllLedgers();
        
        // Assert
        Assert.NotNull(ledgers);
        Assert.Single(ledgers);
    }
}