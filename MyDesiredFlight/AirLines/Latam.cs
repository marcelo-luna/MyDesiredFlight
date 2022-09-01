using MyDesiredFlight.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;

namespace MyDesiredFlight.AirLines
{
    public class Latam : ISearchFly
    {
        readonly string BASE_URL;

        public Latam(string baseUrl)
        {
            BASE_URL = baseUrl;
        }

        public Latam()
        {

        }

        public async Task<decimal> SearchFly(string origin, string destination, string startDate, string endDate, string[] options)
        {
            var driver = new ChromeDriver();

            INavigation nav = driver.Navigate();

            nav.GoToUrl($"https://www.latamairlines.com/br/pt/oferta-voos?origin={origin}&inbound={endDate}T10%3A00%3A00.000Z&outbound={startDate}T10%3A00%3A00.000Z&destination={destination}&adt=1&chd=0&inf=0&trip=RT&cabin=Economy&redemption=false&sort=RECOMMENDED");

            await Task.Delay(30000);

            var div = driver.FindElement(By.Id("country-suggestion-body-reject-change"));

            div.Click();

            div = driver.FindElement(By.Id("cookies-politics-button"));

            div.Click();

            var priceToGo = decimal.Parse(await SelectFirstFlightRow(div, driver));

            await Task.Delay(2000);

            var priceToTurn = decimal.Parse((await SelectFirstFlightRow(div, driver)));

            var total = priceToGo + priceToTurn;

            Console.WriteLine(total);
            Console.ReadLine();

            return total;
        }

        public async Task<string> SelectFirstFlightRow(IWebElement div, IWebDriver driver)
        {
            await Task.Delay(5000);

            div = driver.FindElement(By.Id("WrapperCardFlight0"));

            div.Click();

            await Task.Delay(2000);

            div = driver.FindElement(By.Id("WrapperBundleCardbundle-detail-00"));

            var price = div.FindElement(By.CssSelector("span[class='sc-gKLXLV hSGFVx']")).Text;

            div = div.FindElement(By.CssSelector("button"));

            await Task.Delay(2000);

            div.Click();

            return price.Replace(".","").Replace(",",".");
        }
    }
}
