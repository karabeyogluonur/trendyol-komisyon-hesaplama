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

            builder.Property(financialTransaction => financialTransaction.ExternalTransactionId).IsRequired().HasMaxLength(100);
            builder.Property(financialTransaction => financialTransaction.ExternalOrderNumber).HasMaxLength(100);
            builder.Property(financialTransaction => financialTransaction.ExternalTransactionType).HasMaxLength(100);
            builder.Property(financialTransaction => financialTransaction.Description).HasMaxLength(1000);
            builder.Property(financialTransaction => financialTransaction.Title).HasMaxLength(250);

            builder.Property(financialTransaction => financialTransaction.Amount).HasPrecision(18, 2);

            builder.HasIndex(financialTransaction => financialTransaction.TransactionDate);
            builder.HasIndex(financialTransaction => financialTransaction.ExternalTransactionId);
            builder.HasIndex(financialTransaction => financialTransaction.ExternalOrderNumber);

            builder.HasOne(financialTransaction => financialTransaction.MarketplaceAccount)
                   .WithMany()
                   .HasForeignKey(financialTransaction => financialTransaction.MarketplaceAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
