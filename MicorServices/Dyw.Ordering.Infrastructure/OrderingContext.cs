using Dyw.Infrastructure.Core;
using Dyw.Ordering.Domain.Entities.OrderAggregate;
using Dyw.Ordering.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dyw.Ordering.Infrastructure
{
    public class OrderingContext : EFContext
    {
        public OrderingContext(DbContextOptions options, IMediator _mediator) : base(options, _mediator)
        {

        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
