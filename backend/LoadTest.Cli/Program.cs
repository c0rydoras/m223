using System.Net.Http.Json;
using System.Text.Json;
using Bank.Core.Models;
using Bank.Web.Dto;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace LoadTest.Cli
{
    class Program
    {
        private static HttpClient client = new();
        private static string BASE_URL = "http://localhost:5000/api/v1";

        //Prüft die Transaktionssicherheit
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            var random = new Random();

            string jwt = await Login("admin", "adminpass");
            var ledgers = await getAllLedgers(jwt);
            var totalMoneyStart = ledgers.Sum(l => l.Balance);
            foreach (var ledger in ledgers)
            {
                Console.WriteLine(ledger.Name);
            }

            var scenario = Scenario
                .Create(
                    "http_scenario",
                    async context =>
                    {
                        var response = await makeBooking(
                            ledgers.ElementAt(random.Next(0, ledgers.Count)).Id,
                            ledgers.ElementAt(random.Next(0, ledgers.Count)).Id,
                            0.1m,
                            jwt
                        );

                        return response;
                    }
                )
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(
                        rate: 100,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30)
                    )
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("fetch_users_report")
                .WithReportFolder("fetch_users_reports")
                .WithReportFormats(ReportFormat.Html)
                .Run();

            ledgers = await getAllLedgers(jwt);
            var totalMoneyEnd = ledgers.Sum(l => l.Balance);
            var moneyDifference = totalMoneyStart - totalMoneyEnd;

            Console.WriteLine(
                $"At the start the total money was: {totalMoneyStart}. After the test the total money was: {totalMoneyEnd}"
            );
            Console.WriteLine($"The difference is: {moneyDifference}");
            if (moneyDifference == 0)
            {
                Console.WriteLine("Starting and ending money okay!");
            }
            else
            {
                Console.WriteLine("The total amount of money changed :((");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task<string> Login(string user, string password)
        {
            var map = new LoginDTO();
            map.Username = user;
            map.Password = password;

            var response = await client.PostAsJsonAsync($"{BASE_URL}/Login", map);
            if (response.IsSuccessStatusCode)
            {
                var parsedResponse = await response.Content.ReadFromJsonAsync<TokenDTO>();
                return parsedResponse!.Token;
            }

            throw new HttpRequestException(response.ReasonPhrase);
        }

        private static async Task<List<Ledger>> getAllLedgers(string jwt)
        {
            List<Ledger> allLedgers = [];

            var request = new HttpRequestMessage(HttpMethod.Get, $"{BASE_URL}/Ledgers");
            request.Headers.Add("Accept", "text/plain");
            request.Headers.Add("Authorization", $"Bearer {jwt}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var responseJson = await response.Content.ReadAsStringAsync();
            allLedgers = JsonSerializer.Deserialize<List<Ledger>>(responseJson, serializerOptions)!;

            return allLedgers;
        }

        public static Task<Response<HttpResponseMessage>> makeBooking(
            int sourceId,
            int destinationId,
            decimal amount,
            string token
        )
        {
            var dto = new BookingDto();
            dto.SourceId = sourceId;
            dto.DestinationId = destinationId;
            dto.Amount = amount;
            var json = JsonContent.Create(dto);

            var request = Http.CreateRequest("POST", $"{BASE_URL}/bookings")
                .WithHeader("Accept", "application/json")
                .WithHeader("Authorization", $"Bearer {token}")
                .WithBody(json);

            var response = Http.Send(client, request);

            return response;
        }

        private class LoginDTO
        {
            public String Username { get; set; }
            public String Password { get; set; }
        }

        private class TokenDTO
        {
            public String Token { get; set; }
        }
    }
}
