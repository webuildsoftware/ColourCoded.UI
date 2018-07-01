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
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true, CompanyProfileId = 1 });
        Controller = new HomeController(MockApiCaller, MockCookieHelper.Object);
      }
    }

    [TestMethod]
    public void LoadIndex_NoOrders()
    {
      // Given
      var resources = new Resources();
      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetHomeOrders", new GetHomeOrdersRequestModel { Username = resources.TestUsername, CompanyProfileId = 1 }, new List<HomeOrdersModel>());

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
      var orders = new List<HomeOrdersModel>
      {
        new HomeOrdersModel
        {
          OrderId = 1,
          OrderNo = "Moq001",
          Total = "R 2 999.99",
          Status = "Accepted"
        },
        new HomeOrdersModel
        {
          OrderId = 1,
          OrderNo = "Moq001",
          Total = "R 2 999.99",
          Status = "N/A"
        },
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetHomeOrders", new GetHomeOrdersRequestModel { Username = resources.TestUsername, CompanyProfileId = 1 }, orders);

      // When
      var result = resources.Controller.Index() as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(2, model.Orders.Count);
      Assert.AreEqual(orders[0].OrderNo, model.Orders[0].OrderNo);
      Assert.AreEqual(orders[0].Total, model.Orders[0].Total);
    }

    [TestMethod]
    public void GetHomeOrdersInPeriod_Success()
    {
      // Given
      var resources = new Resources();
      var orders = new List<HomeOrdersModel>
      {
        new HomeOrdersModel
        {
          OrderId = 1,
          OrderNo = "Moq001",
          Total = "R 2 999.99",
          Status = "Status"
        }
      };

      var startDate = DateTime.Now.AddMonths(-1);
      var endDate = DateTime.Now;

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetHomeOrdersInPeriod", new GetHomeOrdersPeriodRequestModel { Username = resources.TestUsername, CompanyProfileId = 1, StartDate = startDate, EndDate = endDate}, orders);

      // When
      var result = resources.Controller.GetHomeOrdersByPeriod(startDate, endDate) as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(1, model.Orders.Count);
      Assert.AreEqual(orders[0].OrderNo, model.Orders[0].OrderNo);
      Assert.AreEqual(orders[0].Total, model.Orders[0].Total);
    }
  }
}
