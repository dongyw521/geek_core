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
        IUnitOfWork UnitOfWork { get; }

        TEntity Add(TEntity entity);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        bool Remove(TEntity entity);

        Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    }

    public interface IRepository<TEntity, TPrimaryKey> : IRepository<TEntity> where TEntity : Entity<TPrimaryKey>, IAggregateRoot
    {
        bool Delete(TPrimaryKey Id);

        Task<bool> DeleteAsync(TPrimaryKey Id, CancellationToken cancellationToken = default);

        TEntity Get(TPrimaryKey Id);

        Task<TEntity> GetAsync(TPrimaryKey Id, CancellationToken cancellationToken = default);
    }
}
