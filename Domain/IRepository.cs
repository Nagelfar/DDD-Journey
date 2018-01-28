using System;

namespace Domain
{
    public interface IRepository<T>
    {
        void Save(T entity);

        void Save(Guid id, T entity);

        T  Get(Guid id);
    }
}
