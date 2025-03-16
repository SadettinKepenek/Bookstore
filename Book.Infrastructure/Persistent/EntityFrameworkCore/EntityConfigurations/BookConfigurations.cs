using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Book.Infrastructure.Persistent.EntityFrameworkCore.EntityConfigurations;

public class BookConfigurations : IEntityTypeConfiguration<Domain.Book>
{
    public void Configure(EntityTypeBuilder<Domain.Book> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property<uint>("Version").IsRowVersion();
        builder.ToTable("Books", "book");
    }
}