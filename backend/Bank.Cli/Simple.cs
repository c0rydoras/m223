using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class Simple
{
    public static void Run(ILedgerRepository ledgerRepository)
    {
        ////////////////////
        // Your Code Here
        ////////////////////

        var LegerList = ledgerRepository.GetAllLedgers().ToArray();

        Console.WriteLine("Booking, press ESC to stop.");
        while (true)
        {
            var random = new Random();
            var firstLeger = LegerList.ElementAt(random.Next(0, LegerList.Length));
            var secondLeger = LegerList.ElementAt(random.Next(0, LegerList.Length));
            ledgerRepository.Book(random.Next(0, 101), firstLeger, secondLeger);
            Console.Write(".");
            if (Console.KeyAvailable && Console.ReadKey(false).Key == ConsoleKey.Escape)
            {
                break;
            }
        }

        Console.WriteLine();
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

        Console.WriteLine("Hello, World!");
    }
}
