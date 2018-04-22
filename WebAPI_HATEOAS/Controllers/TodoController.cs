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
        public IActionResult GetById(long id, [FromHeader(Name = "Accept")]string accept)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (accept.EndsWith("hateoas"))
            {
                var link = new LinkHelper<TodoItem>(item);
                link.Links.Add(new Link
                {
                    Href = Url.Link("GetTodo", new { item.Id }),
                    Rel = "self",
                    method = "GET"
                });
                link.Links.Add(new Link
                {
                    Href = Url.Link("PutTodo", new { item.Id }),
                    Rel = "put-todo",
                    method = "PUT"
                });
                link.Links.Add(new Link
                {
                    Href = Url.Link("DeleteTodo", new { item.Id }),
                    Rel = "delete-todo",
                    method = "DELETE"
                });
                return new ObjectResult(link);
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
        [HttpPut("{id}", Name = "PutTodo")]
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
        [HttpDelete("{id}", Name = "DeleteTodo")]
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