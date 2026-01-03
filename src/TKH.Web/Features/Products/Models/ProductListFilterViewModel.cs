using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TKH.Core.Common.Constants;

namespace TKH.Web.Features.Products.Models
{
    public class ProductListFilterViewModel
    {
        [Display(Name = "Barkod / Ürün Adı")]
        public string? Barcode { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = ApplicationDefaults.ProductPageSize;

        [Display(Name = "Satışta Olanlar")]
        public bool? IsOnSale { get; set; }

        [Display(Name = "Stokta Olanlar")]
        public bool? HasStock { get; set; }

        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
