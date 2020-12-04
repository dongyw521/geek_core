using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dyw.Domain.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dyw.Infrastructure.Core
{
    public class EFContext : DbContext, IUnitOfWork, ITransaction
    {
        protected IMediator mediator;

        public EFContext(DbContextOptions options, IMediator _mediator) : base(options)
        {
            mediator = _mediator;
        }

        #region 事务

        private IDbContextTransaction _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = Database.BeginTransaction();
            return Task.FromResult(_currentTransaction);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"The transaction {transaction.TransactionId} is not the current!");
            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollBackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        public void RollBackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion

        #region IUnitOfWork
        /// <summary>
        /// 一次性持久化到库，并触发领域事件
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            var domainEntities = this.ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());
            var domainEvents = domainEntities.SelectMany(e => e.Entity.DomainEvents).ToList();
            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvent());
            foreach (var _event in domainEvents)
            {
                await mediator.Publish(_event);//触发当前被操作实体上绑定的所有领域事件
            }
            return true;
        }

        #endregion


    }
}
