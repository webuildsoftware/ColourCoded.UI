using ColourCoded.UI.Areas.Security.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Areas.Security.Models.Login.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ColourCoded.UI.Shared;
using System;

namespace ColourCoded.Tests.Security.Login
{
  [TestClass]
  public class AuthenticateControllerTests
  {
    private class Resources
    {
      public AuthenticateController Controller;
      public MockApiCaller MockApiCaller;
      public MockCookieHelper MockCookieHelper;
      public MockUserAgentHelper MockUserAgentHelper;

      public Resources()
      {
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new MockCookieHelper();
        MockUserAgentHelper = new MockUserAgentHelper();
        Controller = new AuthenticateController(MockApiCaller, MockCookieHelper, MockUserAgentHelper);
      }
    }

    [TestMethod]
    public void Change_Password_Index()
    {
      // Given
      var resources = new Resources();
      var viewModel = new AuthenticateViewModel { Username = "testuser"};

      // When
      var result = resources.Controller.ChangePasswordIndex(viewModel) as ViewResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ViewName);
    }

    [TestMethod]
    public void Change_Password_Successful()
    {
      // Given
      const string username = "testuser";
      var resources = new Resources();
      var viewModel = new AuthenticateViewModel { Username = username };
      var currentPassword = "oldPassword";
      var newPassword = "newPassword";

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Login", new LoginRequestModel
      {
        Username = username,
        Password = currentPassword,
      }, new UserModel { Username = username, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });


      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ChangePassword", new ChangePasswordRequestModel
      {
        Username = username,
        NewPassword = newPassword,
      }, new UserModel { Username = viewModel.Username, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });


      // When
      var result = resources.Controller.ChangePassword(username, currentPassword, newPassword) as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Home", result.ControllerName);
    }

    [TestMethod]
    public void Change_Password_IncorrectPassword()
    {
      // Given
      const string username = "testuser";
      var resources = new Resources();
      var viewModel = new AuthenticateViewModel { Username = username };
      var currentPassword = "oldPassword";
      var newPassword = "newPassword";

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Login", new LoginRequestModel
      {
        Username = username,
        Password = currentPassword,
      }, new UserModel { Username = username, IsAuthenticated = false });

      // When
      var result = resources.Controller.ChangePassword(username, currentPassword, newPassword) as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("ChangePasswordIndex", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName);
    }

    [TestMethod]
    public void Register_Save_Successful()
    {
      // Given
      var resources = new Resources();
      var inputModel = new RegistrationInputModel
      {
        Username = "testuser",
        Password = "mypassword",
        ConfirmPassword = "mypassword",
        FirstName = "Test",
        LastName = "User",
        EmailAddress = "someemail@address.com",
      };

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Register", new RegisterUserRequestModel
      {
        Username = inputModel.Username,
        Password = inputModel.Password,
        ConfirmPassword = inputModel.ConfirmPassword,
        FirstName = inputModel.FirstName,
        LastName = inputModel.LastName,
        EmailAddress = inputModel.EmailAddress,
        Browser = "Unable to determine",
        Device = "Unable to determine",
      }, new UserModel { Username = "1", ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true } );

      // When
      var result = resources.Controller.Register(inputModel) as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Home", result.ControllerName);
    }

    [TestMethod]
    public void Logout_Successful()
    {
      // Given
      var resources = new Resources();

      // When
      var result = resources.Controller.Logout() as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName);
    }

    [TestMethod]
    public void ValidateUser_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ValidateUsername", new ValidateUserRequestModel { Username = "testusername" }, new ValidationResult());

      // When
      var result = resources.Controller.ValidateUser("testusername") as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value as ValidationResult;
      Assert.IsTrue(model.IsValid);
    }

    [TestMethod]
    public void ValiidateUser_AlreadyExists_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var validationResult = new ValidationResult();
      validationResult.InValidate("", "Username already exists");
      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ValidateUsername", new ValidateUserRequestModel { Username = "testusername" }, validationResult);

      // When
      var result = resources.Controller.ValidateUser("testusername") as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value as ValidationResult;
      Assert.IsFalse(model.IsValid);
      Assert.AreEqual(1, model.Messages.Count);
      Assert.AreEqual("Username already exists", model.Messages[0].Message);
    }

    [TestMethod]
    public void ValidateEmail_Successful_ValidationResult()
    {
      // Given
      var resources = new Resources();

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ValidateUserEmail", new ValidateEmailRequestModel { EmailAddress = "test@gmail.com" }, new ValidationResult());

      // When
      var result = resources.Controller.ValidateEmail("test@gmail.com") as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value as ValidationResult;
      Assert.IsTrue(model.IsValid);
    }

    [TestMethod]
    public void ValidateEmail_AlreadyExists_ValidationResult()
    {
      // Given
      var resources = new Resources();
      var validationResult = new ValidationResult();
      validationResult.InValidate("", "Already linked to another user.");
      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ValidateUserEmail", new ValidateEmailRequestModel { EmailAddress = "test@gmail.com" }, validationResult);

      // When
      var result = resources.Controller.ValidateEmail("test@gmail.com") as JsonResult;

      // Then
      Assert.IsNotNull(result);
      var model = result.Value as ValidationResult;
      Assert.IsFalse(model.IsValid);
      Assert.AreEqual(1, model.Messages.Count);
      Assert.AreEqual("Already linked to another user.", model.Messages[0].Message);
    }

    [TestMethod]
    public void Login_Successful()
    {
      // Given
      var resources = new Resources();

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Login", new LoginRequestModel
      {
        Username = "testuser",
        Password = "password",
        Browser = "Unable to determine",
        Device = "Unable to determine",
      }, new UserModel { Username = "testuser", ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true });

      // When
      var result = resources.Controller.Login("testuser", "password") as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Home", result.ControllerName);
    }

    [TestMethod]
    public void Login_Password_Unsuccessful()
    {
      // Given
      var resources = new Resources();
      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Login", 
        new LoginRequestModel {
        Username = "testuser",
        Password = "password",
        Browser = "Unable to determine",
        Device = "Unable to determine",
      }, new UserModel
      {
        Username = "1",
        ApiSessionToken = Guid.NewGuid().ToString(),
        IsAuthenticated = false
      });

      // When
      var result = resources.Controller.Login("testuser", "password") as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName);
      Assert.AreEqual("Invalid password. Please try again.", result.RouteValues["ErrorMessage"]);
    }

    [TestMethod]
    public void Login_Username_Unsuccessful()
    {
      // Given
      var resources = new Resources();
      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:Login", 
        new LoginRequestModel {
          Username = "testuser",
          Password = "password",
          Browser = "Unable to determine",
          Device = "Unable to determine",
        }, new UserModel { Username = null });

      // When
      var result = resources.Controller.Login("testuser", "password") as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName);
      Assert.AreEqual("Username does not exist.", result.RouteValues["ErrorMessage"]);
    }

    [TestMethod]
    public void ForgottenPassword_SendEmail_Successful()
    {
      // Given
      var resources = new Resources();

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ForgotPassword", new CredentialsRequestModel { EmailAddress = "test@gmail.com"}, true);

      // When
      var result = resources.Controller.RequestCredentials("test@gmail.com") as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("Index", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName); // Password has been sent. Please check your inbox.
      Assert.AreEqual("Your password has been sent. Please check your email inbox.", result.RouteValues["ErrorMessage"]);
    }

    [TestMethod]
    public void ForgottenPassword_NoEmail_Unsuccessful()
    {
      // Given
      var resources = new Resources();

      resources.MockApiCaller.AddMockResponse("WebApi:Authenticate:ForgotPassword", new CredentialsRequestModel { EmailAddress = "test@gmail.com" }, false);

      // When
      var result = resources.Controller.RequestCredentials("test@gmail.com") as RedirectToActionResult;

      // Then
      Assert.IsNotNull(result);
      Assert.AreEqual("ForgottenPassword", result.ActionName);
      Assert.AreEqual("Authenticate", result.ControllerName); // Password has been sent. Please check your inbox.
      Assert.AreEqual("The email address is not registered.", result.RouteValues["ErrorMessage"]);
    }
  }
}

