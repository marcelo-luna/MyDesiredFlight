using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyDesiredFlight.AirLines;
using MyDesiredFlight.Interfaces;
using System.Threading.Tasks;

namespace MyDesiredFlight
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ISearchFly, Latam>()
                .BuildServiceProvider();

            //configure console logging
            //serviceProvider
            //    .GetService<ILoggerFactory>()
            //    .AddConsole(LogLevel.Debug);

            //var logger = serviceProvider.GetService<ILoggerFactory>()
            //    .CreateLogger<Program>();
            //logger.LogDebug("Starting application");

            //do the actual work here
            var bar = serviceProvider.GetService<ISearchFly>();
            await bar.SearchFly("MXP", "GRU", "2022-12-23", "2023-01-21");

            //logger.LogDebug("All done!");
        }

 
    }
}
