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
    protected UserModel CurrentUser;

    public HomeController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper)
    {
      WebApiCaller = webApiCaller;
      CookieHelper = cookieHelper;
      CurrentUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
    }

    [Authorize]
    public IActionResult Index()
    {
      try
      {
        var orders = WebApiCaller.PostAsync<List<HomeOrdersModel>>("WebApi:Home:GetHomeOrders", new GetHomeOrdersRequestModel { Username = CurrentUser.Username, CompanyProfileId = CurrentUser.CompanyProfileId });

        return View("Index", new HomeViewModel { Orders = orders });
      }
      catch (Exception Ex)
      {
        return RedirectToAction("Error", new GlobalErrorModel(Ex.Message, Ex.StackTrace, Ex.GetBaseException().Message));
      }
    }

    [Authorize]
    [HttpPost]
    public IActionResult GetHomeOrdersByPeriod(DateTime startDate, DateTime endDate)
    {
      try
      {
        var result = WebApiCaller.PostAsync<List<HomeOrdersModel>>("WebApi:Home:GetHomeOrdersInPeriod", new GetHomeOrdersPeriodRequestModel { Username = CurrentUser.Username, CompanyProfileId = CurrentUser.CompanyProfileId, StartDate = startDate, EndDate = endDate });

        return View("Index", new HomeViewModel { Orders = result, StartDate = startDate, EndDate = endDate });
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