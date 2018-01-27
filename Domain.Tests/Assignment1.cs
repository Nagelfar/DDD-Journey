using System;
using Xunit;

namespace Domain.Tests.Assignment1
{
    public class Assignment1
    {
        [Fact]
        public void canCreateTodo()
        {
            var service = new TodoService();
            service.CreateTodo("Sommerreifen aufziehen", new DateTime(2018, 4, 15));
        }
    }
 
    public class Todo
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Priority Priority { get;  set; }
    }


    public enum Priority
    {
        Low,
        Medium,
        High
    }

     public class TodoService
    {
        IRepository<Todo> repository = new InMemoryRepository<Todo>();

        public void CreateTodo(string text, DateTime date)
        {
            var todo = new Todo()
            {
                Text = text,
                Date = date,
                Priority = date < DateTime.Now ? Priority.Medium : Priority.Low
            };

            repository.Save(todo);
        }
    }

}
