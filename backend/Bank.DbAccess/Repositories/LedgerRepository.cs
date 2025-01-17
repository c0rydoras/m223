using System.Data;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository(AppDbContext context)
    : ILedgerRepository
{

    public string Book(decimal amount, Ledger from, Ledger to)
    {
        if (from.Id == to.Id)
        {
            return "0";
        }
        using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);
        {

            try
            {

                amount = 10;
                var localFrom = context.Ledgers.Find(from.Id);
                var localTo = context.Ledgers.Find(to.Id);
                //context.Ledgers.Entry(from).Reload();
                //context.Ledgers.Entry(to).Reload();
                localFrom.Balance -= amount;
                // Complicate calculations
                Thread.Sleep(250);
                localTo.Balance += amount;
                context.SaveChanges();

                // Console.WriteLine($"Booking {amount} from {from.Name} to {to.Name}");

                transaction.Commit();
                return ".";
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                //Console.WriteLine("  Message: {0}", ex.Message);

                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                    //Console.WriteLine(ex);
                    return "R";
                }
                catch (Exception ex2)
                {
                    // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                    //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    //Console.WriteLine("  Message: {0}", ex2.Message);
                    return "E";
                }
            }
        }
    }

    public IEnumerable<Ledger> GetAllLedgers()
    {
        return context.Ledgers;
    }

    public decimal GetTotalMoney()
    {
        using var transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        return context.Ledgers.Sum(x => x.Balance);
    }


    public Ledger? SelectOne(int id)
    {
        return context.Ledgers.FirstOrDefault(l => l.Id == id);
    }

    public void Update(Ledger ledger)
    {
        context.Update(ledger);
        context.SaveChanges();
    }

    public void Create(String name)
    {
        var ledger = new Ledger{Name = name, Balance = 0};
        context.Add(ledger);
        context.SaveChanges();
    }

    public void Delete(Ledger ledger)
    {
        context.Ledgers.Remove(ledger);
        context.SaveChanges();
    }
}