using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Dyw.Infrastructure.Core
{
    public class EFContext : DbContext, IUnitOfWork
    {
        protected IMediator mediator;

        public EFContext(DbContextOptions options,IMediator _mediator) : base(options)
        {
            mediator = _mediator;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            throw new NotImplementedException();
        }


    }
}
