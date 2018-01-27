using System;

namespace Domain
{
    public interface IRepository<T>
    {
        void Save(T entity);
    }
}
