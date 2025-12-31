using Microsoft.AspNetCore.Mvc;

namespace TKH.Presentation.Controllers
{
    public class ToolsController : Controller
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
