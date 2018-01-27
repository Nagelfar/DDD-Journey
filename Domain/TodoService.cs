using System;
using Domain.Model;

namespace Domain
{
    public class TodoService
    {
        IRepository<Todo> repository = null;

        public void CreateTodo(string text, DateTime until)
        {
            var todo = new Todo()
            {
                Text = text,
                Until = until
            };

            repository.Save(todo);
        }
    }
}