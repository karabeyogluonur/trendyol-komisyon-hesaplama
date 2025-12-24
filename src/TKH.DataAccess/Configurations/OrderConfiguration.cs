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

            builder.Property(order => order.MarketplaceOrderNumber).IsRequired().HasMaxLength(64);

            builder.HasIndex(order => order.MarketplaceOrderNumber);

            builder.Property(order => order.Amount).HasPrecision(18, 2);

            builder.Property(order => order.Status).IsRequired();

            builder.HasOne(order => order.MarketplaceAccount).WithMany().HasForeignKey(order => order.MarketplaceAccountId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
