using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TKH.Entities;

namespace TKH.DataAccess.Configurations
{
    public class FinancialTransactionConfiguration : IEntityTypeConfiguration<FinancialTransaction>
    {
        public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
        {
            builder.ToTable("FinancialTransactions");

            builder.HasKey(financialTransaction => financialTransaction.Id);

            builder.Property(financialTransaction => financialTransaction.MarketplaceTransactionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(financialTransaction => new { financialTransaction.MarketplaceAccountId, financialTransaction.MarketplaceTransactionId })
                .IsUnique();

            builder.Property(financialTransaction => financialTransaction.OrderNumber).HasMaxLength(64);
            builder.HasIndex(financialTransaction => financialTransaction.OrderNumber);
            builder.HasIndex(financialTransaction => financialTransaction.TransactionDate);

            builder.Property(financialTransaction => financialTransaction.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.CommissionAmount)
                .HasPrecision(18, 2);

            builder.Property(financialTransaction => financialTransaction.CommissionRate)
                .HasPrecision(18, 2);

            builder.Property(financialTransaction => financialTransaction.OrderItemBarcode)
                .HasMaxLength(64);

            builder.Property(financialTransaction => financialTransaction.Description)
                .HasMaxLength(2000);

            builder.HasOne<MarketplaceAccount>()
                .WithMany()
                .HasForeignKey(financialTransaction => financialTransaction.MarketplaceAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
