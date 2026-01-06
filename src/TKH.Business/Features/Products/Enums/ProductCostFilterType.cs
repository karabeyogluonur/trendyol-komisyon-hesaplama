using System.ComponentModel.DataAnnotations;

namespace TKH.Business.Features.Products.Enums
{
    public enum ProductCostFilterType
    {
        [Display(Name = "Tümü")]
        All = 0,

        [Display(Name = "Tamamlanmışlar")]
        Completed = 1,

        [Display(Name = "Alış Fiyatı Olmayanlar")]
        MissingPurchasePrice = 10,

        [Display(Name = "Kargo Gideri Olmayanlar")]
        MissingShippingCost = 11,

        [Display(Name = "Komisyon Oranı Olmayanlar")]
        MissingCommission = 12
    }
}
