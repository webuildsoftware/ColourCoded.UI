﻿using System;
using System.Collections.Generic;
using ColourCoded.UI.Areas.Orders.Controllers;
using ColourCoded.UI.Areas.Orders.Models.RequestModels;
using ColourCoded.UI.Areas.Orders.Models.InputModels;
using ColourCoded.UI.Areas.Orders.Models.ResponseModels;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Rotativa.AspNetCore;

namespace ColourCoded.Tests.Orders
{
  [TestClass]
  public class OrdersControllerTests
  {
    private class Resources
    {
      public string TestUsername { get; set; }
      public int CompanyProfileId { get; set; }
      public OrdersController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;

      public Resources()
      {
        TestUsername = "testuser";
        CompanyProfileId = 1;
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true, CompanyProfileId = CompanyProfileId });
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
    public void GetOrderNoSeed_HasCompany()
    {
      // given
      var resources = new Resources();
      const int orderNoSeed = 152;
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderNoSeed", new GetCompanyOrderNoSeedRequestModel { CompanyProfileId = 1 }, orderNoSeed);

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
      const string username = "testuser";
      const int orderId = 101010;
      var requestModel = new AddOrderRequestModel { OrderNo = orderNo, Username = username, CompanyProfileId = 1 };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrder", requestModel, orderId);

      // when 
      var result = resources.Controller.AddOrder(orderNo) as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual(orderId, result.Value);
    }

    [TestMethod]
    public void EditOrder_ReturnsOrderIdInt()
    {
      // given
      var resources = new Resources();
      const string orderNo = "TestQuote123";
      const int orderId = 101010;
      const string username = "testuser";
      var requestModel = new EditOrderNoRequestModel { OrderId = orderId, OrderNo = orderNo, Username = username };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:EditOrderNo", requestModel, "Success");

      // when 
      var result = resources.Controller.EditOrderNo(orderId, orderNo) as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value);
    }

    [TestMethod]
    public void AddOrderDetail_ReturnsValidationResult_Success()
    {
      // given
      var resources = new Resources();
      const int orderId = 101010;
      const decimal vatRate = 0.15M;
      const string username = "testuser";

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderDetailLineNo", new GetOrderDetailLineNoRequestModel { OrderId = orderId }, 1);

      var inputModel = new List<OrderDetailInputModel> 
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = 100M,
          Quantity = 10,
          Discount = 100M,
          LineTotal = 900M
        },
        new OrderDetailInputModel
        {
          Product = "Delivery Fee",
          UnitPrice = 80M,
          Quantity = 1,
          Discount = 0M,
          LineTotal = 80M
        }
      };

      var requestModel = new List<AddOrderDetailRequestModel>();

      foreach (var model in inputModel)
      {
        requestModel.Add(new AddOrderDetailRequestModel
        {
          LineNo = 1,
          OrderId = orderId,
          ItemDescription = model.Product,
          UnitPrice = model.UnitPrice,
          Quantity = model.Quantity,
          Discount = model.Discount,
          LineTotal = model.LineTotal,
          Username = username
        });
      }

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, new ValidationResult());

      // when 
      var result = resources.Controller.AddOrderDetail(orderId, inputModel) as JsonResult;

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
      const string username = "testuser";

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderDetailLineNo", new GetOrderDetailLineNoRequestModel { OrderId = orderId }, 1);

      var inputModel = new List<OrderDetailInputModel>
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = 100M,
          Quantity = 10,
          Discount = 100M,
          LineTotal = 0M
        }
      };

      var requestModel = new List<AddOrderDetailRequestModel>();

      requestModel.Add(new AddOrderDetailRequestModel
      {
        LineNo = 1,
        OrderId = orderId,
        ItemDescription = inputModel[0].Product,
        UnitPrice = inputModel[0].UnitPrice,
        Quantity = inputModel[0].Quantity,
        Discount = inputModel[0].Discount,
        LineTotal = inputModel[0].LineTotal,
        Username = username
      });

      var validationResult = new ValidationResult();
      validationResult.InValidate("", "The line total provided is incorrect.");

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, validationResult);

      // when 
      var result = resources.Controller.AddOrderDetail(orderId, inputModel) as JsonResult;

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
      const string username = "testuser";

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderDetailLineNo", new GetOrderDetailLineNoRequestModel { OrderId = orderId }, 1);

      var inputModel = new List<OrderDetailInputModel>
      {
        new OrderDetailInputModel
        {
          Product = "Pizza",
          UnitPrice = -100M,
          Quantity = 10,
          Discount = 100M,
          LineTotal = 900M
        }
      };

      var requestModel = new List<AddOrderDetailRequestModel>();

      requestModel.Add(new AddOrderDetailRequestModel
      {
        LineNo = 1,
        OrderId = orderId,
        ItemDescription = inputModel[0].Product,
        UnitPrice = inputModel[0].UnitPrice,
        Quantity = inputModel[0].Quantity,
        Discount = inputModel[0].Discount,
        LineTotal = inputModel[0].LineTotal,
        Username = username
      });

      var validationResult = new ValidationResult();
      validationResult.InValidate("", "Unit price less than zero.");

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderDetail", requestModel, validationResult);

      // when 
      var result = resources.Controller.AddOrderDetail(orderId, inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var resultModel = (ValidationResult)result.Value;
      Assert.IsNotNull(resultModel);
      Assert.IsFalse(resultModel.IsValid);
    }

    [TestMethod]
    public void ConfirmOrderDetail_Load()
    {
      // given
      var resources = new Resources();
      var orderId = 1234;
      var requestModel = new GetOrderDetailRequestModel { OrderId = orderId };
      var responseModel = new OrderDetailModel
      {
        OrderId = orderId,
        OrderNo = "QUOTE" + orderId.ToString(),
        CreateDate = DateTime.Now,
        SubTotal = 222M,
        VatTotal = 20M,
        Total = 242M,
        Discount = 0M,
        OrderLineDetails = new List<OrderLineDetailModel>
        {
          new OrderLineDetailModel
          {
            OrderId = orderId,
            ItemDescription = "TestProduct",
            UnitPrice = 111M,
            Quantity = 2,
            Discount = 0M,
            LineTotal = 242M
          }
        }
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderDetail", requestModel, responseModel);

      // when
      var result = resources.Controller.ConfirmOrderDetail(orderId) as ViewResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual("ConfirmOrderDetail", result.ViewName);
      var model = (OrderDetailModel)result.Model;
      Assert.AreEqual(1, model.OrderLineDetails.Count);
      Assert.AreEqual(responseModel.OrderId, model.OrderId);
      Assert.AreEqual(responseModel.OrderNo, model.OrderNo);
      Assert.AreEqual(responseModel.SubTotal, model.SubTotal);
      Assert.AreEqual(responseModel.VatTotal, model.VatTotal);
      Assert.AreEqual(responseModel.Discount, model.Discount);
      Assert.AreEqual(responseModel.Total, model.Total);
      Assert.AreEqual(responseModel.OrderLineDetails[0].ItemDescription, model.OrderLineDetails[0].ItemDescription);
      Assert.AreEqual(responseModel.OrderLineDetails[0].Quantity, model.OrderLineDetails[0].Quantity);
      Assert.AreEqual(responseModel.OrderLineDetails[0].UnitPrice, model.OrderLineDetails[0].UnitPrice);
      Assert.AreEqual(responseModel.OrderLineDetails[0].Discount, model.OrderLineDetails[0].Discount);
      Assert.AreEqual(responseModel.OrderLineDetails[0].OrderId, model.OrderLineDetails[0].OrderId);
      Assert.AreEqual(responseModel.OrderLineDetails[0].LineTotal, model.OrderLineDetails[0].LineTotal);
    }

    [TestMethod]
    public void AcceptOrder_Success()
    {
      // given
      var resources = new Resources();
      var orderId = 1234;
      var requestModel = new AcceptOrderRequestModel { OrderId = orderId, Username = resources.TestUsername };
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AcceptOrder", requestModel, "Success");

      // when
      var result = resources.Controller.AcceptOrder(orderId) as JsonResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value.ToString());
    }

    #region Order Customers Edit/Add/View

    [TestMethod]
    public void GetUser_Company_Customers_ReturnsCustomerModel()
    {
      // given
      var resources = new Resources();
      var requestModel = new GetOrderCustomersRequestModel { CompanyProfileId = resources.CompanyProfileId, Username = resources.TestUsername };
      var responseModel = new List<CustomerModel>
      {
        new CustomerModel
        {
          CompanyProfileId = resources.CompanyProfileId,
          CustomerId = 1,
          CustomerName = "City of Cape Town",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new CustomerModel
        {
          CompanyProfileId = resources.CompanyProfileId,
          CustomerId = 2,
          CustomerName = "The Juggernauts",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new CustomerModel
        {
          CompanyProfileId = resources.CompanyProfileId,
          CustomerId = 2,
          CustomerName = "Water Inc.",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        }
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderCustomers", requestModel, responseModel);

      // when 
      var result = resources.Controller.GetOrderCustomers() as JsonResult;

      // then
      Assert.IsNotNull(result);
      var model = (List<CustomerModel>) result.Value;
      Assert.IsNotNull(model);
      Assert.AreEqual(3, model.Count);

      Assert.AreEqual(responseModel[0].CompanyProfileId, model[0].CompanyProfileId);
      Assert.AreEqual(responseModel[0].CustomerName, model[0].CustomerName);
      Assert.AreEqual(responseModel[0].CustomerId, model[0].CustomerId);
      Assert.AreEqual(responseModel[0].CreateDate, model[0].CreateDate);
      Assert.AreEqual(responseModel[0].CreateUser, model[0].CreateUser);

      Assert.AreEqual(responseModel[1].CompanyProfileId, model[1].CompanyProfileId);
      Assert.AreEqual(responseModel[1].CustomerName, model[1].CustomerName);
      Assert.AreEqual(responseModel[1].CustomerId, model[1].CustomerId);
      Assert.AreEqual(responseModel[1].CreateDate, model[1].CreateDate);
      Assert.AreEqual(responseModel[1].CreateUser, model[1].CreateUser);

      Assert.AreEqual(responseModel[2].CompanyProfileId, model[2].CompanyProfileId);
      Assert.AreEqual(responseModel[2].CustomerName, model[2].CustomerName);
      Assert.AreEqual(responseModel[2].CustomerId, model[2].CustomerId);
      Assert.AreEqual(responseModel[2].CreateDate, model[2].CreateDate);
      Assert.AreEqual(responseModel[2].CreateUser, model[2].CreateUser);
    }

    [TestMethod]
    public void GetCustomerContacts_ReturnsContactModel()
    {
      // given
      var resources = new Resources();
      const int customerId = 1;
      var requestModel = new GetCustomerContactsRequestModel { CustomerId = customerId };
      var responseModel = new List<ContactModel>
      {
        new ContactModel
        {
          CustomerId = customerId,
          ContactId = 1,
          ContactName = "Rustum Gabier",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = customerId,
          ContactId = 2,
          ContactName = "Mobb Deep",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = customerId,
          ContactId = 3,
          ContactName = "Elton Pappy",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        }
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetCustomerContacts", requestModel, responseModel);

      // when 
      var result = resources.Controller.GetCustomerContacts(customerId) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var model = (List<ContactModel>)result.Value;
      Assert.IsNotNull(model);
      Assert.AreEqual(3, model.Count);

      Assert.AreEqual(responseModel[0].CustomerId, model[0].CustomerId);
      Assert.AreEqual(responseModel[0].ContactId, model[0].ContactId);
      Assert.AreEqual(responseModel[0].ContactName, model[0].ContactName);
      Assert.AreEqual(responseModel[0].CreateDate, model[0].CreateDate);
      Assert.AreEqual(responseModel[0].CreateUser, model[0].CreateUser);

      Assert.AreEqual(responseModel[1].CustomerId, model[1].CustomerId);
      Assert.AreEqual(responseModel[1].ContactId, model[1].ContactId);
      Assert.AreEqual(responseModel[1].ContactName, model[1].ContactName);
      Assert.AreEqual(responseModel[1].CreateDate, model[1].CreateDate);
      Assert.AreEqual(responseModel[1].CreateUser, model[1].CreateUser);

      Assert.AreEqual(responseModel[2].CustomerId, model[2].CustomerId);
      Assert.AreEqual(responseModel[2].ContactId, model[2].ContactId);
      Assert.AreEqual(responseModel[2].ContactName, model[2].ContactName);
      Assert.AreEqual(responseModel[2].CreateDate, model[2].CreateDate);
      Assert.AreEqual(responseModel[2].CreateUser, model[2].CreateUser);
    }

    [TestMethod]
    public void AddNewCustomer_NoContact_Success_ReturnsValidationResult()
    {
      // given
      var resources = new Resources();
      var inputModel = new AddOrderCustomerRequestModel
      {
        CustomerId = 0,
        CustomerName = "New Customer",
        CustomerDetails = "Testing a customer details text area.",
        CustomerContactNo = "0214478512",
        CustomerMobileNo = "0847110055",
        CustomerAccountNo = "DR3243",
        CustomerEmailAddress = "someemail@gmail.com",
        ContactId = 0
      };
      var responseModel = new AddCustomerOrderModel { CustomerId = 1, ContactId = 1 };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderCustomer", inputModel, responseModel);

      // when
      var result = resources.Controller.AddCustomerOrder(inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var model = (AddCustomerOrderModel)result.Value;
      Assert.AreEqual(responseModel.CustomerId, model.CustomerId);
      Assert.AreEqual(responseModel.ContactId, model.ContactId);
    }

    [TestMethod]
    public void AddNewCustomer_NewContact_Success_ReturnsValidationResult()
    {
      // given
      var resources = new Resources();
      var inputModel = new AddOrderCustomerRequestModel
      {
        OrderId = 1,
        CustomerId = 0,
        CustomerName = "New Customer",
        CustomerDetails = "Testing a customer details text area.",
        CustomerContactNo = "0214478512",
        CustomerMobileNo = "0847110055",
        CustomerAccountNo = "DR3243",
        CustomerEmailAddress = "someemail@gmail.com",
        ContactId = 0,
        ContactName = "ContactName",
        ContactAdded = true,
        ContactEmailAddress = "",
        ContactNo ="0219983333"
      };
      var responseModel = new AddCustomerOrderModel { OrderId = 1, CustomerId = 1, ContactId = 1 };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddOrderCustomer", inputModel, responseModel);

      // when
      var result = resources.Controller.AddCustomerOrder(inputModel) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var model = (AddCustomerOrderModel)result.Value;
      Assert.AreEqual(responseModel.CustomerId, model.CustomerId);
      Assert.AreEqual(responseModel.ContactId, model.ContactId);
      Assert.AreEqual(responseModel.OrderId, model.OrderId);
    }

    [TestMethod]
    public void ConfirmOrderCustomer_Load()
    {
      // given
      var resources = new Resources();
      var orderId = 1234;
      var requestModel = new GetOrderCustomerDetailRequestModel { OrderId = orderId };

      var responseModel = new OrderCustomerDetailModel
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
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderCustomerDetails", requestModel, responseModel);

      // when
      var result = resources.Controller.ConfirmOrderCustomer(new AddCustomerOrderModel { OrderId = orderId}) as ViewResult;

      // then
      Assert.IsNotNull(result);
      Assert.AreEqual("ConfirmOrderCustomer", result.ViewName);

      var model = (OrderCustomerDetailModel)result.Model;
      Assert.AreEqual(responseModel.CustomerName, model.CustomerName);
      Assert.AreEqual(responseModel.CustomerDetails, model.CustomerDetails);
      Assert.AreEqual(responseModel.CustomerContactNo, model.CustomerContactNo);
      Assert.AreEqual(responseModel.CustomerAccountNo, model.CustomerAccountNo);
      Assert.AreEqual(responseModel.CustomerMobileNo, model.CustomerMobileNo);
      Assert.AreEqual(responseModel.CustomerEmailAddress, model.CustomerEmailAddress);
      Assert.AreEqual(responseModel.ContactAdded, model.ContactAdded);
      Assert.AreEqual(responseModel.ContactName, model.ContactName);
      Assert.AreEqual(responseModel.ContactNo, model.ContactNo);
      Assert.AreEqual(responseModel.ContactEmailAddress, model.ContactEmailAddress);
    }

    [TestMethod]
    public void GetOrderCustomerDetails()
    {
      // given
      var resources = new Resources();
      const int orderId = 123;
      var responsemodel = new OrderCustomerDetailModel
      {
        CustomerName = "Test Costumer",
        CustomerDetails = "This is some long customer description",
        CustomerContactNo = "0214472215",
        CustomerAccountNo = "DC1122",
        CustomerMobileNo = "0728543333",
        CustomerEmailAddress = "someemail@gmail.com",
        ContactAdded = true,
        ContactName = "Contraption",
        ContactNo = "0214472215",
        ContactEmailAddress = "someemail@gmail.com",
        CustomerId = 1,
        ContactId = 1,
        OrderId = 245,
        OrderNo = "MOQ001",
        OrderCreateDate = DateTime.Now
      };
      var requestmodel = new GetOrderCustomerDetailRequestModel { OrderId = orderId };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetOrderCustomerDetails", requestmodel, responsemodel);

      // when
      var result = resources.Controller.GetOrderCustomerDetails(orderId) as JsonResult;

      // then
      Assert.IsNotNull(result);
      var model = (OrderCustomerDetailModel)result.Value;
      Assert.AreEqual(responsemodel.OrderId, model.OrderId);
      Assert.AreEqual(responsemodel.OrderNo, model.OrderNo);
      Assert.AreEqual(responsemodel.OrderCreateDate, model.OrderCreateDate);
      Assert.AreEqual(responsemodel.CustomerName, model.CustomerName);
      Assert.AreEqual(responsemodel.CustomerId, model.CustomerId);
      Assert.AreEqual(responsemodel.CustomerMobileNo, model.CustomerMobileNo);
      Assert.AreEqual(responsemodel.CustomerEmailAddress, model.CustomerEmailAddress);
      Assert.AreEqual(responsemodel.CustomerDetails, model.CustomerDetails);
      Assert.AreEqual(responsemodel.CustomerContactNo, model.CustomerContactNo);
      Assert.AreEqual(responsemodel.CustomerAccountNo, model.CustomerAccountNo);
      Assert.AreEqual(responsemodel.ContactId, model.ContactId);
      Assert.AreEqual(responsemodel.ContactName, model.ContactName);
      Assert.AreEqual(responsemodel.ContactNo, model.ContactNo);
      Assert.AreEqual(responsemodel.ContactEmailAddress, model.ContactEmailAddress);
      Assert.AreEqual(responsemodel.ContactAdded, model.ContactAdded);
    }
    #endregion

    #region Address Details 
    [TestMethod]
    public void GetCustomerAddresses()
    {
      // Given
      var resources = new Resources();
      var customerId = 1;
      var requestModel = new GetCustomerAddressesRequestModel { CustomerId = customerId };
      var responseModel = new List<AddressDetailsModel>
      {
        new AddressDetailsModel
        {
          AddressDetailId = 1,
          AddressType = "Work",
          AddressLine1 = "24 Victoria Street",
          AddressLine2= "Muizenberg",
          City = "Cape Town",
          PostalCode = "7786",
          Country = "RSA",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new AddressDetailsModel
        {
          AddressDetailId = 1,
          AddressType = "Delivery",
          AddressLine1 = "24 John Street",
          AddressLine2= "Pelican Park",
          City = "Cape Town",
          PostalCode = "7786",
          Country = "RSA",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
        new AddressDetailsModel
        {
          AddressDetailId = 1,
          AddressType = "Home",
          AddressLine1 = "City Of Cape Town",
          AddressLine2= "Adderley Street",
          City = "Cape Town",
          PostalCode = "7800",
          Country = "RSA",
          CreateUser = resources.TestUsername,
          CreateDate = DateTime.Now
        },
      };
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetCustomerAddresses", requestModel, responseModel);

      // When 
      var result = resources.Controller.GetCustomerAddresses(customerId) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = (List<AddressDetailsModel>)result.Value;
      Assert.IsNotNull(model);
      Assert.AreEqual(3, model.Count);

      Assert.IsTrue(model.Any(o => o.AddressLine1 == responseModel[0].AddressLine1));
      Assert.IsTrue(model.Any(o => o.AddressLine2 == responseModel[0].AddressLine2));
      Assert.IsTrue(model.Any(o => o.City == responseModel[0].City));
      Assert.IsTrue(model.Any(o => o.Country == responseModel[0].Country));
      Assert.IsTrue(model.Any(o => o.PostalCode == responseModel[0].PostalCode));

      Assert.IsTrue(model.Any(o => o.AddressLine1 == responseModel[1].AddressLine1));
      Assert.IsTrue(model.Any(o => o.AddressLine2 == responseModel[1].AddressLine2));
      Assert.IsTrue(model.Any(o => o.City == responseModel[1].City));
      Assert.IsTrue(model.Any(o => o.Country == responseModel[1].Country));
      Assert.IsTrue(model.Any(o => o.PostalCode == responseModel[1].PostalCode));

      Assert.IsTrue(model.Any(o => o.AddressLine1 == responseModel[2].AddressLine1));
      Assert.IsTrue(model.Any(o => o.AddressLine2 == responseModel[2].AddressLine2));
      Assert.IsTrue(model.Any(o => o.City == responseModel[2].City));
      Assert.IsTrue(model.Any(o => o.Country == responseModel[2].Country));
      Assert.IsTrue(model.Any(o => o.PostalCode == responseModel[2].PostalCode));
    }

    [TestMethod]
    public void RemoveCustomerAddress()
    {
      // Given
      var resources = new Resources();
      var addressDetailId = 1;
      var customerId = 1;
      var requestModel = new RemoveCustomerAddressRequestModel { AddressDetailId = addressDetailId, CustomerId = customerId, Username = resources.TestUsername };
      
      resources.MockApiCaller.AddMockResponse("WebApi:Orders:RemoveCustomerOrderAddress", requestModel, "Success");

      // When 
      var result = resources.Controller.RemoveCustomerOrderAddress(addressDetailId, customerId) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value.ToString();
      Assert.AreEqual("Success", model);
    }

    [TestMethod]
    public void AddNewCustomerAddress()
    {
      // Given
      var resources = new Resources();
      var addressDetailId = 0;
      var requestModel = new AddCustomerOrderAddressRequestModel
      {
        AddressDetailId = addressDetailId,
        AddressType = "Delivery",
        AddressLine1 = "25 Anzio Crescent",
        AddressLine2 = "Strandfontein",
        City = "Cape Town",
        Country = "South Africa",
        PostalCode = "7788"
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddCustomerOrderAddress", requestModel, "Success");

      // When 
      var result = resources.Controller.AddCustomerOrderAddress(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value.ToString();
      Assert.AreEqual("Success", model);
    }

    [TestMethod]
    public void AddExistingCustomerAddress()
    {
      // Given
      var resources = new Resources();
      var addressDetailId = 21;
      var requestModel = new AddCustomerOrderAddressRequestModel
      {
        AddressDetailId = addressDetailId,
        AddressType = "Delivery",
        AddressLine1 = "25 Anzio Crescent",
        AddressLine2 = "Strandfontein",
        City = "Cape Town",
        Country = "South Africa",
        PostalCode = "7788"
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:AddCustomerOrderAddress", requestModel, "Success");

      // When 
      var result = resources.Controller.AddCustomerOrderAddress(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value.ToString();
      Assert.AreEqual("Success", model);
    }

    [TestMethod]
    public void GetCustomerOrderAddress()
    {
      // Given
      var resources = new Resources();
      var orderId = 123;
      var requestModel = new GetOrderAddressRequestModel { OrderId = orderId, CustomerId = 1};
      var responseModel = new AddressDetailsModel
      {
        AddressDetailId = 1,
        AddressType = "Work",
        AddressLine1 = "24 Victoria Street",
        AddressLine2 = "Muizenberg",
        City = "Cape Town",
        PostalCode = "7786",
        Country = "South Africa",
        CreateUser = resources.TestUsername,
        CreateDate = DateTime.Now
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Orders:GetCustomerOrderAddress", requestModel, responseModel);

      // When 
      var result = resources.Controller.GetCustomerOrderAddress(orderId, 1) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = (AddressDetailsModel)result.Value;
      Assert.IsNotNull(model);
      Assert.AreEqual(responseModel.AddressDetailId, model.AddressDetailId);
      Assert.AreEqual(responseModel.AddressLine1, model.AddressLine1);
      Assert.AreEqual(responseModel.AddressLine2, model.AddressLine2);
      Assert.AreEqual(responseModel.City, model.City);
      Assert.AreEqual(responseModel.Country, model.Country);
      Assert.AreEqual(responseModel.PostalCode, model.PostalCode);
    }
    #endregion

    #region View Orders

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
          Status = "Accepted",
          CustomerName = "City of Cape Town",
          EmailAddress = "someemail@address.co.za"
        },
        new HomeOrdersModel
        {
          OrderId = 1,
          OrderNo = "Moq001",
          Total = "R 2 999.99",
          Status = "N/A",
          CustomerName = "Maria Sharpova",
          EmailAddress = "someemail@address.co.za"
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
          Status = "Status",
          EmailAddress = "someemail@address.co.za"
        }
      };

      var startDate = DateTime.Now.AddMonths(-1);
      var endDate = DateTime.Now;

      resources.MockApiCaller.AddMockResponse("WebApi:Home:GetHomeOrdersInPeriod", new GetHomeOrdersPeriodRequestModel { Username = resources.TestUsername, CompanyProfileId = 1, StartDate = startDate, EndDate = endDate }, orders);

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
      var model = (OrderQuotationViewModel)result.Model;
      Assert.IsNotNull(model);
      Assert.AreEqual("OrderQuotation", result.ViewName);
      Assert.AreEqual(model.CompanyProfile, responseModel.CompanyProfile);
      Assert.AreEqual(model.CustomerDetail, responseModel.CustomerDetail);
      Assert.AreEqual(model.DeliveryAddress, responseModel.DeliveryAddress);
      Assert.AreEqual(model.OrderTotals, responseModel.OrderTotals);
    }

    #endregion


  }
}
