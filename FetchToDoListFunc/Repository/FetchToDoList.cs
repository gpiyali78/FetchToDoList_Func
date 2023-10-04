using FetchToDoListFunc.Model;
using Microsoft.AspNetCore.Mvc;
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

        public Task<TaskList> GetTaskDetailsByTaskIdAsync(string taskId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskList> GetTaskDetailsByTaskNameAsync(string taskName)
        {
            throw new NotImplementedException();
        }
    }
}
