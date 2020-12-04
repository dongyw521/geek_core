using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dyw.Infrastructure.Core
{
    public interface ITransaction
    {
        bool HasActiveTransaction { get; }

        IDbContextTransaction GetCurrentTransaction();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync(IDbContextTransaction transaction);

        void RollBackTransaction();
    }
}
