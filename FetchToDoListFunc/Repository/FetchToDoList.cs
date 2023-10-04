using FetchToDoListFunc.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFunc.Repository
{
    internal class FetchToDoList : IFetchToDoList
    {
        private readonly IMongoCollection<TaskList> _taskCollection;

        public FetchToDoList(IMongoDatabase database)
        {
            _taskCollection = database.GetCollection<TaskList>("todolist");
        }

        public async Task<ActionResult> CreateAsync(TaskList toDolist)
        {
            try
            {
                await _taskCollection.InsertOneAsync(toDolist);
                return new OkObjectResult("Task Added Successfully");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        public async Task<ActionResult> DeleteTaskAsync(ObjectId taskId)
        {
            try
            {
                await _taskCollection.FindOneAndDeleteAsync(Builders<TaskList>.Filter.Eq("_id", taskId));
                return new OkObjectResult("Task Deleted Successfully");
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
                throw;
            }
        }

        public async Task<List<TaskList>> GetAllAsync()
        {
            List<TaskList> tasks = null;
            try
            {
                tasks = await _taskCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                //return new BadRequestObjectResult(ex.Message);
                throw;
            }
            return tasks;
        }

        public Task<int> GetLatestTaskIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TaskList> GetTaskDetailsByTaskIdAsync(ObjectId taskId)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<TaskList>> GetTaskDetailsByTaskNameAsync(string taskName)
        {
            try
            {
                var product = await _taskCollection.Find(_ => _.TaskName.Equals(taskName)).ToListAsync<TaskList>();
                return new OkObjectResult(product);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        public async Task<ActionResult> UpdateTaskStatusAsync(ObjectId taskId)
        {
            try
            {
                await _taskCollection.FindOneAndUpdateAsync(Builders<TaskList>.Filter.Eq("_id", taskId), Builders<TaskList>.Update.Set("TaskStatus", "Completed"));
                return new OkObjectResult("Task updated successfully");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }
    }
}
