using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.Property(orderItem => orderItem.Barcode).HasMaxLength(100);
            builder.Property(orderItem => orderItem.Sku).HasMaxLength(100);
            builder.Property(orderItem => orderItem.UnitPrice).HasPrecision(18, 2);
            builder.Property(orderItem => orderItem.PlatformCoveredDiscount).HasPrecision(18, 2);
            builder.Property(orderItem => orderItem.SellerCoveredDiscount).HasPrecision(18, 2);
            builder.Property(orderItem => orderItem.CommissionRate).HasPrecision(18, 2);
            builder.Property(orderItem => orderItem.VatRate).HasPrecision(18, 2);

            builder.HasOne(orderItem => orderItem.Order)
                   .WithMany(order => order.OrderItems)
                   .HasForeignKey(orderItem => orderItem.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(orderItem => orderItem.Product)
                   .WithMany(product => product.OrderItems)
                   .HasForeignKey(orderItem => orderItem.ProductId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
