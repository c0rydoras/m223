using System.Data;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository(AppDbContext context) : ILedgerRepository
{
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
    }

    public void Create(String name)
    {
        var ledger = new Ledger { Name = name, Balance = 0 };
        context.Add(ledger);
        context.SaveChanges();
    }

    public void Delete(Ledger ledger)
    {
        context.Ledgers.Remove(ledger);
        context.SaveChanges();
    }
}
