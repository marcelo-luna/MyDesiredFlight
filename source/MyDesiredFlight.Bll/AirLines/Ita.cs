using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyDesiredFlight.Bll.Interface;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools;

namespace MyDesiredFlight.Bll.AirLines
{
    public class Ita : ISearchFly
    {
        private readonly string BASE_URL;
        private readonly ILogger<Ita> _logger;

        public Ita(IConfiguration configuration, ILogger<Ita> logger)
        {
            BASE_URL = configuration.GetSection("ITA_URL").Value;
            _logger = logger;
        }

        public async Task<decimal> SearchFly(string origin, string destination, string dateFrom, string dateTo, string[] options)
        {
            var chromeOptions = new ChromeOptions();
            #region windows
            //chromeOptions.AddArguments("--headless", "--disable-gpu", "--no-sandbox", "--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36");
            var service = ChromeDriverService.CreateDefaultService();
            #endregion

            #region linux
            //chromeOptions.AddArguments("--disable-xss-auditor", "--disable-web-security","--disable-blink-features=AutomationControlled", "--headless", "--disable-gpu", "--no-sandbox", "--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.96 Safari/537.36");
            //var service = ChromeDriverService.CreateDefaultService("/usr/bin/", "chromedriver");
            #endregion

            using (var driver = new ChromeDriver(options: chromeOptions))
            //using (var driver = new ChromeDriver(service: service, options: chromeOptions))
            {
                INavigation nav = driver.Navigate();
                nav.GoToUrl(BASE_URL);

                IWebElement webElement;


                var originInput = driver.FindElement(By.Id("Origin"));

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('Origin').setAttribute('value', 'GRU')");
                js.ExecuteScript("document.getElementById('Destination').setAttribute('value', 'MXP')");
                js.ExecuteScript("document.getElementById('andata').setAttribute('value', '25/09/2022')");
                js.ExecuteScript("document.getElementById('ritorno').setAttribute('value', '25/10/2022')");

                driver.FindElement(By.Id("cercaVoliSubmit")).Click();

                //driver.FindElement(By.XPath("//a[@data-details-flight='0']"));

                var total = 0;

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
