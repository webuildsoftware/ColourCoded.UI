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

    public ViewResult OrderDetail(int orderId)
    {
      return View();
    }

    public ViewResult ConfirmOrderDetail(int orderId)
    {
      var result = WebApiCaller.PostAsync<OrderDetailModel>("WebApi:Orders:GetOrderDetail", new GetOrderDetailRequestModel { OrderId = orderId });

      return View("ConfirmOrderDetail", result);
    }

    [HttpGet]
    public JsonResult GetOrderLineDetails(int orderId)
    {
      var result = WebApiCaller.PostAsync<List<OrderLineDetailModel>>("WebApi:Orders:GetOrderLineDetails", new GetOrderLineDetailsRequestModel { OrderId = orderId });

      return Json(result);
    }

    [HttpGet]
    public JsonResult GetVatRate()
    {
      var result = WebApiCaller.PostAsync<decimal>("WebApi:Orders:GetVatRate", null);

      return Json(result);
    }

    [HttpGet]
    public JsonResult GetOrderNoSeed()
    {
      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:GetOrderNoSeed", null);

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddOrder(string orderNo, int orderId)
    {
      if (orderId != 0)
        return Json(orderId);

      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");

      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:AddOrder", new AddOrderRequestModel { OrderNo = orderNo, Username = loggedInUser.Username });

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddOrderDetail(int orderId, decimal vatRate, List<OrderDetailInputModel> inputModel)
    {
      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");

      var validationResult = new ValidationResult();

      // get max line number for orderId
      var lineNo = WebApiCaller.PostAsync<int>("WebApi:Orders:GetOrderDetailLineNo", new GetOrderDetailLineNoRequestModel { OrderId = orderId });

      foreach (var model in inputModel)
      {
        validationResult = WebApiCaller.PostAsync<ValidationResult>("WebApi:Orders:AddOrderDetail", new AddOrderDetailRequestModel
        {
          LineNo = lineNo,
          OrderId = orderId,
          ItemDescription = model.Product,
          UnitPrice = model.UnitPrice,
          Quantity = model.Quantity,
          Discount = model.Discount,
          LineTotal = model.LineTotal,
          Vat = Convert.ToDecimal(model.LineTotal * vatRate),
          Username = loggedInUser.Username
        });

        if (!validationResult.IsValid)
          break;
      }

      return Json(validationResult);
    }
  }
}