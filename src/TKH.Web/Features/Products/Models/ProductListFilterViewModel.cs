using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Core.Common.Constants;

namespace TKH.Web.Features.Products.Models
{
    public class ProductListFilterViewModel
    {
        [FromQuery(Name = "Barcode")]
        [Display(Name = "Barkod / Ürün Adı")]
        public string? Barcode { get; set; }

        [FromQuery(Name = "PageIndex")]
        public int PageIndex { get; set; } = 1;

        [FromQuery(Name = "PageSize")]
        public int PageSize { get; set; } = ApplicationDefaults.ProductPageSize;

        [FromQuery(Name = "IsOnSale")]
        [Display(Name = "Satış Durumu")]
        public bool? IsOnSale { get; set; }

        [FromQuery(Name = "HasStock")]
        [Display(Name = "Stok Durumu")]
        public bool? HasStock { get; set; }

        [FromQuery(Name = "CategoryId")]
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}
