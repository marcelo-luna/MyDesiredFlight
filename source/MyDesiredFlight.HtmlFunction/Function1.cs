using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyDesiredFlight.Bll.Interface;
using MyDesiredFlight.Bll.AirLines;

namespace MyDesiredFlight.HtmlFunction
{
    public class Function1
    {
        public ISearchFly _latam;
        public ISearchFly _ita;

        public Function1(ISearchFly latam, ISearchFly ita)
        {
            _latam = latam;
            _ita = ita;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "trip/{origin}/{destination}/{dateFrom}/{dateTo}")] HttpRequest req,
            string origin, string destination, string dateFrom, string dateTo, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //var latam = _latam.SearchFly(origin, destination, dateFrom, dateTo);
            var ita = _ita.SearchFly(origin, destination, dateFrom, dateTo);

            await Task.WhenAll(ita);

            string responseMessage = $"Price ita {ita.Result}";

            return new OkObjectResult(responseMessage);
        }
    }   
}
