using System.Collections.Generic;
using System;
using ColourCoded.UI.Areas.Home.Models;
using ColourCoded.UI.Areas.Orders.Models.RequestModels;
using ColourCoded.UI.Areas.Orders.Models.ResponseModels;
using ColourCoded.UI.Areas.Orders.Models.InputModels;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Areas.Security.Models.Login.RequestModels;

namespace ColourCoded.UI.Shared.WebApiCaller
{
  public class MockWebApi
  {
    public List<MockApiResponseModel> Responses;

    public MockWebApi()
    {
      Responses = new List<MockApiResponseModel>();
      ConfigureMock_RoleController_Responses();
    }

    public void ConfigureMock_RoleController_Responses()
    {
      // WebApi:Orders:GetOrderCustomerDetail

      var responseModel5 = new OrderCustomerDetailModel
      {
        CustomerName = "Test Costume",
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
        OrderId = 1,
        OrderNo = "MOQ001",
        OrderCreateDate = DateTime.Now
      };

      
      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderCustomerDetail", RequestModel = new GetOrderCustomerDetailRequestModel { OrderId = 1 }, ResponseContent = responseModel5 });


      // WebApi:Orders:AddOrderCustomer
      var inputModel = new AddOrderCustomerRequestModel
      {
        OrderId = 1,
        CustomerId = 0,
        CustomerName = "1",
        CustomerDetails = "1",
        CustomerContactNo = "1",
        CustomerMobileNo = "1",
        CustomerAccountNo = "1",
        CustomerEmailAddress = "1@gmail.com",
        ContactId = 0,
        ContactAdded = false
      };
      var responseModel4 = new OrderCustomerModel { OrderId = 1, CustomerId = 1, ContactId = 1};

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:AddOrderCustomer", RequestModel = inputModel, ResponseContent = responseModel4 });

      inputModel = new AddOrderCustomerRequestModel
      {
        OrderId = 1,
        CustomerId = 0,
        CustomerName = "1",
        CustomerDetails = "1",
        CustomerContactNo = "1",
        CustomerMobileNo = "1",
        CustomerAccountNo = "1",
        CustomerEmailAddress = "1@gmail.com",
        ContactId = 0,
        ContactAdded = true,
        ContactEmailAddress = "1@gmail.com",
        ContactName = "1",
        ContactNo = "1"
      };
      responseModel4 = new OrderCustomerModel { OrderId = 1, CustomerId = 1, ContactId = 1 };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:AddOrderCustomer", RequestModel = inputModel, ResponseContent = responseModel4 });


      // WebApi:Orders:GetCustomerContacts
      var responseModel3 = new List<ContactModel>
      {
        new ContactModel
        {
          CustomerId = 1,
          ContactId = 1,
          ContactName = "Rustum Gabier",
          ContactNo = "0217548899",
          EmailAddress = "rustum@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = 1,
          ContactId = 2,
          ContactName = "Mobb Deep",
          ContactNo = "0825554444",
          EmailAddress = "mobbdeep@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = 1,
          ContactId = 3,
          ContactName = "Elton Pappy",
          ContactNo = "0726985223",
          EmailAddress = "elton@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        }
      };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetCustomerContacts", RequestModel = new GetCustomerContactsRequestModel { CustomerId = 1 }, ResponseContent = responseModel3 });

      responseModel3 = new List<ContactModel>
      {
        new ContactModel
        {
          CustomerId = 2,
          ContactId = 4,
          ContactName = "Chocalate",
          ContactNo = "0217548899",
          EmailAddress = "Chocalate@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = 2,
          ContactId = 5,
          ContactName = "Taster",
          ContactNo = "0825554444",
          EmailAddress = "Taster@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new ContactModel
        {
          CustomerId = 2,
          ContactId = 6,
          ContactName = "Swedend",
          ContactNo = "0726985223",
          EmailAddress = "Swedend@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        }
      };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetCustomerContacts", RequestModel = new GetCustomerContactsRequestModel { CustomerId = 2 }, ResponseContent = responseModel3 });

      // WebApi:Orders:GetOrderCustomers
      var responseModel2 = new List<CustomerModel>
      {
        new CustomerModel
        {
          CompanyProfileId = 1,
          CustomerId = 1,
          CustomerName = "City of Cape Town",
          CustomerDetails = "Ravensmead Centre c/o Jan Smuts and Gilleger rd",
          AccountNo = "Moq1",
          ContactNo = "0217548899",
          MobileNo = "0214458888",
          EmailAddress = "someemail@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new CustomerModel
        {
          CompanyProfileId = 1,
          CustomerId = 2,
          CustomerName = "The Juggernauts",
          CustomerDetails = "Fantastic Four",
          AccountNo = "Moq2",
          ContactNo = "0213966655",
          MobileNo = "0214458888",
          EmailAddress = "test@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        },
        new CustomerModel
        {
          CompanyProfileId = 1,
          CustomerId = 2,
          CustomerName = "Water Inc.",
          CustomerDetails = "Cravenby and Newlands",
          AccountNo = "Moq3",
          ContactNo = "0214458888",
          MobileNo = "0725584589",
          EmailAddress = "mo@capetown.gov.za",
          CreateUser ="zunaid",
          CreateDate = DateTime.Now
        }
      };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderCustomers", RequestModel = new GetOrderCustomersRequestModel() { Username = "zunaid", CompanyProfileId = 1 }, ResponseContent = responseModel2 });

      /*
      // WebApi:Login
      Responses.Add(new MockApiResponseModel
      {
        WepApiUrl = "WebApi:Authenticate:Login",
        RequestModel = new LoginRequestModel { Username = "zunaid", Password = "222222" },
        ResponseContent = new UserModel { Username = "zunaid", ApiSessionToken = System.Guid.NewGuid().ToString(), IsAuthenticated = true, CompanyProfileId = 1 }
      });

      //"WebApi:Home:GetHomerOrders"
      var viewModel = new List<HomeOrdersModel>
          {
            new HomeOrdersModel
            {
              OrderId = 1, OrderNo = "Moq001", CreateDate = DateTime.Now.AddDays(24).ToShortDateString(), Total = "R 2 999.99"
            },
            new HomeOrdersModel
            {
              OrderId = 2, OrderNo = "Moq002", CreateDate = DateTime.Now.AddDays(2).ToShortDateString(), Total = "R 1 999.99"
            }
          };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Home:GetHomeOrders", RequestModel = new GetHomeOrdersRequestModel() { Username = "zunaid", CompanyProfileId = 1}, ResponseContent = viewModel });


      // resources.MockApiCaller.AddMockResponse("WebApi:Home:GetVatRate", null, vatRate);
      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetVatRate", RequestModel = null, ResponseContent = 0.15M });
      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderNoSeed", RequestModel = null, ResponseContent = 123 });
     

     // WebApi: Orders: GetOrderDetail
       var responseModel = new OrderDetailModel
       {
         OrderId = 1,
         OrderNo = "QUOTE123",
         CreateDate = DateTime.Now,
         SubTotal = 222M,
         VatTotal = 20M,
         Total = 242M,
         Discount = 0M,
         OrderLineDetails = new List<OrderLineDetailModel>
         {
          new OrderLineDetailModel
          {
            OrderId = 1,
            ItemDescription = "TestProduct",
            UnitPrice = 111M,
            Quantity = 2,
            Discount = 0M,
            LineTotal = 242M
          },
          new OrderLineDetailModel
          {
            OrderId = 1,
            ItemDescription = "Delivery Fee",
            UnitPrice = 111M,
            Quantity = 21,
            Discount = 0M,
            LineTotal = 111M
          },
         }
       };

      Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderDetail", RequestModel = new GetOrderDetailRequestModel { OrderId = 1 }, ResponseContent = responseModel });
      */
      //var orderLineDetails = new List<OrderLineDetailModel>
      //  {
      //    new OrderLineDetailModel
      //    {
      //      OrderId = 123,
      //      ItemDescription = "TestProduct",
      //      UnitPrice = 111M,
      //      Quantity = 2,
      //      Discount = 0M,
      //      LineTotal = 242M
      //    },
      //    new OrderLineDetailModel
      //    {
      //      OrderId = 123,
      //      ItemDescription = "Delivery Fee",
      //      UnitPrice = 111M,
      //      Quantity = 21,
      //      Discount = 0M,
      //      LineTotal = 111M
      //    },
      //  };

      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderLineDetails", RequestModel = new GetOrderLineDetailsRequestModel { OrderId = 123 }, ResponseContent = orderLineDetails });

      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:GetOrderDetailLineNo", RequestModel = new GetOrderDetailLineNoRequestModel { OrderId = 123 }, ResponseContent = 1 });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:EditOrderNo", RequestModel = new EditOrderNoRequestModel { OrderId = 123, OrderNo = "QUOTE123", Username = "zunaid" }, ResponseContent = "Success" });

      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:AddOrder", RequestModel = new AddOrderRequestModel { OrderNo = "QUOTE123", Username = "zunaid" }, ResponseContent = 123 });

      //var inputModel = new List<OrderDetailInputModel>
      //{
      //  new OrderDetailInputModel
      //  {
      //    Product = "test",
      //    UnitPrice = 1M,
      //    Quantity = 1M,
      //    Discount = 0M,
      //    LineTotal = 1M
      //  },
      //  new OrderDetailInputModel
      //  {
      //    Product = "delivery",
      //    UnitPrice = 1M,
      //    Quantity = 1M,
      //    Discount = 0M,
      //    LineTotal = 1M
      //  }
      //};

      //foreach (var model in inputModel)
      //{
      //  var requestModel = new AddOrderDetailRequestModel
      //  {
      //    LineNo = 1,
      //    OrderId = 123,
      //    ItemDescription = model.Product,
      //    UnitPrice = model.UnitPrice,
      //    Quantity = model.Quantity,
      //    Discount = model.Discount,
      //    LineTotal = model.LineTotal,
      //    Username = "zunaid"
      //  };

      //  Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Orders:AddOrderDetail", RequestModel = requestModel, ResponseContent = new ValidationResult() });
      //}
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Home:GetUserOrders", RequestModel = new FindUserOrdersRequestModel { Username = "zunaid" }, ResponseContent = viewModel });

      // WebApi:Role:GetUsernames
      //var usernames = new List<string> { "Jon", "Jonny", "Jonathon", "Johno"};
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Users:Filter", RequestModel = "jj", ResponseContent = usernames });

      //usernames = new List<string> { "Adam", "Andrea", "Abigail", "Aslam" };
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Users:Filter", RequestModel = "aa", ResponseContent = usernames });

      // WebApi:Authenticate:ChangePassword
      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Authenticate:ChangePassword",
      //  RequestModel = new ChangePasswordRequestModel
      //  {
      //    Username = "zunaid",
      //    NewPassword = "111111",
      //  },
      //  ResponseContent = new UserModel { Username = "zunaid", ApiSessionToken = new System.Guid().ToString(), IsAuthenticated = true }
      //});

      // WebApi:Register:Save
      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Register:Save",
      //  RequestModel = new RegisterUserRequestModel
      //  {
      //    Username = "zunaid",
      //    Password = "222222",
      //    ConfirmPassword = "222222",
      //    FirstName = "3",
      //    LastName = "4",
      //    EmailAddress = "zunaid@gmail.com"
      //  },
      //  ResponseContent = new UserModel { Username = "zunaid", EmailAddress = "zunaid@gmail.com", ApiSessionToken = System.Guid.NewGuid().ToString() }
      //  });

      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Register:Save",
      //  RequestModel = new RegisterUserRequestModel
      //  {
      //    Username = "111111",
      //    Password = "222222",
      //    ConfirmPassword = "222222",
      //    FirstName = "3",
      //    LastName = "4",
      //    EmailAddress = "5@gmail.com"
      //  },
      //  ResponseContent = null 
      //});

      //// WebApi:Register:ValidateUser
      //var responseModel = new ValidationResult();
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateUser", RequestModel = new ValidateUserRequestModel { Username = "zunaid" }, ResponseContent = responseModel });

      //responseModel = new ValidationResult();
      //responseModel.InValidate("", "Username already exists");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateUser", RequestModel = new ValidateUserRequestModel { Username = "rowena" }, ResponseContent = responseModel });

      //responseModel = new ValidationResult();
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateUser", RequestModel = new ValidateUserRequestModel { Username = "111111" }, ResponseContent = responseModel });

      ////WebApi: Register: ValidateEmail
      //responseModel = new ValidationResult();
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateEmail", RequestModel = new ValidateEmailRequestModel { EmailAddress = "zunaid@gmail.com" }, ResponseContent = responseModel });

      //responseModel = new ValidationResult();
      //responseModel.InValidate("", "Already linked to another user.");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateEmail", RequestModel = new ValidateEmailRequestModel { EmailAddress = "rowena@gmail.com" }, ResponseContent = responseModel });

      //responseModel = new ValidationResult();
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Register:ValidateEmail", RequestModel = new ValidateEmailRequestModel { EmailAddress = "5@gmail.com" }, ResponseContent = responseModel });



      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Login",
      //  RequestModel = new LoginRequestModel { Username = "1", Password = "1" },
      //  ResponseContent = null
      //});

      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Login",
      //  RequestModel = new LoginRequestModel { Username = "rowena", Password = "1234" },
      //  ResponseContent = new UserModel { Username = "rowena", ApiSessionToken = System.Guid.NewGuid().ToString(), IsAuthenticated = false, CompanyProfileId =  = 1 }
      //});

      //WebApi:Login:RequestCredentials
      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Login:ForgotPassword",
      //  RequestModel = new CredentialsRequestModel { EmailAddress = "zunaid@gmail.com" },
      //  ResponseContent = true
      //});

      //Responses.Add(new MockApiResponseModel
      //{
      //  WepApiUrl = "WebApi:Login:RequestCredentials",
      //  RequestModel = new CredentialsRequestModel { EmailAddress = "5@gmail.com" },
      //  ResponseContent = false
      //});

      //WebApi: Permissions: GetArtifacts
      //var artifacts = new List<ArtifactModel> { new ArtifactModel { ArtifactId = 1, ArtifactName = "Add Customer"},
      //                                  new ArtifactModel { ArtifactId = 2, ArtifactName = "Create Quote"},
      //                                  new ArtifactModel { ArtifactId = 3, ArtifactName = "Add Company Profile"},
      //                                  new ArtifactModel { ArtifactId = 4, ArtifactName = "Remove Delivery Address"} };
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:GetArtifacts", RequestModel = null, ResponseContent = artifacts });

      //// WebApi:Permissions:AddArtifact - 
      //var responseModel = new ValidationResult();
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:AddArtifact", RequestModel = new AddArtifactRequestModel { ArtifactName = "Testing", CreateUsername = "zunaid" }, ResponseContent = new ValidationResult() });

      //// WebApi:Permissions:AddArtifact - Already exists
      //responseModel.InValidate("", "The artifact already exists");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:AddArtifact", RequestModel = new AddArtifactRequestModel { ArtifactName = "Add Customer", CreateUsername = "zunaid" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:AddArtifact", RequestModel = new AddArtifactRequestModel { ArtifactName = "Create Quote", CreateUsername = "zunaid" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:AddArtifact", RequestModel = new AddArtifactRequestModel { ArtifactName = "Add Company Profile", CreateUsername = "zunaid" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:AddArtifact", RequestModel = new AddArtifactRequestModel { ArtifactName = "Remove Delivery Address", CreateUsername = "zunaid" }, ResponseContent = responseModel });

      //// WebApi:Permissions:RemoveArtifact 
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:RemoveArtifact", RequestModel = new RemoveArtifactRequestModel { ArtifactId = 1 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:RemoveArtifact", RequestModel = new RemoveArtifactRequestModel { ArtifactId = 2 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:RemoveArtifact", RequestModel = new RemoveArtifactRequestModel { ArtifactId = 3 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:RemoveArtifact", RequestModel = new RemoveArtifactRequestModel { ArtifactId = 4 }, ResponseContent = null });

      //// WebApi:Permissions:Find
      //var permissions = new List<PermissionModel>();
      //permissions.Add(new PermissionModel { PermissionId = 1, ArtifactId = 1, RoleName = "Administrator" });
      //permissions.Add(new PermissionModel { PermissionId = 2, ArtifactId = 1, RoleName = "Operations" });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Find", RequestModel = new FindPermissionsRequestModel { ArtifactId = 1 }, ResponseContent = permissions });

      //// WebApi: Permissions: EditArtifact - Successful
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:EditArtifact", RequestModel = new EditArtifactRequestModel { ArtifactId = 1, ArtifactName = "Testing", UpdateUsername = "zunaid" }, ResponseContent = new ValidationResult() });

      //// WebApi:Permissions:Remove
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Remove", RequestModel = new RemovePermissionRequestModel { PermissionId = 1 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Remove", RequestModel = new RemovePermissionRequestModel { PermissionId = 2 }, ResponseContent = null });

      ////// WebApi:Permissions:Add - Successful
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Add", RequestModel = new AddPermissionRequestModel { ArtifactId = 1, RoleId = 2, CreateUsername = "zunaid" }, ResponseContent = new ValidationResult() });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Add", RequestModel = new AddPermissionRequestModel { ArtifactId = 1, RoleId = 3, CreateUsername = "zunaid" }, ResponseContent = new ValidationResult() });

      ////// WebApi:Permissions:Add - Already exists
      //responseModel = new ValidationResult();
      //responseModel.InValidate("", "The permission already exists");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Add", RequestModel = new AddPermissionRequestModel { ArtifactId = 1, RoleId = 1, CreateUsername = "zunaid" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Permissions:Add", RequestModel = new AddPermissionRequestModel { ArtifactId = 1, RoleId = 4, CreateUsername = "zunaid" }, ResponseContent = responseModel });


      // WebApi:Role:GetAll
      //var roles = new List<RoleModel> { new RoleModel { RoleId = 1, RoleName = "Administrator"},
      //                                  new RoleModel { RoleId = 2, RoleName = "Vetting Clerk"},
      //                                  new RoleModel { RoleId = 3, RoleName = "Programmer"},
      //                                  new RoleModel { RoleId = 4, RoleName = "Operations"} };
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:GetAll", RequestModel = null, ResponseContent = roles });

      // WebApi:Role:AddRole - Successful
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Legal" }, ResponseContent = new ValidationResult() });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Orders" }, ResponseContent = new ValidationResult() });

      // WebApi: Role: EditRole - Successful
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:EditRole", RequestModel = new EditRoleRequestModel { RoleId = 1, RoleName = "Legal", AuditUsername = string.Empty }, ResponseContent = new ValidationResult() });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:EditRole", RequestModel = new EditRoleRequestModel { RoleId = 1, RoleName = "Orders", AuditUsername = string.Empty }, ResponseContent = new ValidationResult() });

      // WebApi:Role:AddRole - Already exists
      //responseModel.InValidate("", "The role already exists");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Administrator" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Vetting Clerk" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Programmer" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddRole", RequestModel = new AddRoleRequestModel { RoleName = "Operations" }, ResponseContent = responseModel });

      // WebApi:Role:GetMembers
      //var members = new List<MemberModel>();
      //members.Add(new MemberModel { MemberId = 1, RoleId = 1, Username = "jacob" });
      //members.Add(new MemberModel { MemberId = 2, RoleId = 1, Username = "aslam" });
      //members.Add(new MemberModel { MemberId = 3, RoleId = 1, Username = "melicia" });
      //members.Add(new MemberModel { MemberId = 4, RoleId = 1, Username = "craig" });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:GetMembers", RequestModel = new FindMembersRequestModel { RoleId = 1 }, ResponseContent = members} );

      //members = new List<MemberModel>();
      //members.Add(new MemberModel { MemberId = 5, RoleId = 2, Username = "john" });
      //members.Add(new MemberModel { MemberId = 6, RoleId = 2, Username = "kaasiem" });
      //members.Add(new MemberModel { MemberId = 7, RoleId = 2, Username = "abdullah" });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:GetMembers", RequestModel = new FindMembersRequestModel { RoleId = 2 }, ResponseContent = members });

      //members = new List<MemberModel>();
      //members.Add(new MemberModel { MemberId = 8, RoleId = 3, Username = "rugaya" });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:GetMembers", RequestModel = new FindMembersRequestModel { RoleId = 3 }, ResponseContent = members });

      //// WebApi:Role:AddMember - Successful
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "Zunaid" }, ResponseContent = new ValidationResult() });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "Regan" }, ResponseContent = new ValidationResult() });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "John" }, ResponseContent = new ValidationResult() });

      //// WebApi:Role:AddMember - Already exists
      //responseModel = new ValidationResult();
      //responseModel.InValidate("", "The member already exists");
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "jacob" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "aslam" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "melicia" }, ResponseContent = responseModel });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:AddMember", RequestModel = new AddMemberRequestModel { RoleId = 1, Username = "craig" }, ResponseContent = responseModel });

      // WebApi:Role:RemoveRole 
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveRole", RequestModel = new RemoveRoleRequestModel { RoleId = 1 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveRole", RequestModel = new RemoveRoleRequestModel { RoleId = 2 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveRole", RequestModel = new RemoveRoleRequestModel { RoleId = 3 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveRole", RequestModel = new RemoveRoleRequestModel { RoleId = 4 }, ResponseContent = null });

      // WebApi:Role:RemoveMember
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveMember", RequestModel = new RemoveMemberRequestModel { RoleId = 1, MemberId = 1 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveMember", RequestModel = new RemoveMemberRequestModel { RoleId = 1, MemberId = 2 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveMember", RequestModel = new RemoveMemberRequestModel { RoleId = 1, MemberId = 3 }, ResponseContent = null });
      //Responses.Add(new MockApiResponseModel { WepApiUrl = "WebApi:Role:RemoveMember", RequestModel = new RemoveMemberRequestModel { RoleId = 1, MemberId = 4 }, ResponseContent = null });
    }
  }
}
