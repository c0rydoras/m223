using System.Data;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository(AppDbContext context) : ILedgerRepository
{
    public string Book(decimal amount, Ledger from, Ledger to)
    {
        if (from.Id == to.Id)
        {
            return "0";
        }
        try
        {
            using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);
            context.Entry(from).Reload();
            from.Balance -= amount;
            Thread.Sleep(250);
            context.Entry(to).Reload();
            to.Balance += amount;
            context.SaveChanges();
            transaction.Commit();
            return ".";
        }
        catch (Exception ex)
        {
            return "R";
        }
    }

    public decimal GetTotalMoney()
    {
        using var transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        return context.Ledgers.Sum(l => l.Balance);
    }


    public IEnumerable<Ledger> GetAllLedgers()
    {
        using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);
        return context.Ledgers;
    }

    public Ledger SelectOne(int id)
    {
        return context.Ledgers.First(l => l.Id == id);
    }

    public void Update(Ledger ledger)
    {
        context.Update(ledger);
        context.SaveChanges();
    }
}