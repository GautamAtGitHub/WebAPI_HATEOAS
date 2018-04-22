using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI_HATEOAS.Model;

namespace WebAPI_HATEOAS.Controllers
{
    [Route("api/Todo")]
    public class TodoController : Controller
    {
        private readonly ToDoContext _context;

        public TodoController(ToDoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Id = 101, Name = "Demo Task", IsComplete = false });
                _context.SaveChanges();
            }
        }

        // GET: api/Todo
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}.{format?}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Todo
        [HttpPost]
        public void Post([FromBody]TodoItem item)
        {
            if (item != null)
            {
                _context.TodoItems.Add(item);
                _context.SaveChanges();
            }
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]TodoItem item)
        {
            if (item != null && item.Id != id)
            {
                var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
                if (todo != null)
                {
                    todo.IsComplete = item.IsComplete;
                    todo.Name = item.Name;

                    _context.TodoItems.Update(todo);
                    _context.SaveChanges();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                _context.SaveChanges();
            }
        }
    }
}