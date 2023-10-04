using FetchToDoListFunc.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFunc.Repository
{
    public interface IFetchToDoList
    {
        Task<List<TaskList>> GetAllAsync();
        Task<int> GetLatestTaskIdAsync();
        Task<TaskList> GetTaskDetailsByTaskIdAsync(ObjectId taskId);
        Task<ActionResult<TaskList>> GetTaskDetailsByTaskNameAsync(string taskName);
        Task<ActionResult> CreateAsync(TaskList toDolist);
        Task<ActionResult> UpdateTaskStatusAsync(ObjectId taskID);
        Task<ActionResult> DeleteTaskAsync(ObjectId taskId);
    }
}
