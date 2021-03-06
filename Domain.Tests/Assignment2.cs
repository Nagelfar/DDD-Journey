using System;
using Xunit;

namespace Domain.Tests
{
    public class Assignment2
    {
        [Fact]
        public void canResolveATodoIssue()
        {
            // Arrange
            var service = new TodoService();
            var id = Guid.NewGuid();
            service.ComposeTodo(id, "Sommerreifen aufziehen", new System.DateTime(2018, 4, 15));

            // Act
            service.Resolve(id);
        }
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

        public void ComposeTodo(Guid id, string task, DateTime until)
        {
            var todo = Todo.ComposeNew(task);

            todo.ShiftDeadline(until);

            repository.Save(id, todo);
        }

        internal void Resolve(Guid id)
        {
            var todo = repository.Get(id);
            todo.Resolve();
        }
    }
}