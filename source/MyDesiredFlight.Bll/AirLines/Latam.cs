﻿using Microsoft.Extensions.Logging;
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
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless", "--disable-gpu", "--no-sandbox", "--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.75 Safari/537.36");
            var service = ChromeDriverService.CreateDefaultService("/usr/bin/", "chromedriver");
            //var service = ChromeDriverService.CreateDefaultService();

            //using (var driver = new ChromeDriver(options: chromeOptions))
            using (var driver = new ChromeDriver(service: service, options: chromeOptions))
            {
                INavigation nav = driver.Navigate();

                var replacedUrl = "https://www.latamairlines.com/br/pt";
                nav.GoToUrl(replacedUrl);
                await Task.Delay(5000);

                replacedUrl = BASE_URL.Replace("{origin}", origin).Replace("{dateTo}", dateTo).Replace("{dateFrom}", dateFrom).Replace("{destination}", destination);
                //replacedUrl = "https://www.latamairlines.com/br/pt";
                nav.GoToUrl(replacedUrl);

                _logger.LogInformation($"Base URL {replacedUrl}");

                _logger.LogInformation("Waiting initial page");
                await Task.Delay(40000);

                _logger.LogInformation("Select country");


                IWebElement div;

                try
                {

                    //var testt = driver.FindElement(By.ClassName("sec-text-container"));

                    var test = driver.FindElement(By.TagName("body")).Text;
                    var testtt = driver.FindElement(By.TagName("body"));

                    div = driver.FindElement(By.Id("country-suggestion-body-reject-change"));
                    div.Click();


                }
                catch (Exception)
                {
                    div = driver.FindElement(By.Id("country-suggestion-dialog-close"));
                    div.Click();

                }
                finally
                {
                }


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
