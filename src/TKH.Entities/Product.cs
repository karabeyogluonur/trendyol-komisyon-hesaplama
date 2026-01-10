using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Product : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        #region Properties

        public string ExternalId { get; private set; } = string.Empty;
        public string ExternalProductCode { get; private set; } = string.Empty;
        public string ExternalUrl { get; private set; } = string.Empty;

        public string Barcode { get; private set; } = string.Empty;
        public string Sku { get; private set; } = string.Empty;
        public string ModelCode { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string ImageUrl { get; private set; } = string.Empty;

        public double Deci { get; private set; }
        public decimal VatRate { get; private set; }
        public int StockQuantity { get; private set; }

        public ProductUnitType UnitType { get; private set; }

        public bool IsOnSale { get; private set; }
        public bool IsApproved { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsArchived { get; private set; }

        public DateTime LastUpdateDateTime { get; private set; }

        public int MarketplaceAccountId { get; private set; }
        public int? CategoryId { get; private set; }

        public virtual MarketplaceAccount MarketplaceAccount { get; private set; }
        public virtual Category? Category { get; private set; }

        public virtual ICollection<ProductAttribute> Attributes { get; private set; } = new List<ProductAttribute>();
        public virtual ICollection<ProductPrice> Prices { get; private set; } = new List<ProductPrice>();
        public virtual ICollection<ProductExpense> Expenses { get; private set; } = new List<ProductExpense>();
        public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

        #endregion

        #region Interface Implementation

        int IHasMarketplaceAccount.MarketplaceAccountId
        {
            get => MarketplaceAccountId;
            set => MarketplaceAccountId = value;
        }

        #endregion

        #region Ctor

        protected Product()
        {
            UnitType = ProductUnitType.Piece;
        }

        #endregion

        #region Factory

        public static Product Create(
            int marketplaceAccountId,
            string externalId,
            string externalProductCode,
            string name,
            string barcode,
            string sku,
            string modelCode,
            string externalUrl,
            string imageUrl,
            double deci,
            decimal vatRate,
            int stockQuantity,
            bool isOnSale,
            int? categoryId)
        {
            return new Product
            {
                MarketplaceAccountId = marketplaceAccountId,
                ExternalId = externalId,
                ExternalProductCode = externalProductCode,
                Name = name,
                Barcode = barcode,
                Sku = sku,
                ModelCode = modelCode,
                ExternalUrl = externalUrl,
                ImageUrl = imageUrl,
                Deci = deci,
                VatRate = vatRate,
                StockQuantity = stockQuantity,
                IsOnSale = isOnSale,
                CategoryId = categoryId,

                UnitType = ProductUnitType.Piece,
                IsApproved = true, // Default varsayım
                IsLocked = false,
                IsArchived = false,
                LastUpdateDateTime = DateTime.UtcNow
            };
        }

        #endregion

        #region Update Methods

        public void UpdateGeneralInfo(
            string externalProductCode,
            string name,
            string barcode,
            string sku,
            string modelCode,
            string externalUrl,
            string imageUrl,
            double deci,
            decimal vatRate,
            int stockQuantity,
            bool isOnSale,
            DateTime lastUpdateDateTime)
        {
            ExternalProductCode = externalProductCode;
            Name = name;
            Barcode = barcode;
            Sku = sku;
            ModelCode = modelCode;
            ExternalUrl = externalUrl;
            ImageUrl = imageUrl;
            Deci = deci;
            VatRate = vatRate;
            StockQuantity = stockQuantity;
            IsOnSale = isOnSale;
            LastUpdateDateTime = lastUpdateDateTime;
        }

        public void UpdateCategory(int? categoryId)
        {
            if (CategoryId == categoryId)
                return;

            CategoryId = categoryId;
        }

        #endregion

        #region Collection Behaviors (Rich Domain)

        public void AddOrUpdatePrice(ProductPriceType productPriceType, decimal? amount, bool isVatIncluded)
        {
            ProductPrice? activeProductPrice = Prices.FirstOrDefault(price => price.Type == productPriceType && price.IsActive());

            if (!amount.HasValue)
            {
                activeProductPrice?.MarkAsExpired();
                return;
            }

            if (activeProductPrice is not null)
            {
                if (!activeProductPrice.ShouldUpdate(amount.Value))
                    return;

                activeProductPrice.MarkAsExpired();
            }

            ProductPrice newProductPrice = ProductPrice.Create(this.Id, productPriceType, amount.Value, isVatIncluded);
            Prices.Add(newProductPrice);
        }

        public void AddOrUpdateExpense(
            ProductExpenseType productExpenseType,
            GenerationType generationType,
            decimal? amount,
            decimal vatRate,
            bool isVatIncluded)
        {
            ProductExpense? activeProductExpense = Expenses.FirstOrDefault(expense =>
                expense.Type == productExpenseType &&
                expense.EndDate == null &&
                expense.GenerationType == generationType);

            if (!amount.HasValue)
            {
                activeProductExpense?.MarkAsEnded(DateTime.UtcNow);
                return;
            }

            if (activeProductExpense is not null)
            {
                if (!activeProductExpense.IsDifferent(amount.Value, vatRate, isVatIncluded))
                    return;

                activeProductExpense.MarkAsEnded(DateTime.UtcNow);
            }

            ProductExpense newProductExpense = ProductExpense.Create(
                this.Id,
                productExpenseType,
                amount.Value,
                generationType,
                vatRate,
                isVatIncluded
            );

            Expenses.Add(newProductExpense);
        }

        public void AddOrUpdateAttribute(int categoryAttributeId, int? attributeValueId, string? customValue)
        {
            ProductAttribute? existingProductAttribute = Attributes.FirstOrDefault(attribute => attribute.CategoryAttributeId == categoryAttributeId);

            if (existingProductAttribute is not null)
                existingProductAttribute.UpdateValue(attributeValueId, customValue);
            else
            {
                ProductAttribute newProductAttribute = ProductAttribute.Create(this.Id, categoryAttributeId, attributeValueId, customValue);
                Attributes.Add(newProductAttribute);
            }
        }

        #endregion
    }
}
