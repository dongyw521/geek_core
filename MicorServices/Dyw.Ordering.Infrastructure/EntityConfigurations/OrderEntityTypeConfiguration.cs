using Dyw.Ordering.Domain.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dyw.Ordering.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.ToTable("order");
            builder.Property(o => o.UserId).HasMaxLength(100);
            builder.Property(o => o.UserName).HasMaxLength(200);
            builder.OwnsOne(o => o.Address, _b => {
                _b.WithOwner();
                _b.Property(a => a.City).HasMaxLength(20);
                _b.Property(a => a.Street).HasMaxLength(300);
                _b.Property(a => a.ZipCode).HasMaxLength(10);
            });
        }
    }
}
