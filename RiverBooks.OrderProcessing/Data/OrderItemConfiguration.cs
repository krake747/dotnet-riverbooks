using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RiverBooks.OrderProcessing.Data;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
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