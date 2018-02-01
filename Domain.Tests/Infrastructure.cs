using System;
using System.Linq;

namespace Domain.Tests
{
    using System.Collections.Generic;
    using static Output;
     public interface IRepository<T>
    {
        void Save(T entity);

        void Save(Guid id, T entity);

        T  Get(Guid id);
    }
    
    public class InMemoryRepository<T> : IRepository<T>
    {
        private IDictionary<Guid, T> entities = new Dictionary<Guid, T>();

        public T Get(Guid id)
        {
            return entities[id];
        }

        public void Save(T entity)
        {
            // NoOp
            InColor(ConsoleColor.Magenta, "", "Saving " + entity, "");

            entities[Guid.NewGuid()] = entity;
        }

        public void Save(Guid id, T entity)
        {
            InColor(ConsoleColor.Magenta, "", "Saving " + entity + " with id " + id, "");

            entities[id] = entity;
        }
    }

    public static class Output
    {
        public static void InColor(ConsoleColor color, params string[] text)
        {
            Console.ForegroundColor = color;
            text.ToList().ForEach(Console.WriteLine);
            Console.ResetColor();
        }
    }
}