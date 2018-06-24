using ColourCoded.UI.Areas.Security.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColourCoded.UI.Areas.Security.Models.Role;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Areas.Security.Models.Role.RequestModels;
using System.Linq;
using Moq;
using ColourCoded.UI.Areas.Security.Models.Login;
using System;

namespace ColourCoded.Tests.Security.Roles
{

  [TestClass]
  public class RolesControllerTests
  {
    private class Resources
    {
      public RolesController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;
      public string TestUsername { get; set; }

      public Resources()
      {
        TestUsername = "testuser";
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });
        Controller = new RolesController(MockApiCaller, MockCookieHelper.Object);
      }
    }

    [TestMethod]
    public void GetAll_RolesList()
    {
      // Given
      var resources = new Resources();

      var roles = new List<RoleModel> { new RoleModel { RoleId = 1, RoleName = "Administrator"},
                                        new RoleModel { RoleId = 2, RoleName = "Vetting Clerk"},
                                        new RoleModel { RoleId = 3, RoleName = "Programmer"},
                                        new RoleModel { RoleId = 4, RoleName = "Operations"} };

      resources.MockApiCaller.AddMockResponse("WebApi:Role:GetAll", null, roles);

      // When
      var result = resources.Controller.GetAll() as JsonResult;

      // Then
      Assert.IsNotNull(result);

      var resultModel = result.Value as List<RoleModel>;
      Assert.IsNotNull(resultModel);
      Assert.AreEqual(4, resultModel.Count);
    }

    [TestMethod]
    public void AddRole_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddRoleRequestModel { RoleName = "Test Role", CreateUser = resources.TestUsername };
      resources.MockApiCaller.AddMockResponse("WebApi:Role:AddRole", requestModel, new ValidationResult());

      // When
      var result = resources.Controller.AddRole(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsTrue(resultModel.IsValid);
      Assert.AreEqual(0, resultModel.Messages.Count);
    }

    [TestMethod]
    public void AddRole_AlreadyExists_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddRoleRequestModel { RoleName = "Test Role" };
      var responseModel = new ValidationResult();
      responseModel.InValidate("", "The role already exists");
      resources.MockApiCaller.AddMockResponse("WebApi:Role:AddRole", requestModel, responseModel);

      // When
      var result = resources.Controller.AddRole(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsFalse(resultModel.IsValid);
      Assert.AreEqual(1, resultModel.Messages.Count);
      Assert.AreEqual("The role already exists", resultModel.Messages[0].Message);
    }

    [TestMethod]
    public void EditRole_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new EditRoleRequestModel { RoleId = 1, RoleName = "Test Role", CreateUser = resources.TestUsername };

      var responseModel = new ValidationResult();
      resources.MockApiCaller.AddMockResponse("WebApi:Role:EditRole", requestModel, responseModel);

      // When
      var result = resources.Controller.EditRole(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsTrue(resultModel.IsValid);
      Assert.AreEqual(0, resultModel.Messages.Count);
    }

    [TestMethod]
    public void RemoveRole_Successful()
    {
      // Given
      var resources = new Resources();
      var requestModel = new RemoveRoleRequestModel { RoleId = 1 };
      resources.MockApiCaller.AddMockResponse("WebApi:Role:RemoveRole", requestModel, "Success");

      // When 
      var result = resources.Controller.RemoveRole(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value.ToString());
    }

    [TestMethod]
    public void GetRoleMembers_RoleMembersList()
    {
      // Given
      var resources = new Resources();
      var requestModel = new FindRoleMembersRequestModel { RoleId = 1 };
      var members = new List<RoleMemberModel>();
      members.Add(new RoleMemberModel { RoleMemberId = 1, RoleId = 1, Username = "SomeUsernameOne" });
      members.Add(new RoleMemberModel { RoleMemberId = 2, RoleId = 1, Username = "SomeUsernameTwo" });
      members.Add(new RoleMemberModel { RoleMemberId = 3, RoleId = 1, Username = "SomeUsernameThree" });
      members.Add(new RoleMemberModel { RoleMemberId = 4, RoleId = 1, Username = "SomeUsernameFour" });

      resources.MockApiCaller.AddMockResponse("WebApi:Role:GetRoleMembers", requestModel, members);

      // When
      var result = resources.Controller.GetRoleMembers(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);

      var resultModel = result.Value as List<RoleMemberModel>;
      Assert.IsNotNull(resultModel);
      Assert.AreEqual(4, resultModel.Count);
      Assert.IsTrue(resultModel.All(m => m.RoleId == requestModel.RoleId));
    }

    [TestMethod]
    public void AddMember_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddRoleMemberRequestModel { RoleId = 1, Username = "SomeUsername", CreateUser = resources.TestUsername };
      resources.MockApiCaller.AddMockResponse("WebApi:Role:AddRoleMember", requestModel, new ValidationResult());

      // When
      var result = resources.Controller.AddRoleMember(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsTrue(resultModel.IsValid);
      Assert.AreEqual(0, resultModel.Messages.Count);
    }

    [TestMethod]
    public void AddMember_AlreadyExists_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddRoleMemberRequestModel { RoleId = 1, Username = "SomeUsername", CreateUser = resources.TestUsername };
      var validationResult = new ValidationResult();
      validationResult.InValidate("", "The member already exists");
      resources.MockApiCaller.AddMockResponse("WebApi:Role:AddRoleMember", requestModel, validationResult);

      // When
      var result = resources.Controller.AddRoleMember(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsFalse(resultModel.IsValid);
      Assert.AreEqual(1, resultModel.Messages.Count);
      Assert.AreEqual("The member already exists", resultModel.Messages[0].Message);
    }

    [TestMethod]
    public void RemoveMember_Successful()
    {
      // Given
      var resources = new Resources();
      var requestModel = new RemoveRoleMemberRequestModel { RoleId = 1 , RoleMemberId = 1};
      resources.MockApiCaller.AddMockResponse("WebApi:Role:RemoveRoleMember", requestModel, "Success");

      // When 
      var result = resources.Controller.RemoveRoleMember(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value.ToString());
    }

    [TestMethod]
    public void FilterUsers_UsernamesList()
    {
      // Given
      var resources = new Resources();
      const string filterText = "a";
      resources.MockApiCaller.AddMockResponse("WebApi:Role:SearchUsers", new SearchUsersRequestModel { SearchTerm = filterText }, new List<string> { "andrea", "abigail", "aslam" });

      // When
      var result = resources.Controller.FilterUsernames(filterText) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var responseModel = result.Value as List<string>;
      Assert.IsNotNull(responseModel);
      Assert.AreEqual(3, responseModel.Count);
      Assert.IsTrue(responseModel.Contains("andrea"));
      Assert.IsTrue(responseModel.Contains("abigail"));
      Assert.IsTrue(responseModel.Contains("aslam"));
    }
  }
}
