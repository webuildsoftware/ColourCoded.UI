using Microsoft.AspNetCore.Mvc;
using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Areas.Security.Models.Login.RequestModels;
using ColourCoded.UI.Shared;
using System;
using Newtonsoft.Json;

namespace ColourCoded.UI.Areas.Security.Controllers
{
  [Area("Security")]
  public class AuthenticateController : Controller
  {
    protected IWebApiCaller WebApiCaller;
    protected ICookieHelper CookieHelper;
    protected IUserAgentHelper UserAgentHelper;

    public AuthenticateController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper, IUserAgentHelper userAgentHelper)
    {
      WebApiCaller = webApiCaller;
      CookieHelper = cookieHelper;
      UserAgentHelper = userAgentHelper;
    }

    public ViewResult Index(AuthenticateViewModel model)
    {
      return View(model);
    }

    public ViewResult RegisterIndex(AuthenticateViewModel model)
    {
      return View("Register", model);
    }

    public ViewResult ChangePasswordIndex(AuthenticateViewModel viewModel)
    {
      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
      
      if(loggedInUser != null)
        return View("ChangePassword", viewModel);

      return View("Index", viewModel); // Login screen
    }

    public ViewResult ForgottenPasswordIndex(AuthenticateViewModel model)
    {
      return View("ForgottenPassword", model);
    }

    public JsonResult ValidateUser(string username)
    {
      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Authenticate:ValidateUsername", new ValidateUserRequestModel { Username = username }));
    }

    public JsonResult ValidateEmail(string emailAddress)
    {
      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Authenticate:ValidateUserEmail", new ValidateEmailRequestModel { EmailAddress = emailAddress }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public RedirectToActionResult Login(string username, string password)
    {
      try
      {
        string browserInfo = "Unable to determine";
        string deviceInfo = "Unable to determine";

        try
        {
          UserAgentHelper.SetUserAgent(Request.Headers["User-Agent"]);
          browserInfo = UserAgentHelper.Browser.Name + " " + UserAgentHelper.Browser.Version + " " + UserAgentHelper.Browser.Major;
          deviceInfo = UserAgentHelper.OS.Name + " " + UserAgentHelper.OS.Version;
        }
        catch {  }

        var userModel = WebApiCaller.PostAsync<UserModel>("WebApi:Authenticate:Login", new LoginRequestModel { Username = username, Password = password, Browser = browserInfo, Device = deviceInfo });

        if (userModel.Username != null)
        {
          if(userModel.IsAuthenticated)
          {
            CookieHelper.SignIn(userModel);
            CookieHelper.SetCookie("LoggedInUser", JsonConvert.SerializeObject(userModel)); // put the encrypted version of the api session token
            return RedirectToAction("Index", "Home", new { area = "Home", userModel.Username });
          }
          else
            return RedirectToAction("Index", "Authenticate", new AuthenticateViewModel { Username = username, ErrorMessage = "Invalid password. Please try again." });
        }
        else
          return RedirectToAction("Index", "Authenticate", new AuthenticateViewModel { ErrorMessage = "Username does not exist." });
      }
      catch (Exception ex)
      {
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public RedirectToActionResult Register(RegistrationInputModel inputModel)
    {
      try
      {
        if (ModelState.IsValid)
        {
          string browserInfo = "Unable to determine";
          string deviceInfo = "Unable to determine";

          try
          {
            UserAgentHelper.SetUserAgent(Request.Headers["User-Agent"]);
            browserInfo = UserAgentHelper.Browser.Name + " " + UserAgentHelper.Browser.Version + " " + UserAgentHelper.Browser.Major;
            deviceInfo = UserAgentHelper.OS.Name + " " + UserAgentHelper.OS.Version;
          }
          catch { }

          var userModel = WebApiCaller.PostAsync<UserModel>("WebApi:Authenticate:Register", new RegisterUserRequestModel
          {
            Username = inputModel.Username,
            Password = inputModel.Password,
            ConfirmPassword = inputModel.ConfirmPassword,
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
            EmailAddress = inputModel.EmailAddress,
            Browser = browserInfo,
            Device = deviceInfo
          });

          if (userModel != null)
          {
            CookieHelper.SignIn(userModel);
            CookieHelper.SetCookie("LoggedInUser", JsonConvert.SerializeObject(userModel));
            return RedirectToAction("Index", "Home", new { area = "Home" });
          }
          else
            return RedirectToAction("Register", new { ErrorMessage = "Unable to perform registration. Please contact IT support." });
        }
        else
          return RedirectToAction("Register", new { ErrorMessage = "There are validation errors with the information supplied." });
      }
      catch (Exception ex)
      {
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", Message = ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public RedirectToActionResult RequestCredentials(string emailAddress)
    {
      try
      {
        var emailExists = WebApiCaller.PostAsync<bool>("WebApi:Authenticate:ForgotPassword", new CredentialsRequestModel { EmailAddress = emailAddress });

        if (emailExists)
          return RedirectToAction("Index", "Authenticate", new AuthenticateViewModel { ErrorMessage = "Your password has been sent. Please check your email inbox." });
        else
          return RedirectToAction("ForgottenPassword", "Authenticate", new AuthenticateViewModel { ErrorMessage = "The email address is not registered." });
      }
      catch (Exception ex)
      { 
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", Message = ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public RedirectToActionResult ChangePassword(string username, string currentPassword, string newPassword)
    {
      try
      {
        var userModel = WebApiCaller.PostAsync<UserModel>("WebApi:Authenticate:Login", new LoginRequestModel { Username = username, Password = currentPassword });

        if (userModel.IsAuthenticated)
        {
          var user = WebApiCaller.PostAsync<UserModel>("WebApi:Authenticate:ChangePassword", new ChangePasswordRequestModel { Username = username, NewPassword = newPassword });
          CookieHelper.SignIn(user);

          return RedirectToAction("Index", "Home", new { area = "Home", user.Username });
        }
        else
          return RedirectToAction("ChangePasswordIndex", "Authenticate", new AuthenticateViewModel { Username = username, ErrorMessage = "Invalid current password. Please try again." });

      }
      catch (Exception ex)
      {
        return RedirectToAction("Error", "Home", new { area = "Home", IsError = "True", ex.Message, BaseMessage = ex.GetBaseException().Message });
      }
    }

    public RedirectToActionResult Logout()
    {
      CookieHelper.SignOut();
      return RedirectToAction("Index", "Authenticate");
    }
  }
}