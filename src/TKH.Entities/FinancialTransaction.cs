using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class FinancialTransaction : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        #region Properties

        public int MarketplaceAccountId { get; private set; }

        public string ExternalTransactionId { get; private set; } = string.Empty;
        public string? ExternalOrderNumber { get; private set; }

        public FinancialTransactionType TransactionType { get; private set; }
        public string ExternalTransactionType { get; private set; } = string.Empty;

        public DateTime TransactionDate { get; private set; }
        public decimal Amount { get; private set; }

        public string? Description { get; private set; }
        public string? Title { get; private set; }
        public decimal? CommissionRate { get; private set; }

        public ShipmentTransactionSyncStatus ShipmentTransactionSyncStatus { get; private set; }

        public virtual MarketplaceAccount MarketplaceAccount { get; private set; }

        #endregion

        #region Interface Implementation

        int IHasMarketplaceAccount.MarketplaceAccountId
        {
            get => MarketplaceAccountId;
            set => MarketplaceAccountId = value;
        }

        #endregion

        #region Ctor

        protected FinancialTransaction()
        {
            ShipmentTransactionSyncStatus = ShipmentTransactionSyncStatus.Pending;
        }

        #endregion

        #region Factory

        public static FinancialTransaction Create(
            int marketplaceAccountId,
            string externalTransactionId,
            string? externalOrderNumber,
            FinancialTransactionType transactionType,
            string externalTransactionType,
            DateTime transactionDate,
            decimal amount,
            string? description,
            string? title,
            decimal? commissionRate)
        {
            return new FinancialTransaction
            {
                MarketplaceAccountId = marketplaceAccountId,
                ExternalTransactionId = externalTransactionId,
                ExternalOrderNumber = externalOrderNumber,
                TransactionType = transactionType,
                ExternalTransactionType = externalTransactionType,
                TransactionDate = transactionDate,
                Amount = amount,
                Description = description,
                Title = title,
                CommissionRate = commissionRate,
                ShipmentTransactionSyncStatus = ShipmentTransactionSyncStatus.Pending
            };
        }

        public void UpdateDetails(
            string? externalOrderNumber,
            FinancialTransactionType transactionType,
            string externalTransactionType,
            DateTime transactionDate,
            decimal amount,
            string? description,
            string? title,
            decimal? commissionRate)
        {
            if (ExternalOrderNumber == externalOrderNumber &&
                TransactionType == transactionType &&
                ExternalTransactionType == externalTransactionType &&
                TransactionDate == transactionDate &&
                Amount == amount &&
                Description == description &&
                Title == title &&
                CommissionRate == commissionRate)
            {
                return;
            }

            ExternalOrderNumber = externalOrderNumber;
            TransactionType = transactionType;
            ExternalTransactionType = externalTransactionType;
            TransactionDate = transactionDate;
            Amount = amount;
            Description = description;
            Title = title;
            CommissionRate = commissionRate;
        }

        #endregion

        #region Behavior

        public void UpdateShipmentSyncStatus(ShipmentTransactionSyncStatus newShipmentTransactionSyncStatus)
        {
            if (ShipmentTransactionSyncStatus == newShipmentTransactionSyncStatus)
                return;

            ShipmentTransactionSyncStatus = newShipmentTransactionSyncStatus;
        }

        #endregion
    }
}
