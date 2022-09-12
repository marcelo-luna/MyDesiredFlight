using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDesiredFlight.Bll.AirLines;
using MyDesiredFlight.Bll.Interface;

[assembly: FunctionsStartup(typeof(MyDesiredFlight.HtmlFunction.Startup))]
namespace MyDesiredFlight.HtmlFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);
            builder.Services.AddLogging();

            builder.Services.AddSingleton<ISearchFly, Latam>();
            builder.Services.AddSingleton<ISearchFly, Ita>();

            //builder.Services.AddSingleton(new Function1(builder.Services.BuildServiceProvider().GetRequiredService<ISearchFly>()));
        }

        private IConfiguration BuildConfiguration(string applicationRootPath)
        {
            var config =
                new ConfigurationBuilder()
                    .SetBasePath(applicationRootPath)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            return config;
        }
    }
}
