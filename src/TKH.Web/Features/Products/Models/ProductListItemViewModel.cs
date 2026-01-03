using System.ComponentModel.DataAnnotations;

namespace TKH.Web.Features.Products.Models
{
    public record ProductListItemViewModel
    {
        public int Id { get; init; }

        [Display(Name = "Ürün Adı")]
        public string Name { get; init; } = string.Empty;

        [Display(Name = "Görsel")]
        public string ImageUrl { get; init; } = string.Empty;

        [Display(Name = "Stok Kodu (SKU)")]
        public string Sku { get; init; } = string.Empty;

        [Display(Name = "Barkod")]
        public string Barcode { get; init; } = string.Empty;

        [Display(Name = "Pazaryeri Linki")]
        public string ExternalUrl { get; set; } = string.Empty;

        [Display(Name = "Model Kodu")]
        public string ModelCode { get; init; } = string.Empty;

        [Display(Name = "Kategori")]
        public string CategoryName { get; init; } = string.Empty;

        [Display(Name = "Pazaryeri")]
        public string MarketplaceName { get; init; } = string.Empty;

        [Display(Name = "Satış Fiyatı")]
        public decimal SellingPrice { get; init; }

        [Display(Name = "Liste Fiyatı")]
        public decimal ListPrice { get; init; }

        [Display(Name = "Stok Adedi")]
        public int StockQuantity { get; init; }

        [Display(Name = "Varyant Özeti")]
        public string VariantSummary { get; init; } = string.Empty;

        [Display(Name = "Satışta")]
        public bool IsOnSale { get; init; }

        [Display(Name = "Onaylı")]
        public bool IsApproved { get; init; }

        [Display(Name = "Kilitli")]
        public bool IsLocked { get; init; }

        public string StockStatusClass => StockQuantity switch
        {
            0 => "danger",
            < 10 => "warning",
            _ => "success"
        };
    }
}
