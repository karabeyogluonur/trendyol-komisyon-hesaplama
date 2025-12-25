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

            builder.Property(financialTransaction => financialTransaction.MarketplaceAccountId)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.MarketplaceTransactionId)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.OrderNumber)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(financialTransaction => financialTransaction.TransactionType)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.MarketplaceTransactionType)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(financialTransaction => financialTransaction.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.TransactionDate)
                .IsRequired();

            builder.Property(financialTransaction => financialTransaction.Title)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(financialTransaction => financialTransaction.Description)
                .HasMaxLength(int.MaxValue)
                .IsRequired(false);

            builder.HasIndex(financialTransaction => new { financialTransaction.MarketplaceAccountId, financialTransaction.MarketplaceTransactionId })
                .IsUnique();

            builder.HasIndex(financialTransaction => financialTransaction.TransactionDate);
            builder.HasIndex(financialTransaction => financialTransaction.OrderNumber);
        }
    }
}
