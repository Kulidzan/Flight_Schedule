using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using ZeKju.Bll.Interface;

namespace ZeKju_BK
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter start date (yyyy-mm-dd): ");
            string startDateInput = Console.ReadLine();
            if (!DateTime.TryParse(startDateInput, out DateTime startDate))
            {
                Console.WriteLine("Invalid start date format.");
                return;
            }

            Console.Write("Enter end date (yyyy-mm-dd): ");
            string endDateInput = Console.ReadLine();
            if (!DateTime.TryParse(endDateInput, out DateTime endDate))
            {
                Console.WriteLine("Invalid end date format.");
                return;
            }

            Console.Write("Enter agency ID: ");
            string agencyIdInput = Console.ReadLine();
            if (!int.TryParse(agencyIdInput, out int agencyId))
            {
                Console.WriteLine("Invalid agency ID format.");
                return;
            }

            var host = CreateHostBuilder(args).Build();
            var detector = host.Services.GetRequiredService<IFlightScheduleChangeDetector>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var changes = await detector.DetectChanges(startDate, endDate, agencyId).ConfigureAwait(false);
            stopwatch.Stop();

            using (var writer = new StreamWriter("results.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(changes);
            }
            Console.WriteLine($"Results written to results.csv.");
            Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalSeconds} seconds.");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    Startup.ConfigureServices(services, context.Configuration);
                });
    }
}
