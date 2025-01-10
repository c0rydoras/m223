using Bank.Core.Models;
using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class Simple
{
    public static void Run(ILedgerRepository ledgerRepository)
    {
        ////////////////////
        // Your Code Here
        ////////////////////
        
        Console.WriteLine("Booking, press ESC to stop");

        var allLedgers = ledgerRepository.GetAllLedgers().ToArray();
        
        while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
        {
            var random = new Random();
            var from = allLedgers[random.Next(0, allLedgers.Length)];
            var to = allLedgers[random.Next(0, allLedgers.Length)];
            var amount = random.Next(0, 101);
            ledgerRepository.Book(amount, from, to);
            Console.Write(".");
        }
        
        Console.WriteLine("Getting total money in system at the end.");
        try
        {
            var startMoney = ledgerRepository.GetTotalMoney();
            Console.WriteLine($"Total end money: {startMoney}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in getting total money.");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine();
    }
}