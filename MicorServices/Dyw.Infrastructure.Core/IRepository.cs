using Dyw.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dyw.Infrastructure.Core
{
    public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        bool Remove(TEntity entity);

        Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        bool Delete(TKey Id);

        Task<bool> DeleteAsync(TKey Id, CancellationToken cancellationToken = default);

        TEntity Get(TKey Id);

        Task<TEntity> GetAsync(TKey Id, CancellationToken cancellationToken = default);
    }
}
