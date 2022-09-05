using Microsoft.Extensions.Logging;
using MyDesiredFlight.Bll.Interface;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyDesiredFlight.Bll.AirLines
{
    public class Latam : ISearchFly
    {
        private readonly string BASE_URL;
        private readonly ILogger<Latam> _logger;

        public Latam(IConfiguration configuration, ILogger<Latam> logger)
        {
            BASE_URL = configuration.GetSection("LATAM_URL").Value;
            _logger = logger;
        }

        public async Task<decimal> SearchFly(string origin, string destination, string dateFrom, string dateTo, string[] options)
        {
            using (var driver = new ChromeDriver())
            {
                INavigation nav = driver.Navigate();
                //https://www.latamairlines.com/br/pt/oferta-voos?origin={origin}&inbound={dateTo}T10%3A00%3A00.000Z&outbound={dateFrom}T10%3A00%3A00.000Z&destination={destination}&adt=1&chd=0&inf=0&trip=RT&cabin=Economy&redemption=false&sort=RECOMMENDED""
                nav.GoToUrl(BASE_URL.Replace("{origin}", origin).Replace("{dateTo}", dateTo).Replace("{dateFrom}",dateFrom).Replace("{destination}",destination));

                _logger.LogInformation("Waiting initial page");
                await Task.Delay(40000);

                _logger.LogInformation("Select country");
                var div = driver.FindElement(By.Id("country-suggestion-body-reject-change"));

                div.Click();

                _logger.LogInformation("Select cookies policy");
                div = driver.FindElement(By.Id("cookies-politics-button"));

                div.Click();

                _logger.LogInformation("Select first flight");
                var priceToGo = decimal.Parse(await SelectFirstFlightRow(div, driver));

                _logger.LogInformation("Select second flight");
                var priceToTurn = decimal.Parse((await SelectFirstFlightRow(div, driver)));

                var total = priceToGo + priceToTurn;

                Console.WriteLine(total);

                return total;
            }
        }

        public async Task<string> SelectFirstFlightRow(IWebElement div, IWebDriver driver)
        {
            _logger.LogInformation("Waiting time");
            await Task.Delay(3000);

            _logger.LogInformation("Select first row flight");
            div = driver.FindElement(By.CssSelector("span[class='duration-text']"));

            div.Click();

            await Task.Delay(2000);

            _logger.LogInformation("Select column low price");
            div = driver.FindElement(By.Id("WrapperBundleCardbundle-detail-00"));

            var allDiv = div.Text.Replace("\r\n", "@").Split('@');

            string price = "";

            foreach (var r in allDiv)
            {
                if (r.IndexOf('.') > 0 && r.IndexOf(',') > 0 && char.IsNumber(r.ToCharArray()[0]))
                {
                    price = r;
                }
            }

            div = div.FindElement(By.CssSelector("button"));

            await Task.Delay(1000);
            
            _logger.LogInformation("Button next");
            div.Click();

            return price.Replace(".", "").Replace(",", ".");
        }
    }
}
