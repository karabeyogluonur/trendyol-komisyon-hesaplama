using AutoMapper;

using TKH.Business.Features.Products.Dtos;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.Products.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {

            #region Profit

            CreateMap<Product, ProductProfitSummaryDto>().ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src =>
                src.Prices.Where(productPrice => productPrice.Type == ProductPriceType.SalePrice && productPrice.EndDate == null).Select(productPrice => productPrice.Amount).FirstOrDefault()))

              .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src =>
                src.Prices.Where(productPrice => productPrice.Type == ProductPriceType.PurchasePrice && productPrice.EndDate == null).Select(productPrice => productPrice.Amount).FirstOrDefault()))

              .ForMember(dest => dest.ManualShippingCost, opt => opt.MapFrom(src =>
                src.Expenses.Where(productExpense => productExpense.Type == ProductExpenseType.ShippingCost && productExpense.EndDate == null && productExpense.GenerationType == GenerationType.Manual).Select(productExpense => productExpense.Amount)
                .FirstOrDefault()))

              .ForMember(dest => dest.AutomatedShippingCost, opt => opt.MapFrom(src =>
                src.Expenses.Where(productExpense => productExpense.Type == ProductExpenseType.ShippingCost && productExpense.EndDate == null && productExpense.GenerationType == GenerationType.Automated).Select(productExpense => productExpense.Amount)
                .FirstOrDefault()))

              .ForMember(dest => dest.ManualCommissionRate, opt => opt.MapFrom(src =>
                src.Expenses.Where(productExpense => productExpense.Type == ProductExpenseType.CommissionRate && productExpense.EndDate == null && productExpense.GenerationType == GenerationType.Manual).Select(productExpense => productExpense.Amount)
                .FirstOrDefault()))

              .ForMember(dest => dest.AutomatedCommissionRate, opt => opt.MapFrom(src =>
                src.Expenses.Where(productExpense => productExpense.Type == ProductExpenseType.CommissionRate && productExpense.EndDate == null && productExpense.GenerationType == GenerationType.Automated).Select(productExpense => productExpense.Amount)
                .FirstOrDefault()))

              .ForMember(dest => dest.ServiceFee, opt => opt.MapFrom(src =>
                src.Expenses.Where(productExpense => productExpense.Type == ProductExpenseType.MarketplaceServiceFee && productExpense.EndDate == null)
                .Select(productExpense => productExpense.Amount)
                .FirstOrDefault()));

            #endregion

            #region Cost

            CreateMap<Product, ProductCostSummaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
                .ForMember(dest => dest.ModelCode, opt => opt.MapFrom(src => src.ModelCode))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.ExternalUrl, opt => opt.MapFrom(src => src.ExternalUrl))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.Prices
                    .Where(product => product.Type == ProductPriceType.PurchasePrice && product.EndDate == null)
                    .Select(product => product.Amount)
                    .FirstOrDefault()))
                .ForMember(dest => dest.ManualCommissionRate, opt => opt.MapFrom(src => src.Expenses
                    .Where(product => product.Type == ProductExpenseType.CommissionRate && product.GenerationType == GenerationType.Manual && product.EndDate == null)
                    .Select(product => product.Amount)
                    .FirstOrDefault()))

                .ForMember(dest => dest.AutomatedCommissionRate, opt => opt.MapFrom(src => src.Expenses
                    .Where(product => product.Type == ProductExpenseType.CommissionRate && product.GenerationType == GenerationType.Automated && product.EndDate == null)
                    .Select(product => product.Amount)
                    .FirstOrDefault()))

                .ForMember(dest => dest.ManualShippingCost, opt => opt.MapFrom(src => src.Expenses
                    .Where(product => product.Type == ProductExpenseType.ShippingCost && product.GenerationType == GenerationType.Manual && product.EndDate == null)
                    .Select(product => product.Amount)
                    .FirstOrDefault()))

                .ForMember(dest => dest.AutomatedShippingCost, opt => opt.MapFrom(src => src.Expenses
                    .Where(product => product.Type == ProductExpenseType.ShippingCost && product.GenerationType == GenerationType.Automated && product.EndDate == null)
                    .Select(product => product.Amount)
                    .FirstOrDefault()));


            #endregion

            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "-"))

                .ForMember(dest => dest.SellingPrice, opt => opt.MapFrom(src => src.Prices
                    .Where(price => price.Type == ProductPriceType.SalePrice)
                    .OrderByDescending(price => price.StartDate).Select(price => price.Amount).FirstOrDefault()))

                .ForMember(dest => dest.ListPrice, opt => opt.MapFrom(src => src.Prices
                    .Where(price => price.Type == ProductPriceType.ListPrice)
                    .OrderByDescending(price => price.StartDate).Select(price => price.Amount).FirstOrDefault()))

                .ForMember(dest => dest.MarketplaceType, opt => opt.MapFrom(src => src.MarketplaceAccount.MarketplaceType))

                .ForMember(dest => dest.VariantSummary, opt => opt.MapFrom(src => string.Join(" | ", src.Attributes
                    .Where(productAttribute => productAttribute.Attribute != null && productAttribute.Attribute.IsVariant)
                    .Select(productAttribute => $"{productAttribute.Attribute.Name}: {(productAttribute.Value != null ? productAttribute.Value.Value : productAttribute.CustomValue)}"))));
        }
    }
}
