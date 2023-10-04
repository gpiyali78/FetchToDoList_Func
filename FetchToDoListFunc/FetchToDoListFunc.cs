using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FetchToDoListFunc.Repository;
using System.Collections.Generic;
using FetchToDoListFunc.Model;

namespace FetchToDoListFunc
{
    public class FetchToDoListFunc
    {
        private readonly IFetchToDoList _fetchRepo;

        public FetchToDoListFunc(IFetchToDoList fetRepo)
        {
            _fetchRepo = fetRepo;
        }

        [FunctionName("Fetch")]
        public static string Fetch(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "fetch")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return "Hurray";
        }

        [FunctionName("getall")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetAllTask([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger logger)
        {
            return new OkObjectResult(await _fetchRepo.GetAllAsync());
        }
    }
}
