using System;
using System.Linq;

namespace Domain.Tests
{
    using static Output;
    public class InMemoryRepository<T> : IRepository<T>
    {
        public void Save(T entity)
        {
            // NoOp
            InColor(ConsoleColor.Magenta, "", "Saving " + entity, "");
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