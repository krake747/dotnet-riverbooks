using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RiverBooks.OrderProcessing.Data;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.ComplexProperty(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street1)
                .HasMaxLength(DataSchemaConstants.StreetMaxLength);            
            address.Property(a => a.Street2)
                .HasMaxLength(DataSchemaConstants.StreetMaxLength);            
            address.Property(a => a.City)
                .HasMaxLength(DataSchemaConstants.CityMaxLength);            
            address.Property(a => a.State)
                .HasMaxLength(DataSchemaConstants.StateMaxLength);            
            address.Property(a => a.PostalCode)
                .HasMaxLength(DataSchemaConstants.PostalCodeMaxLength);            
            address.Property(a => a.Country)
                .HasMaxLength(DataSchemaConstants.CountryMaxLength);
        });
        
        builder.ComplexProperty(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Street1)
                .HasMaxLength(DataSchemaConstants.StreetMaxLength);            
            address.Property(a => a.Street2)
                .HasMaxLength(DataSchemaConstants.StreetMaxLength);            
            address.Property(a => a.City)
                .HasMaxLength(DataSchemaConstants.CityMaxLength);            
            address.Property(a => a.State)
                .HasMaxLength(DataSchemaConstants.StateMaxLength);            
            address.Property(a => a.PostalCode)
                .HasMaxLength(DataSchemaConstants.PostalCodeMaxLength);            
            address.Property(a => a.Country)
                .HasMaxLength(DataSchemaConstants.CountryMaxLength);
        });
    }
}