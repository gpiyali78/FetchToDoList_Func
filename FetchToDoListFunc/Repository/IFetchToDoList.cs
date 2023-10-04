using FetchToDoListFunc.Model;
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
        Task<TaskList> GetTaskDetailsByTaskNameAsync(string taskName);
    }
}
