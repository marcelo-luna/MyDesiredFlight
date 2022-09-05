using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyDesiredFlight.Bll.Interface;

namespace MyDesiredFlight.HtmlFunction
{
    public class Function1
    {
        public ISearchFly _searchFly;

        public Function1(ISearchFly searchFly)
        {
            _searchFly = searchFly;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "trip/{origin}/{destination}/{dateFrom}/{dateTo}")] HttpRequest req,
            string origin, string destination, string dateFrom, string dateTo, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var latam = _searchFly.SearchFly(origin, destination, dateFrom, dateTo);

            await Task.WhenAll(latam);

            string responseMessage = $"Price Latam {latam.Result}";

            return new OkObjectResult(responseMessage);
        }
    }
}
