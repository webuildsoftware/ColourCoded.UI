using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ColourCoded.UI.Areas.Home.Models;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Areas.Security.Models.Login;
using System;
using System.Collections.Generic;

namespace ColourCoded.UI.Areas.Home.Controllers
{
  [Area("Home")]
  public class HomeController : Controller
  {
    protected IWebApiCaller WebApiCaller;
    protected ICookieHelper CookieHelper;

    public HomeController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper)
    {
      WebApiCaller = webApiCaller;
      CookieHelper = cookieHelper;
    }

    [Authorize]
    public IActionResult Index()
    {
      try
      {
        var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");

        var result = WebApiCaller.PostAsync<List<HomeOrdersModel>>("WebApi:Home:GetUserOrders", new FindUserOrdersRequestModel { Username = loggedInUser.Username });

        return View("Index", new HomeViewModel { Orders = result });
      }
      catch (Exception Ex)
      {
        return RedirectToAction("Error", new GlobalErrorModel(Ex.Message, Ex.StackTrace, Ex.GetBaseException().Message));
      }
    }

    public ViewResult Error(GlobalErrorModel webApiErrorModel)
    {
      return View(webApiErrorModel);
    }
  }
}