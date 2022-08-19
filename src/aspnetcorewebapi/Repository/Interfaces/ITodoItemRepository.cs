using aspnetcorewebapi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aspnetcorewebapi.Repository.Interfaces
{  
    public interface ITodoItemRepository   
    {
        Task<bool> Add(TodoItem model);  
        Task<List<TodoItem>> GetAll();  
        Task<TodoItem> Get(int id);  
        Task<bool> Delete(int id);  
        Task<bool> Update(TodoItem model);  
    }  
}