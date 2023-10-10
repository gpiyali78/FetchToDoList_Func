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
using System.Collections;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.WebJobs.ServiceBus;
using System.Text;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;

namespace FetchToDoListFunc
{
    public class FetchToDoListFunction
    {
        private readonly IFetchToDoList _fetchRepo;
        private readonly IConfiguration _configuration;
        private static readonly TelemetryClient telemetryClient = new TelemetryClient();


        public FetchToDoListFunction(IFetchToDoList fetRepo, IConfiguration configuration)
        {
            _fetchRepo = fetRepo;
            _configuration = configuration;
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
        //[return: ServiceBus("todolist-queue", EntityType =ServiceBusEntityType.Queue,Connection = "AzureWebJobsServiceBus")]
        public async Task<ActionResult<string>> CreateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger logger)
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
                //var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                //var input = JsonConvert.DeserializeObject<TaskList>(reqBody);
                //var task = new TaskList
                //{
                //    //TaskId=input.TaskId,
                //    TaskName=input.TaskName,
                //    Description=input.Description,
                //    TaskStartDate=input.TaskStartDate,
                //    TaskEndDate=input.TaskEndDate,
                //    TaskStatus=input.TaskStatus,
                //    TotalEffortRequired=input.TotalEffortRequired
                //};
                //return new OkObjectResult(await _fetchRepo.CreateAsync(task));
                var message = await new StreamReader(req.Body).ReadToEndAsync();
                string queueName = "todolist-queue";
                ServiceBusClient serviceBusClient = new ServiceBusClient("Endpoint=sb://todolist-asb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=D+OgbaR4NcUokAt1SJcFTKoomop9NJawP+ASbEGxBA0=", new ServiceBusClientOptions()
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets
                });
                ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(message));

                return new OkObjectResult("Message sent to the queue.");
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
