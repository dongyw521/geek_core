using Dyw.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyw.Infrastructure.Core.Extensions
{
    public static class MediatorExtension
    {
        public async static Task DispatchDomainEventsAsync(this IMediator mediator, DbContext dbContext)
        {
            var domainEntities = dbContext.ChangeTracker.Entries<Entity>().Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());
            var domainEvents = domainEntities.SelectMany(e => e.Entity.DomainEvents).ToList();
            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvent());
            foreach (var _event in domainEvents)
            {
                await mediator.Publish(_event);//触发当前被操作实体上绑定的所有领域事件
            }
        }
    }
}
