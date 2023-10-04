using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FetchToDoListFunc
{
    public static class FetchToDoListFunc
    {
        [FunctionName("Fetch")]
        public static string Fetch(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "fetch")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return "Hello";
        }
    }
}
