using Microsoft.AspNetCore.Mvc;

namespace TKH.Presentation.Controllers
{
    public class ToolsController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> ProductProfitCalculator()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DimensionalWeightCalculator()
        {
            return View();
        }
    }
}
