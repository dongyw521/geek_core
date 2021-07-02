using Dyw.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dyw.Infrastructure.Core
{
    public abstract class Repository<TEntity, TDbContext> : IRepository<TEntity> where TEntity : Entity, IAggregateRoot where TDbContext : EFContext
    {
        protected virtual TDbContext DbContext { get; set; }

        public Repository(TDbContext _dbContext)
        {
            DbContext = _dbContext;
        }

        /// <summary>
        /// 工作单元是嵌入到仓储对象里的，并且是由DbContext来实现
        /// </summary>
        public virtual IUnitOfWork UnitOfWork => DbContext;


        public virtual TEntity Add(TEntity entity)
        {
            return DbContext.Add(entity).Entity;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(entity));
        }

        public virtual bool Remove(TEntity entity)
        {
            DbContext.Remove(entity);
            return true;
        }

        public virtual Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Remove(entity));
        }

        public virtual TEntity Update(TEntity entity)
        {
            return DbContext.Update(entity).Entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }
    }

    public abstract class Repository<TEntity, TPrimaryKey, TDbContext> : Repository<TEntity, TDbContext>, IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>, IAggregateRoot where TDbContext : EFContext
    {
        public Repository(TDbContext context) : base(context) 
        { }

        public virtual bool Delete(TPrimaryKey Id)
        {
            var entity = DbContext.Find<TEntity>(Id);
            if (entity == null) return false;
            DbContext.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TPrimaryKey Id, CancellationToken cancellationToken = default)
        {
            var entity = await DbContext.FindAsync<TEntity>(Id, cancellationToken);
            if (entity == null)
                return false;
            DbContext.Remove(entity);
            return true;
        }

        public virtual TEntity Get(TPrimaryKey Id)
        {
            return DbContext.Find<TEntity>(Id);
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey Id, CancellationToken cancellationToken = default)
        {
            return await DbContext.FindAsync<TEntity>(Id, cancellationToken);
        }
    }
}
