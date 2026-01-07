using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.Property(order => order.ExternalOrderNumber).IsRequired().HasMaxLength(100);
            builder.Property(order => order.ExternalShipmentId).IsRequired().HasMaxLength(100);
            builder.Property(order => order.CurrencyCode).HasMaxLength(3);
            builder.Property(order => order.CargoTrackingNumber).HasMaxLength(100);
            builder.Property(order => order.CargoProviderName).HasMaxLength(100);

            builder.Property(order => order.GrossAmount).HasPrecision(18, 2);
            builder.Property(order => order.TotalDiscount).HasPrecision(18, 2);
            builder.Property(order => order.PlatformCoveredDiscount).HasPrecision(18, 2);

            builder.HasIndex(order => order.ExternalOrderNumber);
            builder.HasIndex(order => order.ExternalShipmentId);
            builder.HasIndex(order => order.OrderDate);
            builder.HasIndex(order => order.MarketplaceAccountId);

            builder.HasMany(order => order.OrderItems)
                   .WithOne(orderItem => orderItem.Order)
                   .HasForeignKey(orderItem => orderItem.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
