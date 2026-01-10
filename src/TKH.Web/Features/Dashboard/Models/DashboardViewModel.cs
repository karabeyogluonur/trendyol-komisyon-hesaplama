namespace TKH.Web.Features.Dashboard.Models
{
    public class DashboardViewModel
    {
        public ProductCostReadinessWidgetViewModel ProductCostReadiness { get; set; }

        public DashboardViewModel()
        {
            ProductCostReadiness = new ProductCostReadinessWidgetViewModel();
        }
    }
}
