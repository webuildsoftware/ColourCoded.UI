using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Home.Controllers;
using ColourCoded.UI.Areas.Home.Models;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ColourCoded.Tests.Home
{
  [TestClass]
  public class HomeControllerTests
  {
    private class Resources
    {
      public string TestUsername { get; set; }
      public HomeController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;

      public Resources()
      {
        TestUsername = "testuser";
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });
        Controller = new HomeController(MockApiCaller, MockCookieHelper.Object);
      }
    }

    [TestMethod]
    public void LoadIndex_NoOrders()
    {
      // Given
      var resources = new Resources();
      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetUserOrders", new FindUserOrdersRequestModel { Username = resources.TestUsername }, new List<HomeOrdersModel>());

      // When
      var result = resources.Controller.Index() as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(0, model.Orders.Count);
    }

    [TestMethod]
    public void LoadIndex_HasOrders()
    {
      // Given
      var resources = new Resources();
      var orders = new List<HomeOrdersModel>{ new HomeOrdersModel {
                                                                    CustomerName = "Test Customer",
                                                                    OrderId = 1,
                                                                    OrderNo = "Moq001",
                                                                    DeliveryDate = DateTime.Now.ToShortDateString(),
                                                                    Total = "R 2 999.99"
                                                                  }  };

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetUserOrders", new FindUserOrdersRequestModel { Username = resources.TestUsername }, orders);

      // When
      var result = resources.Controller.Index() as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(1, model.Orders.Count);
      Assert.AreEqual(orders[0].OrderNo, model.Orders[0].OrderNo);
      Assert.AreEqual(orders[0].CustomerName, model.Orders[0].CustomerName);
      Assert.AreEqual(orders[0].DeliveryDate, model.Orders[0].DeliveryDate);
      Assert.AreEqual(orders[0].Total, model.Orders[0].Total);
    }

    [TestMethod]
    public void GetUserOrdersInPeriod_Success()
    {
      // Given
      var resources = new Resources();
      var orders = new List<HomeOrdersModel>{ new HomeOrdersModel {
                                                                    CustomerName = "Test Customer",
                                                                    OrderId = 1,
                                                                    OrderNo = "Moq001",
                                                                    DeliveryDate = DateTime.Now.ToShortDateString(),
                                                                    Total = "R 2 999.99"
                                                                  }  };
      var startDate = DateTime.Now.AddMonths(-1);
      var endDate = DateTime.Now;

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetUserOrdersInPeriod", new FindUserOrdersPeriodRequestModel { Username = resources.TestUsername, StartDate = startDate, EndDate = endDate}, orders);

      // When
      var result = resources.Controller.GetUserOrdersByPeriod(startDate, endDate) as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(1, model.Orders.Count);
      Assert.AreEqual(orders[0].OrderNo, model.Orders[0].OrderNo);
      Assert.AreEqual(orders[0].CustomerName, model.Orders[0].CustomerName);
      Assert.AreEqual(orders[0].DeliveryDate, model.Orders[0].DeliveryDate);
      Assert.AreEqual(orders[0].Total, model.Orders[0].Total);
    }
  }
}
