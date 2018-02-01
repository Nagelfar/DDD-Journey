using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Domain.Tests.Assignment3.Mod.Helpers;
using Newtonsoft.Json;
using Xunit;
using static Domain.Tests.Output;

namespace Domain.Tests.Assignment3.Mod
{
    public class Assignment3
    {
        [Fact]
        public void canDispatchMessages()
        {
            var service = new TodoService();

            var id = Guid.NewGuid();

            var command = new ComposeTodoCommand
            {
                Id = id,
                Task = "Sommerreifen aufziehen",
                Until = new DateTime(2018, 4, 15)
            };

            DispatchToWhen.SendMessageRightAway(service, command);

            DispatchToWhen.SendMessageRightAway(service, new ResolveTodoCommand
            {
                Id = id
            });
        }

        // [Fact]
        public void massiveLoad()
        {
            var service = new TodoService();

            var commands = Enumerable.Range(1, 15)
                .Select(i => new ComposeTodoCommand
                {
                    Id = Guid.NewGuid(),
                    Task = "Sommerreifen aufziehen " + i,
                    Until = new DateTime(2018, 4, i)
                })
                .ToList();


            commands.ForEach(command => DispatchToWhen.WithBuffer(service, command));

            Console.ReadLine();
        }

        // [Fact]
        public void saveMessage()
        {
            var service = new TodoService();

            var command = new ComposeTodoCommand
            {
                Id = Guid.NewGuid(),
                Task = "Sommerreifen aufziehen",
                Until = new DateTime(2018, 4, 15)
            };

            DispatchToWhen.StoreMessage(service, command);
        }

        // [Fact]
        public void loadMessages()
        {
            var service = new TodoService();
            DispatchToWhen.SendStoredMessage<ComposeTodoCommand>(service);
        }
    }

    public class ComposeTodoCommand
    {
        public string Task { get; set; }
        public DateTime? Until { get; set; }
        public Guid Id { get; internal set; }
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
        private DateTime? resolvedOn;

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
            if (!resolvedOn.HasValue)
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

    namespace Helpers
    {

        using Service = Object;
        using Message = Object;

        public static class DispatchToWhen
        {
            public static void SendMessageRightAway(Service target, Message message)
            {
                messageCount++;
                InColor(ConsoleColor.Red, $"Dispatching a message to a service");

                SendViaDynamicDispatch(target, message);
                messageCount--;
            }

            private static void SendViaDynamicDispatch(Service target, Message message)
            {
                InColor(ConsoleColor.Red, $">Sending message via dynamic dispatch {message.GetType().Name} to service {target.GetType().Name}");

                // here we do a dynamic dispatch
                // this is not typesafe and not the fastest/best way
                // but it is easy to write & understand
                ((dynamic)target).When((dynamic)message);
            }







            public static void StoreMessage(Service target, Message message)
            {
                InColor(ConsoleColor.Red, $"Received message {message.GetType().Name} for service {target.GetType().Name} and storing it instead of sending");

                File.WriteAllText(target.GetType().Name + ".txt", JsonConvert.SerializeObject(message, Formatting.Indented));
            }

            public static void SendStoredMessage<TMessage>(Service target)
            {
                var filename = target.GetType().Name + ".txt";
                if (File.Exists(filename))
                {
                    var message = JsonConvert.DeserializeObject<TMessage>(File.ReadAllText(filename));

                    DispatchToWhen.SendMessageRightAway(target, message);
                }
            }


            private static int messageCount = 0;
            public static void WithTempBuffer(Service target, Message command)
            {

                if (messageCount <= 4)
                    SendMessageRightAway(target, command);
                else
                    WithBuffer(target, command);

            }

            public static void WithBuffer(Service target, Message message)
            {
                InColor(ConsoleColor.Red, $"Queueing message {message.GetType().Name} for service {target.GetType().Name} and sending it later");

                if (!queues.ContainsKey(target))
                    queues[target] = new Queue<Service>();

                InColor(ConsoleColor.Red, $"Queue size is {queues[target].Count}");
                queues[target].Enqueue(message);
            }
            private static void DispatchQueuedMessages(object state)
            {
                foreach (var queue in queues)
                {
                    if (queue.Value.TryDequeue(out Message message))
                    {
                        InColor(ConsoleColor.Red, ">Found queued message " + message);
                        SendViaDynamicDispatch(queue.Key, message);
                        InColor(ConsoleColor.Red, $">>Worked through a message - queue size is now {queue.Value.Count}");
                    }
                }
            }

            private static Timer worker = new Timer(DispatchQueuedMessages, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            private static IDictionary<Service, Queue<Message>> queues = new Dictionary<Service, Queue<Service>>();
        }
    }
}