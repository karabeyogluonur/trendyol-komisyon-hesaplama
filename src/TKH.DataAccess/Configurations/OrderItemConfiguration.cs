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

            builder.Property(orderItem => orderItem.Barcode).HasMaxLength(64);

            builder.Property(orderItem => orderItem.Quantity).IsRequired();

            builder.Property(orderItem => orderItem.Amount).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.MarketplaceDiscount).HasPrecision(18, 2);
            builder.Property(orderItem => orderItem.SellerDiscount).HasPrecision(18, 2);

            builder.Property(orderItem => orderItem.CommissionRate).HasPrecision(18, 2);

            builder.HasOne(orderItem => orderItem.Order)
                .WithMany(orderItem => orderItem.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(orderItem => orderItem.Product)
                .WithMany(orderItem => orderItem.OrderItems)
                .HasForeignKey(orderItem => orderItem.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
