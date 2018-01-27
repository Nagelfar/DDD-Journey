using System;
using Xunit;

namespace Domain.Tests.Assignment2
{
    public class Assignment2
    {
        [Fact]
        public void canCreateTodo()
        {
            var service = new TodoService();

            var command = new ComposeTodoCommand
            {
                Task = "Sommerreifen aufziehen",
                Until = new DateTime(2018, 4, 15)
            };
            service.When(command);
        }
    }


    public class ComposeTodoCommand
    {
        public string Task { get; set; }
        public DateTime? Until { get; set; }
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

        public void When(ComposeTodoCommand command)
        {
            var todo = Todo.ComposeNew(command.Task);

            if (command.Until.HasValue)
                todo.ShiftDeadline(command.Until.Value);

            repository.Save(todo);
        }
    }

}
