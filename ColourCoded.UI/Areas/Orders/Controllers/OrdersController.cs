using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Orders.Models.ResponseModels;
using ColourCoded.UI.Areas.Orders.Models.InputModels;
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
      return View("OrderDetail");
    }

    public ViewResult OrderCustomer(int orderId, string orderNo)
    {
      return View("OrderCustomer");
    }

    public IActionResult ConfirmOrderDetail(int orderId)
    {
      try
      {
        var result = WebApiCaller.PostAsync<OrderDetailModel>("WebApi:Orders:GetOrderDetail", new GetOrderDetailRequestModel { OrderId = orderId });

        return View("ConfirmOrderDetail", result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    public IActionResult ConfirmOrderCustomer(OrderCustomerModel model)
    {
      try
      {
        var result = WebApiCaller.PostAsync<OrderCustomerDetailModel>("WebApi:Orders:GetOrderCustomerDetail", new GetOrderCustomerDetailRequestModel { OrderId = model.OrderId });

        return View("ConfirmOrderCustomer", result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    [HttpGet] // edit mode usage
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
      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:GetOrderNoSeed", new GetCompanyOrderNoSeedRequestModel { CompanyProfileId = CurrentUser.CompanyProfileId });

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddOrder(string orderNo)
    {
      var result = WebApiCaller.PostAsync<int>("WebApi:Orders:AddOrder", new AddOrderRequestModel { OrderNo = orderNo, Username = CurrentUser.Username, CompanyProfileId = CurrentUser.CompanyProfileId });

      return Json(result);
    }

    [HttpPost]
    public JsonResult EditOrderNo(int orderId, string orderNo)
    {
      var result = WebApiCaller.PostAsync<string>("WebApi:Orders:EditOrderNo", new EditOrderNoRequestModel { OrderId = orderId, OrderNo = orderNo, Username = CurrentUser.Username });

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddOrderDetail(int orderId, List<OrderDetailInputModel> inputModel)
    {
      // get max line number for orderId
      var lineNo = WebApiCaller.PostAsync<int>("WebApi:Orders:GetOrderDetailLineNo", new GetOrderDetailLineNoRequestModel { OrderId = orderId });

      var requestModel = new List<AddOrderDetailRequestModel>();

      foreach (var model in inputModel)
      {
        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = lineNo,
          OrderId = orderId,
          ItemDescription = model.Product,
          UnitPrice = model.UnitPrice,
          Quantity = model.Quantity,
          Discount = model.Discount,
          LineTotal = model.LineTotal,
          Username = CurrentUser.Username
        }); 
      }

      var validationResult = WebApiCaller.PostAsync<ValidationResult>("WebApi:Orders:AddOrderDetail", requestModel);

      return Json(validationResult);
    }

    [HttpPost]
    public JsonResult GetOrderCustomers()
    {
      var result = WebApiCaller.PostAsync<List<CustomerModel>>("WebApi:Orders:GetOrderCustomers", new GetOrderCustomersRequestModel { CompanyProfileId = CurrentUser.CompanyProfileId, Username = CurrentUser.Username});

      return Json(result);
    }

    [HttpPost]
    public JsonResult GetCustomerContacts(int customerId)
    {
      var result = WebApiCaller.PostAsync<List<ContactModel>>("WebApi:Orders:GetCustomerContacts", new GetCustomerContactsRequestModel() { CustomerId = customerId });

      return Json(result);
    }

    [HttpPost]
    public JsonResult AddCustomerOrder(AddOrderCustomerRequestModel inputModel)
    {
      var result = WebApiCaller.PostAsync<OrderCustomerModel>("WebApi:Orders:AddOrderCustomer", inputModel);

      return Json(result);
    }
  }
}