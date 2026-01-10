using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Order : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        #region Properties

        public string ExternalOrderNumber { get; private set; } = string.Empty;
        public string ExternalShipmentId { get; private set; } = string.Empty;

        public int MarketplaceAccountId { get; private set; }

        public decimal GrossAmount { get; private set; }
        public decimal TotalDiscount { get; private set; }
        public decimal PlatformCoveredDiscount { get; private set; }

        public string CurrencyCode { get; private set; } = "TRY";

        public OrderStatus Status { get; private set; }

        public DateTime OrderDate { get; private set; }
        public DateTime LastUpdateDateTime { get; private set; }

        public string CargoTrackingNumber { get; private set; } = string.Empty;
        public string CargoProviderName { get; private set; } = string.Empty;

        public double Deci { get; private set; }
        public bool IsShipmentPaidBySeller { get; private set; }
        public bool IsMicroExport { get; private set; }

        public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
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

        protected Order()
        {
        }

        #endregion

        #region Factory

        public static Order Create(
            int marketplaceAccountId,
            string externalOrderNumber,
            string externalShipmentId,
            decimal grossAmount,
            decimal totalDiscount,
            decimal platformCoveredDiscount,
            string currencyCode,
            OrderStatus status,
            DateTime orderDate,
            string cargoTrackingNumber,
            string cargoProviderName,
            double deci,
            bool isShipmentPaidBySeller,
            bool isMicroExport)
        {
            return new Order
            {
                MarketplaceAccountId = marketplaceAccountId,
                ExternalOrderNumber = externalOrderNumber,
                ExternalShipmentId = externalShipmentId,
                GrossAmount = grossAmount,
                TotalDiscount = totalDiscount,
                PlatformCoveredDiscount = platformCoveredDiscount,
                CurrencyCode = currencyCode,
                Status = status,
                OrderDate = orderDate,
                LastUpdateDateTime = DateTime.UtcNow,
                CargoTrackingNumber = cargoTrackingNumber,
                CargoProviderName = cargoProviderName,
                Deci = deci,
                IsShipmentPaidBySeller = isShipmentPaidBySeller,
                IsMicroExport = isMicroExport
            };
        }

        #endregion

        #region Update Methods

        public void UpdateDetails(
            decimal grossAmount,
            decimal totalDiscount,
            decimal platformCoveredDiscount,
            OrderStatus status,
            string cargoTrackingNumber,
            string cargoProviderName,
            double deci,
            bool isShipmentPaidBySeller,
            bool isMicroExport,
            DateTime lastUpdateDateTime)
        {
            GrossAmount = grossAmount;
            TotalDiscount = totalDiscount;
            PlatformCoveredDiscount = platformCoveredDiscount;
            Status = status;
            CargoTrackingNumber = cargoTrackingNumber;
            CargoProviderName = cargoProviderName;
            Deci = deci;
            IsShipmentPaidBySeller = isShipmentPaidBySeller;
            IsMicroExport = isMicroExport;
            LastUpdateDateTime = lastUpdateDateTime;
        }

        public void SyncOrderItems(List<OrderItem> newOrderItems)
        {
            if (OrderItems.Any())
                OrderItems.Clear();

            foreach (OrderItem orderItem in newOrderItems)
                OrderItems.Add(orderItem);
        }

        #endregion
    }
}
