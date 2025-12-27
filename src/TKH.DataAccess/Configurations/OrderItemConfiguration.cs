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

            builder.HasKey(orderItem => orderItem.Id);

            builder.Property(orderItem => orderItem.Barcode)
                .HasMaxLength(64);

            builder.Property(orderItem => orderItem.MarketplaceSku)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(orderItem => orderItem.MarketplaceSku);

            builder.Property(orderItem => orderItem.Quantity).IsRequired();

            builder.Property(orderItem => orderItem.UnitPrice).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.VatRate).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.CommissionRate).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.PlatformCoveredDiscount).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.SellerCoveredDiscount).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.OrderItemStatus).IsRequired();

            builder.HasOne(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(orderItem => orderItem.Product)
                .WithMany(product => product.OrderItems)
                .HasForeignKey(orderItem => orderItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
