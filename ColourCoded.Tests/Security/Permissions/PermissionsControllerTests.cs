using ColourCoded.UI.Areas.Security.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColourCoded.UI.Areas.Security.Models.Permissions;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Areas.Security.Models.Permissions.RequestModels;
using System.Linq;
using Moq;
using ColourCoded.UI.Areas.Security.Models.Login;

namespace ColourCoded.Tests.Security.Roles
{

  [TestClass]
  public class PermissionsControllerTests
  {
    private class Resources
    {
      public string Username { get; set; }
      public PermissionsController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;

      public Resources()
      {
        Username = "testuser";
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        Controller = new PermissionsController(MockApiCaller, MockCookieHelper.Object);
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = Username, ApiSessionToken = System.Guid.NewGuid().ToString(), IsAuthenticated = true });
      }
    }

    [TestMethod]
    public void GetAll_ArtifactsList()
    {
      // Given
      var resources = new Resources();

      var artifacts = new List<ArtifactModel> { new ArtifactModel { ArtifactId = 1, ArtifactName = "Create Quote"},
                                        new ArtifactModel { ArtifactId = 2, ArtifactName = "Vetting Clerk"},
                                        new ArtifactModel { ArtifactId = 3, ArtifactName = "Add Roles"},
                                        new ArtifactModel { ArtifactId = 4, ArtifactName = "Add Customer"} };

      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:GetAllArtifacts", null, artifacts);

      // When
      var result = resources.Controller.GetArtifacts() as JsonResult;

      // Then
      Assert.IsNotNull(result);

      var resultModel = result.Value as List<ArtifactModel>;
      Assert.IsNotNull(resultModel);
      Assert.AreEqual(4, resultModel.Count);
      Assert.IsTrue(resultModel.Any(a => a.ArtifactName == artifacts[0].ArtifactName));
      Assert.IsTrue(resultModel.Any(a => a.ArtifactName == artifacts[1].ArtifactName));
      Assert.IsTrue(resultModel.Any(a => a.ArtifactName == artifacts[2].ArtifactName));
      Assert.IsTrue(resultModel.Any(a => a.ArtifactName == artifacts[3].ArtifactName));
    }

    [TestMethod]
    public void AddArtifact_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddArtifactRequestModel { ArtifactName = "Test Artifact" , CreateUser = resources.Username};
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:AddArtifact", requestModel, new ValidationResult());

      // When
      var result = resources.Controller.AddArtifact(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsTrue(resultModel.IsValid);
      Assert.AreEqual(0, resultModel.Messages.Count);
    }

    [TestMethod]
    public void AddArtifact_AlreadyExists_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddArtifactRequestModel { ArtifactName = "Test Role", CreateUser = resources.Username };
      var responseModel = new ValidationResult();
      responseModel.InValidate("", "The role already exists");
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:AddArtifact", requestModel, responseModel);

      // When
      var result = resources.Controller.AddArtifact(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsFalse(resultModel.IsValid);
      Assert.AreEqual(1, resultModel.Messages.Count);
      Assert.AreEqual("The role already exists", resultModel.Messages[0].Message);
    }

    [TestMethod]
    public void EditArtifact_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new EditArtifactRequestModel { ArtifactId = 1, ArtifactName = "Test Artifact", UpdateUsername = resources.Username };

      var responseModel = new ValidationResult();
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:EditArtifact", requestModel, responseModel);

      // When
      var result = resources.Controller.EditArtifact(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var resultModel = result.Value as ValidationResult;
      Assert.IsTrue(resultModel.IsValid);
      Assert.AreEqual(0, resultModel.Messages.Count);
    }

    [TestMethod]
    public void RemoveArtifact_Successful()
    {
      // Given
      var resources = new Resources();
      var requestModel = new RemoveArtifactRequestModel { ArtifactId = 1 };
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:RemoveArtifact", requestModel, "Success");

      // When 
      var result = resources.Controller.RemoveArtifact(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value.ToString());
    }

    [TestMethod]
    public void GetPermissions()
    {
      // Given
      var resources = new Resources();
      var requestModel = new FindPermissionsRequestModel { ArtifactId = 1 };
      var members = new List<PermissionModel>();
      members.Add(new PermissionModel { PermissionId = 1, ArtifactId = 1, RoleName = "Administrator" });
      members.Add(new PermissionModel { PermissionId = 2, ArtifactId = 1, RoleName = "Operations" });
      members.Add(new PermissionModel { PermissionId = 3, ArtifactId = 1, RoleName = "Programmer" });
      members.Add(new PermissionModel { PermissionId = 4, ArtifactId = 1, RoleName = "Vetting Clerk" });

      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:Find", requestModel, members);

      // When
      var result = resources.Controller.GetPermissions(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);

      var resultModel = result.Value as List<PermissionModel>;
      Assert.IsNotNull(resultModel);
      Assert.AreEqual(4, resultModel.Count);
      Assert.IsTrue(resultModel.All(m => m.ArtifactId == requestModel.ArtifactId));
    }

    [TestMethod]
    public void AddMember_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var requestModel = new AddPermissionRequestModel { ArtifactId = 1, RoleId = 1, CreateUser = resources.Username};
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:Add", requestModel, new ValidationResult());

      // When
      var result = resources.Controller.AddPermission(requestModel) as JsonResult;

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
      var requestModel = new AddPermissionRequestModel { ArtifactId = 1,  RoleId = 1, CreateUser = resources.Username };
      var validationResult = new ValidationResult();
      validationResult.InValidate("", "The member already exists");
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:Add", requestModel, validationResult);

      // When
      var result = resources.Controller.AddPermission(requestModel) as JsonResult;

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
      var requestModel = new RemovePermissionRequestModel { PermissionId = 1};
      resources.MockApiCaller.AddMockResponse("WebApi:Permissions:Remove", requestModel, "Success");

      // When 
      var result = resources.Controller.RemovePermission(requestModel) as JsonResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Success", result.Value.ToString());
    }

  }
}
