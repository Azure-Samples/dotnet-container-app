using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContainerApp.TodoApi.Models;
using Microsoft.Extensions.Logging;
using ContainerApp.TodoApi.Repository.Interfaces;

namespace ContainerApp.TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private ITodoItemRepository _repository;
        private ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoItemRepository repository, ILogger<TodoItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            try
            {
                _logger.LogInformation("Method - GetTodoItems");
                return await _repository.GetAll();
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw;
            }
        }

        // GET
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            try
            {
                _logger.LogInformation("Method - GetTodoItem");
                _logger.LogInformation("Param - Id = " + id);

                var todoItem = await _repository.Get(id);

                if (todoItem == null)
                {
                    return NotFound();
                }

                return todoItem;
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw;
            }
            
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            try
            {
                _logger.LogInformation("Method - PutTodoItem");
                _logger.LogInformation("Param - Id = " + id);
                _logger.LogInformation("Param - todoItem = " + todoItem);
                
                if (id != todoItem.Id)
                {
                    return BadRequest();
                }

                await _repository.Update(todoItem);
            
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw;
            }
        }

        // ADD
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            try
            {
                _logger.LogInformation("Method - PostTodoItem");
                _logger.LogInformation("Param - todoItem = " + todoItem);

                await _repository.Add(todoItem);
                
                return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw;
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(int id)
        {
            try
            {
                _logger.LogInformation("Method - DeleteTodoItem");
                _logger.LogInformation("Param - Id = " + id);

                bool res = await _repository.Delete(id);
                
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw;
            }
        }
    }
}