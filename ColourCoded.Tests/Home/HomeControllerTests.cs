using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Home.Controllers;
using ColourCoded.UI.Areas.Home.Models;
using ColourCoded.UI.Areas.Orders.Models.RequestModels;
using ColourCoded.UI.Areas.Orders.Models.ResponseModels;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rotativa.AspNetCore;

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

    [TestMethod]
    public void DownloadOrderQuotation()
    {
      // given
      var resources = new Resources();
      const string orderNo = "TEST123";
      const int orderId = 123;
      var responseModel = new OrderQuotationViewModel
      {
        CustomerDetail = new OrderCustomerDetailModel
        {
          OrderId = orderId,
          CustomerName = "Test Costume",
          CustomerDetails = "This is some long customer description",
          CustomerContactNo = "0214472215",
          CustomerAccountNo = "DC1122",
          CustomerMobileNo = "0728543333",
          CustomerEmailAddress = "someemail@gmail.com",
          ContactAdded = false,
          ContactName = "Contraption",
          ContactNo = "0214472215",
          ContactEmailAddress = "someemail@gmail.com",
        },
        DeliveryAddress = new AddressDetailsModel
        {
          AddressDetailId = 1,
          AddressType = "Work",
          AddressLine1 = "24 Victoria Street",
          AddressLine2 = "Muizenberg",
          City = "Cape Town",
          PostalCode = "7786",
          Country = "RSA",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        OrderTotals = new OrderDetailModel
        {
          OrderId = orderId,
          OrderNo = "CCoT" + orderId.ToString(),
          CreateDate = DateTime.Now,
          SubTotal = 800M,
          VatTotal = 120M,
          Total = 920M,
          Discount = 0M,
          OrderLineDetails = new List<OrderLineDetailModel>
          {
            new OrderLineDetailModel
            {
              OrderId = orderId,
              ItemDescription = "TestProduct",
              UnitPrice = 100M,
              Quantity = 2,
              Discount = 0M,
              LineTotal = 200M
            },
            new OrderLineDetailModel
            {
              OrderId = orderId,
              ItemDescription = "AnotherProductName",
              UnitPrice = 300M,
              Quantity = 2,
              Discount = 0M,
              LineTotal = 600M
            },
          }
        },
        CompanyProfile = new CompanyProfileModel
        {
          AddressLine1 = "24 Victoria Street",
          AddressLine2 = "Muizenberg",
          City = "Cape Town",
          PostalCode = "7786",
          Country = "RSA",
          EmailAddress = "someemail@gmail.com",
          DisplayName = "A Nice Company Name",
          RegistrationNo = "2018/2378945",
          TaxReferenceNo = "819823",
          TelephoneNo = "0217123344",
          BankingDetails = new BankingDetailsModel
          {
            AccountNo = "4075896644",
            AccountHolder = "Some Account Holder",
            AccountType = "Cheque",
            BankName = "ABSA",
            BranchCode = "632005"
          }
        }
      };

      var requestModel = new GetOrderQuoteRequestModel { OrderId = orderId, CompanyProfileId = 1 };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderQuote", requestModel, responseModel);

      // when
      var result = resources.Controller.DownloadOrder(orderId, orderNo) as ViewAsPdf;

      // then
      Assert.IsNotNull(result);
      var model = (OrderQuotationViewModel) result.Model;
      Assert.IsNotNull(model);
      Assert.AreEqual("OrderQuotation", result.ViewName);
      Assert.AreEqual(model.CompanyProfile, responseModel.CompanyProfile);
      Assert.AreEqual(model.CustomerDetail, responseModel.CustomerDetail);
      Assert.AreEqual(model.DeliveryAddress, responseModel.DeliveryAddress);
      Assert.AreEqual(model.OrderTotals, responseModel.OrderTotals);
    }
  }
}
