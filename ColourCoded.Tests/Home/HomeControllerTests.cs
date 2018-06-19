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
        Controller = new HomeController(MockApiCaller, MockCookieHelper.Object);
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });
      }
    }

    [TestMethod]
    public void LoadIndex_NoOrders()
    {
      // Given
      var resources = new Resources();
      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetUserOrders", new FindUserOrdersRequestModel { Username = resources.TestUsername }, new HomeViewModel());


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
      var viewModel = new HomeViewModel
      {
        Orders = new List<HomeOrdersModel>
          {
            new HomeOrdersModel
            {
              CustomerName = "Test Customer", OrderId = 1, OrderNo = "Moq001", DeliveryDate = DateTime.Now.ToShortDateString(), Total = "R 2 999.99"
            }
          }
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetUserOrders", new FindUserOrdersRequestModel { Username = resources.TestUsername }, viewModel);

      // When
      var result = resources.Controller.Index() as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);

      var model = (HomeViewModel)result.Model;
      Assert.AreEqual(1, model.Orders.Count);
      Assert.AreEqual(viewModel.Orders[0].OrderNo, model.Orders[0].OrderNo);
      Assert.AreEqual(viewModel.Orders[0].CustomerName, model.Orders[0].CustomerName);
      Assert.AreEqual(viewModel.Orders[0].DeliveryDate, model.Orders[0].DeliveryDate);
      Assert.AreEqual(viewModel.Orders[0].Total, model.Orders[0].Total);
    }
  }
}
