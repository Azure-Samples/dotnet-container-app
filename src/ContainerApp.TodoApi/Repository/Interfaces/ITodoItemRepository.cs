using ContainerApp.TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContainerApp.TodoApi.Repository.Interfaces
{  
    public interface ITodoItemRepository   
    {
        /*
        Exemplo de como implementar repository pattern
        https://www.c-sharpcorner.com/article/repository-pattern-in-asp-net-core/
        */

        Task<bool> Add(TodoItem model);  
        Task<List<TodoItem>> GetAll();  
        Task<TodoItem> Get(int id);  
        Task<bool> Delete(int id);  
        Task<bool> Update(TodoItem model);  
    }  
}