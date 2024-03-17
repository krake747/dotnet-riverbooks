using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Infrastructure.Data;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property(item => item.Description)
            .HasMaxLength(DataSchemaConstants.DefaultDescriptionLength)
            .IsRequired();
    }
}