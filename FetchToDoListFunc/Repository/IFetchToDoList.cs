using FetchToDoListFunc.Model;
using Microsoft.AspNetCore.Mvc;
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
        Task<TaskList> GetTaskDetailsByTaskIdAsync(string taskId);
        Task<ActionResult<TaskList>> GetTaskDetailsByTaskNameAsync(string taskName);
        Task<ActionResult> CreateAsync(TaskList toDolist);
        Task<ActionResult> UpdateTaskStatusAsync(string taskID);
        Task<ActionResult> DeleteTaskAsync(string taskId);
    }
}
