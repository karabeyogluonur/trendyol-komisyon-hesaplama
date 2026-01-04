using System.ComponentModel.DataAnnotations;

namespace TKH.Web.Features.ProductProfits.Models
{
    public class ProductProfitListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Barkod")]
        public string Barcode { get; set; } = string.Empty;

        [Display(Name = "Model Kodu")]
        public string ModelCode { get; set; } = string.Empty;

        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Pazaryeri Linki")]
        public string ExternalUrl { get; set; } = string.Empty;

        [Display(Name = "Kargo Ücreti (Kullanıcı)")]
        public decimal ManualShippingCost { get; set; }

        [Display(Name = "Kargo Ücreti (Sistem)")]
        public decimal AutomatedShippingCost { get; set; }

        [Display(Name = "Stok Adedi")]
        public int StockQuantity { get; set; }

        [Display(Name = "Satış Fiyatı")]
        public decimal SalesPrice { get; set; }

        [Display(Name = "Alış Fiyatı")]
        public decimal PurchasePrice { get; set; }

        [Display(Name = "Komisyon Oranı (Kullanıcı)")]
        public decimal ManualCommissionRate { get; set; }

        [Display(Name = "Komisyon Oranı (Sistem)")]
        public decimal AutomatedCommissionRate { get; set; }

        [Display(Name = "KDV Oranı")]
        public decimal VatRate { get; set; }

        [Display(Name = "Hizmet Bedeli")]
        public decimal ServiceFee { get; set; }
    }
}
