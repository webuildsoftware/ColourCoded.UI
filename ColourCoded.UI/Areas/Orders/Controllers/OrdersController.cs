using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Orders.Models;
using ColourCoded.UI.Areas.Orders.Models.RequestModels;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColourCoded.UI.Areas.Orders.Controllers
{
  [Area("Orders")]
  [Authorize]
  public class OrdersController : Controller
  {
    protected IWebApiCaller WebApiCaller;
    protected ICookieHelper CookieHelper;
    protected UserModel CurrentUser;

    public OrdersController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper)
    {
      WebApiCaller = webApiCaller;
      CookieHelper = cookieHelper;
      CurrentUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
    }

    public IActionResult OrderDetail()
    {
      return View();
    }

    public JsonResult GetVatRate()
    {
      var result = WebApiCaller.PostAsync<decimal>("WebApi:Orders:GetVatRate", null);

      return Json(result);
    }

    public JsonResult GetOrderNoSeed()
    {
      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:GetOrderNoSeed", null);

      return Json(result);
    }

    public JsonResult AddOrder(string orderNo)
    {
      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:AddOrder", new AddOrderRequestModel { OrderNo = orderNo });

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddOrderDetail(int orderId, decimal vatRate, List<OrderDetailInputModel> inputModel)
    {
      var validationResult = new ValidationResult();

      foreach (var model in inputModel)
      {
        validationResult = WebApiCaller.PostAsync<ValidationResult>("WebApi:Orders:AddOrderDetail", new AddOrderDetailRequestModel
        {
          OrderId = orderId,
          ItemDescription = model.Product,
          UnitPrice = model.UnitPrice,
          Quantity = model.Quantity,
          Discount = model.Discount,
          LineTotal = model.LineTotal,
          Vat = Convert.ToDecimal(model.LineTotal * vatRate)
        });

        if (!validationResult.IsValid)
          break;
      }

      return Json(validationResult);
    }
  }
}