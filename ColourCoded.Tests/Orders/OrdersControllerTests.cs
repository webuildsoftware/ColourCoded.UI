using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Orders.Controllers;
using ColourCoded.UI.Areas.Orders.Models.RequestModels;
using ColourCoded.UI.Areas.Orders.Models;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ColourCoded.Tests.Orders
{
  [TestClass]
  public class OrdersControllerTests
  {
    private class Resources
    {
      public string TestUsername { get; set; }
      public OrdersController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;

      public Resources()
      {
        TestUsername = "testuser";
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });
        Controller = new OrdersController(MockApiCaller, MockCookieHelper.Object);
      }
    }

    [TestMethod]
    public void GetVatRate_ReturnsDecimal()
    {
      // given
      var resources = new Resources();
      const decimal vatRate = 0.15M;
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetVatRate", null, vatRate);

      // when 
      var result = resources.Controller.GetVatRate() as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual(vatRate, result.Value);
    }

    [TestMethod]
    public void GetVatRate_ReturnsInt()
    {
      // given
      var resources = new Resources();
      const int orderNoSeed = 152;
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderNoSeed", null, orderNoSeed);

      // when 
      var result = resources.Controller.GetOrderNoSeed() as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual(orderNoSeed, result.Value);
    }

    [TestMethod]
    public void AddOrder_ReturnsOrderIdInt()
    {
      // given
      var resources = new Resources();
      const string orderNo = "TestQuote123";
      const int orderId = 101010;
      var requestModel = new AddOrderRequestModel { OrderNo = orderNo };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrder", requestModel, orderId);

      // when 
      var result = resources.Controller.AddOrder(orderNo) as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual(orderId, result.Value);
    }

    [TestMethod]
    public void AddOrderDetail_ReturnsValidationResult_Success()
    {
      // given
      var resources = new Resources();
      const int orderId = 101010;
      const decimal vatRate = 0.15M;

      var inputModel = new List<OrderDetailInputModel> 
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = 100M,
          Quantity = 10M,
          Discount = 100M,
          LineTotal = 900M
        },
        new OrderDetailInputModel
        {
          Product = "Delivery Fee",
          UnitPrice = 80M,
          Quantity = 1M,
          Discount = 0M,
          LineTotal = 80M
        }
      };

      foreach (var model in inputModel)
      {
        var requestModel = new AddOrderDetailRequestModel
        {
          OrderId = orderId,
          ItemDescription = model.Product,
          UnitPrice = model.UnitPrice,
          Quantity = model.Quantity,
          Discount = model.Discount,
          LineTotal = model.LineTotal,
          Vat = Convert.ToDecimal(model.LineTotal * vatRate)
        };

        resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, new ValidationResult());
      }
      
      // when 
      var result = resources.Controller.AddOrderDetail(orderId, vatRate, inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var resultModel = (ValidationResult)result.Value;
      Assert.IsNotNull(resultModel);
      Assert.IsTrue(resultModel.IsValid);
    }

    [TestMethod]
    public void AddOrderDetail_ReturnsValidationResult_LineTotalIncorrect()
    {
      // given
      var resources = new Resources();
      const int orderId = 101010;
      const decimal vatRate = 0.15M;

      var inputModel = new List<OrderDetailInputModel>
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = 100M,
          Quantity = 10M,
          Discount = 100M,
          LineTotal = 0M
        }
      };

      var requestModel = new AddOrderDetailRequestModel
      {
        OrderId = orderId,
        ItemDescription = inputModel[0].Product,
        UnitPrice = inputModel[0].UnitPrice,
        Quantity = inputModel[0].Quantity,
        Discount = inputModel[0].Discount,
        LineTotal = inputModel[0].LineTotal,
        Vat = Convert.ToDecimal(inputModel[0].LineTotal * vatRate)
      };

      var validationResult = new ValidationResult();
      validationResult.InValidate("", "The line total provided is incorrect.");

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, validationResult);

      // when 
      var result = resources.Controller.AddOrderDetail(orderId, vatRate, inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var resultModel = (ValidationResult)result.Value;
      Assert.IsNotNull(resultModel);
      Assert.IsFalse(resultModel.IsValid);
    }

    [TestMethod]
    public void AddOrderDetail_ReturnsValidationResult_UnitPriceLessThanZero()
    {
      // given
      var resources = new Resources();
      const int orderId = 101010;
      const decimal vatRate = 0.15M;

      var inputModel = new List<OrderDetailInputModel>
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = -100M,
          Quantity = 10M,
          Discount = 100M,
          LineTotal = 900M
        }
      };

      var requestModel = new AddOrderDetailRequestModel
      {
        OrderId = orderId,
        ItemDescription = inputModel[0].Product,
        UnitPrice = inputModel[0].UnitPrice,
        Quantity = inputModel[0].Quantity,
        Discount = inputModel[0].Discount,
        LineTotal = inputModel[0].LineTotal,
        Vat = Convert.ToDecimal(inputModel[0].LineTotal * vatRate)
      };

      var validationResult = new ValidationResult();
      validationResult.InValidate("", "Unit price less than zero.");

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, validationResult);

      // when 
      var result = resources.Controller.AddOrderDetail(orderId, vatRate, inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var resultModel = (ValidationResult)result.Value;
      Assert.IsNotNull(resultModel);
      Assert.IsFalse(resultModel.IsValid);
    }
  }
}
