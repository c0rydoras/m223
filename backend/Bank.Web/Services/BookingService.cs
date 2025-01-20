using System.Data;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Data.IsolationLevel;

namespace Bank.Web.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    ILedgerRepository ledgerRepository,
    AppDbContext context
) : IBookingService
{
    public void Book(int sourceId, int destinationId, decimal amount, int retryCounter = 1)
    {
        if (amount <= 0)
        {
            throw new ConstraintException("amount must be greater then 0");
        }

        using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);

        var sourceLedger = ledgerRepository.SelectOne(sourceId);
        if (amount > sourceLedger.Balance)
        {
            throw new ConstraintException(
                $"amount must be smaller then or equal source balance: ({sourceLedger.Balance})"
            );
        }

        var destinationLedger = ledgerRepository.SelectOne(destinationId);

        sourceLedger.Balance -= amount;
        destinationLedger.Balance += amount;

        try
        {
            ledgerRepository.Update(sourceLedger);
            ledgerRepository.Update(destinationLedger);
            bookingRepository.AddBooking(sourceId, destinationId, amount);
            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Console.WriteLine(e);
            Console.WriteLine($"Retry no. {retryCounter}");
            if (retryCounter <= 40)
            {
                retryCounter++;
                Thread.Sleep(1000);
                Book(sourceId, destinationId, amount, retryCounter);
                return;
            }
            Console.WriteLine("Aborting");
            throw;
        }
    }
}
