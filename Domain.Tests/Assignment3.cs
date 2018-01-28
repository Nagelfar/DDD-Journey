using System;
using Xunit;

namespace Domain.Tests.Assignment3
{
    public class Assignment3
    {
        [Fact]
        public void canCreateTodo()
        {
            var service = new TodoService();
            var id = Guid.NewGuid();

            var command = new ComposeTodoCommand
            {
                Id = id,
                Task = "Sommerreifen aufziehen",
                Until = new DateTime(2018, 4, 15)
            };

            service.When(command);

            service.When(new ResolveTodoCommand
            {
                Id = id
            });
        }
    }


    public class ComposeTodoCommand
    {
        public Guid Id { get; set; }
        public string Task { get; set; }
        public DateTime? Until { get; set; }
    }

    public class ResolveTodoCommand
    {
        public Guid Id { get; set; }
    }


    public class Todo
    {
        private readonly string task;
        private DateTime? until;
        private Priority priority;
        private DateTime resolvedOn;

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

        public void Resolve()
        {
            resolvedOn = DateTime.Now;
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

            repository.Save(command.Id, todo);
        }

        public void When(ResolveTodoCommand command)
        {
            var todo = repository.Get(command.Id);

            todo.Resolve();

            repository.Save(command.Id, todo);
        }
    }

}
