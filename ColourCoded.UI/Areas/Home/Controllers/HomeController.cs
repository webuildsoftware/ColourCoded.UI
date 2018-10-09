using Microsoft.AspNetCore.Mvc;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;

namespace ColourCoded.UI.Areas.Home.Controllers
{
  [Area("Home")]
  public class HomeController : Controller
  {
    public HomeController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper)
    {
    }

    public ViewResult Error(GlobalErrorModel webApiErrorModel)
    {
      return View(webApiErrorModel);
    }
  }
}