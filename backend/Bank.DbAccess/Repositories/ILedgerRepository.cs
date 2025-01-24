using Bank.Core.Models;

namespace Bank.DbAccess.Repositories;

public interface ILedgerRepository
{
    IEnumerable<Ledger> GetAllLedgers();
    decimal GetTotalMoney();
    Ledger? SelectOne(int id);
    void Update(Ledger ledger);
    void Create(String name);
    void Delete(Ledger ledger);
}
