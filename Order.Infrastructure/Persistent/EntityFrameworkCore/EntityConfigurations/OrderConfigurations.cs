using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Persistent.EntityFrameworkCore.EntityConfigurations;

public class OrderConfigurations : IEntityTypeConfiguration<Domain.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Order> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ToTable("Orders", "order");

    }
}