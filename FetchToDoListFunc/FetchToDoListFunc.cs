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
using Microsoft.ApplicationInsights;

namespace FetchToDoListFunc
{
    public class FetchToDoListFunc
    {
        private readonly IFetchToDoList _fetchRepo;
        private static readonly TelemetryClient telemetryClient = new TelemetryClient();


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
            telemetryClient.TrackEvent("Fetch function Executed");

            // Check if we have authentication info.
            ValidateJWT auth = new ValidateJWT(req);
            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }
            return new OkObjectResult("hurray"); 
        }

        [FunctionName("getall")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetAllTask([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger logger)
        {
            try
            {
                logger.LogInformation("getall function called");
                telemetryClient.TrackEvent("getall function called");

                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    logger.LogInformation("getall function - unauthorized");
                    telemetryClient.TrackEvent("getall function - unauthorized");
                    return new UnauthorizedResult(); // No authentication info.
                }

                return new OkObjectResult(await _fetchRepo.GetAllAsync());
            }catch (Exception ex)
            {
                logger.LogError("getall function" + ex.Message);
                telemetryClient.TrackEvent("getall function" + ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("add-task")]
        public async Task<ActionResult<List<TaskList>>> CreateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
        {
            try
            {
                logger.LogInformation("add-task function called");
                telemetryClient.TrackEvent("add-task function called");

                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    logger.LogInformation("add-task function - unauthorized");
                    telemetryClient.TrackEvent("add-task function - unauthorized");
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
                logger.LogError("add-task function" + ex.Message);
                telemetryClient.TrackEvent("add-task function" + ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("gettaskbytaskname")]
        public async Task<ActionResult<IEnumerable<TaskList>>> GetTaskDetails([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gettaskbytaskname/{taskname}")] HttpRequest req, ILogger logger,string taskname)
        {
            try
            {
                logger.LogInformation("gettaskbytaskname function called");
                telemetryClient.TrackEvent("gettaskbytaskname function called");
                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    logger.LogInformation("gettaskbytaskname function - unauthorized");
                    telemetryClient.TrackEvent("gettaskbytaskname function - unauthorized");
                    return new UnauthorizedResult(); // No authentication info.
                }
                return new OkObjectResult(await _fetchRepo.GetTaskDetailsByTaskNameAsync(taskname));
            }catch(Exception ex)
            {
                logger.LogError("gettaskbytaskname function" + ex.Message);
                telemetryClient.TrackEvent("gettaskbytaskname function" + ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("update-task")]
        public async Task<ActionResult<List<TaskList>>> UpdateTaskStatus([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "update-task/{id}")] HttpRequest req, ILogger logger,string id)
        {
            try
            {
                logger.LogInformation("update-task function called");
                telemetryClient.TrackEvent("update-task function called");

                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    logger.LogInformation("update-task function - unauthorized");
                    telemetryClient.TrackEvent("update-task function - unauthorized");
                    return new UnauthorizedResult(); // No authentication info.
                }
                return new OkObjectResult(await _fetchRepo.UpdateTaskStatusAsync(ObjectId.Parse(id)));
            }
            catch (Exception ex)
            {
                logger.LogError("update-task function" + ex.Message);
                telemetryClient.TrackEvent("update-task function" + ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [FunctionName("delete-task")]
        public async Task<ActionResult> DeleteProduct([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete-product/{id}")] HttpRequest req, ILogger logger, string id)
        {
            try
            {
                logger.LogInformation("delete-task function called");
                telemetryClient.TrackEvent("delete-task function called");

                ValidateJWT auth = new ValidateJWT(req);
                if (!auth.IsValid)
                {
                    logger.LogInformation("delete-task function - unauthorized");
                    telemetryClient.TrackEvent("delete-task function - unauthorized");
                    return new UnauthorizedResult(); // No authentication info.
                }
                return new OkObjectResult(await _fetchRepo.DeleteTaskAsync(ObjectId.Parse(id)));
            }
            catch (Exception ex)
            {
                logger.LogError("delete-task function" + ex.Message);
                telemetryClient.TrackEvent("delete-task function" + ex.Message);
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }
    }
}
