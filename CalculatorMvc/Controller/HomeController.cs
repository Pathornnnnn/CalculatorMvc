using Microsoft.AspNetCore.Mvc;

namespace CalculatorMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
