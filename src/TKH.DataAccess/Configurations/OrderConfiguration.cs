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

            builder.HasKey(order => order.Id);

            builder.Property(order => order.MarketplaceOrderNumber)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(order => order.MarketplaceOrderNumber);

            builder.Property(order => order.MarketplaceShipmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(order => order.MarketplaceShipmentId)
                .IsUnique();

            builder.Property(order => order.GrossAmount).HasPrecision(18, 2);
            builder.Property(order => order.TotalDiscount).HasPrecision(18, 2);
            builder.Property(order => order.PlatformCoveredDiscount).HasPrecision(18, 2);

            builder.Property(order => order.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(order => order.CargoTrackingNumber)
                .HasMaxLength(64);

            builder.HasIndex(order => order.CargoTrackingNumber);

            builder.Property(order => order.CargoProviderName)
                .HasMaxLength(64);

            builder.Property(order => order.Status).IsRequired();

            builder.HasIndex(order => order.OrderDate);

            builder.HasOne(order => order.MarketplaceAccount)
                .WithMany()
                .HasForeignKey(order => order.MarketplaceAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(order => order.OrderItems)
                .WithOne(orderItem => orderItem.Order)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
