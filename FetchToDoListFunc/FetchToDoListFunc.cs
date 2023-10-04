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
using MongoDB.Bson;
using FetchToDoListFunc.Utility;

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
        public static async Task<ActionResult> Fetch(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fetch")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Check if we have authentication info.
            ValidateJWT auth = new ValidateJWT(req);
            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }
            return new OkObjectResult("hurray"); ;
        }

        [FunctionName("getall")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetAllTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger logger)
        {
            ValidateJWT auth = new ValidateJWT(req);
            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }
            return new OkObjectResult(await _fetchRepo.GetAllAsync());
        }

        [FunctionName("add-task")]
        public async Task<ActionResult<List<TaskList>>> CreateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
        {
            try
            {
                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    return new UnauthorizedResult(); // No authentication info.
                }
                var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<TaskList>(reqBody);
                var task = new TaskList
                {
                    //TaskId=input.TaskId,
                    TaskName=input.TaskName,
                    Description=input.Description,
                    TaskStartDate=input.TaskStartDate,
                    TaskEndDate=input.TaskEndDate,
                    TaskStatus=input.TaskStatus,
                    TotalEffortRequired=input.TotalEffortRequired
                };
                return new OkObjectResult(await _fetchRepo.CreateAsync(task));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("gettaskbytaskname")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetTaskDetails([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gettaskbytaskname/{taskname}")] HttpRequest req, ILogger logger,string taskname)
        {
            ValidateJWT auth = new ValidateJWT(req);
            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }
            return new OkObjectResult(await _fetchRepo.GetTaskDetailsByTaskNameAsync(taskname));
        }

        [FunctionName("update-task")]
        public async Task<ActionResult<List<TaskList>>> UpdateTaskStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "update-task/{id}")] HttpRequest req, ILogger logger,string id)
        {
            try
            {
                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    return new UnauthorizedResult(); // No authentication info.
                }
                return new OkObjectResult(await _fetchRepo.UpdateTaskStatusAsync(ObjectId.Parse(id)));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("delete-task")]
        public async Task<ActionResult> DeleteProduct([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete-product/{id}")] HttpRequest req, ILogger logger, string id)
        {
            try
            {
                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    return new UnauthorizedResult(); // No authentication info.
                }
                return new OkObjectResult(await _fetchRepo.DeleteTaskAsync(ObjectId.Parse(id)));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }
    }
}
