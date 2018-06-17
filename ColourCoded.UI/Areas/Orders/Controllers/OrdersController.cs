using Microsoft.AspNetCore.Mvc;

namespace ColourCoded.UI.Areas.Orders.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}