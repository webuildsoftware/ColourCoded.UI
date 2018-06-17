using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ColourCoded.UI.Areas.Home.Models;
using ColourCoded.UI.Shared;

namespace ColourCoded.UI.Areas.Home.Controllers
{
  [Area("Home")]
  public class HomeController : Controller
  {
    [Authorize]
    public ViewResult Index(HomeViewModel model)
    {
      return View(model);
    }

    public ViewResult Error(GlobalErrorModel webApiErrorModel)
    {
      return View(webApiErrorModel);
    }
  }
}