using Microsoft.AspNetCore.Mvc;

namespace TKH.Presentation.Controllers
{
    public class ErrorController : BaseController
    {
        [Route("Error/500")]
        public IActionResult General()
        {
            return View();
        }

        [Route("Error/404")]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}
