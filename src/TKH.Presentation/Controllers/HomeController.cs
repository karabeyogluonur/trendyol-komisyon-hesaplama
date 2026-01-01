using Microsoft.AspNetCore.Mvc;

namespace TKH.Presentation.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
