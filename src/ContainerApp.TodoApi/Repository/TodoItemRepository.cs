using System;
using System.Collections.Generic;
using ContainerApp.TodoApi.Models;
using ContainerApp.TodoApi.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContainerApp.TodoApi.Repository
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly MyDbContext _context;

        public TodoItemRepository(MyDbContext context)
        {
            _context = context;
        }
        async Task<bool> ITodoItemRepository.Add(TodoItem model)
        {
            _context.TodoItems.Add(model);
            int res = await _context.SaveChangesAsync();
            return true;
        }

        async Task<bool> ITodoItemRepository.Delete(int id)
        {
            var _item = await _context.TodoItems.FindAsync(id);
            _context.TodoItems.Remove(_item);
            int res = await _context.SaveChangesAsync();
            return true;
        }

        async Task<TodoItem> ITodoItemRepository.Get(int id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        async Task<List<TodoItem>> ITodoItemRepository.GetAll()
        {
            return await _context.TodoItems.ToListAsync();
        }

        async Task<bool> ITodoItemRepository.Update(TodoItem model)
        {
            _context.Entry(model).State = EntityState.Modified;
            int res = await _context.SaveChangesAsync();
            return true;
        }
    }
}