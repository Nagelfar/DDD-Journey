using System;
using Xunit;

namespace Domain.Tests.Assignment1.Mod
{
    public class Assignment1
    {
        [Fact]
        public void canCreateTodo()
        {
            var service = new TodoService();
            service.ComposeTodo("Sommerreifen aufziehen", new DateTime(2018, 4, 15));
        }
    }

    public class Todo
    {
        private readonly string task;
        private DateTime? until;
        private Priority priority;

        private Todo(string task)
        {
            this.task = task;
        }

        public static Todo ComposeNew(string task) => new Todo(task);

        public void ShiftDeadline(DateTime until)
        {
            this.until = until;
            this.UpdatePriority(DateTime.Now);
        }

        private void UpdatePriority(DateTime now)
        {
            if (until.HasValue && until < now)
                priority = Priority.High;
            else
                priority = Priority.Medium;
        }
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

        public void ComposeTodo(string task, DateTime until)
        {
            var todo = Todo.ComposeNew(task);
            
            todo.ShiftDeadline(until);

            repository.Save(todo);
        }
    }

}
