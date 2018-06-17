using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Areas.Security.Models.Permissions;
using ColourCoded.UI.Areas.Security.Models.Permissions.RequestModels;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColourCoded.UI.Areas.Security.Controllers
{
  [Area("Security")]
  [Authorize]
  public class PermissionsController : Controller
  {
    protected IWebApiCaller WebApiCaller;
    protected ICookieHelper CookieHelper;

    public PermissionsController(IWebApiCaller webApiCaller, ICookieHelper cookieHelper)
    {
      WebApiCaller = webApiCaller;
      CookieHelper = cookieHelper;
    }

    public ViewResult Index()
    {
      return View();
    }

    public JsonResult GetArtifacts()
    {
      return Json(WebApiCaller.PostAsync<List<ArtifactModel>>("WebApi:Permissions:GetAllArtifacts", null));
    }

    [HttpPost]
    public JsonResult AddArtifact(AddArtifactRequestModel requestModel)
    {
      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
      requestModel.CreateUser = loggedInUser.Username;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Permissions:AddArtifact", requestModel));
    }

    [HttpPost]
    public JsonResult EditArtifact(EditArtifactRequestModel requestModel)
    {
      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
      requestModel.UpdateUsername = loggedInUser.Username;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Permissions:EditArtifact", requestModel));
    }

    [HttpPost]
    public JsonResult RemoveArtifact(RemoveArtifactRequestModel requestModel)
    {
      WebApiCaller.PostAsync<string>("WebApi:Permissions:RemoveArtifact", requestModel);

      return Json("Success");
    }

    [HttpPost]
    public JsonResult GetPermissions(FindPermissionsRequestModel requestModel)
    {
      return Json(WebApiCaller.PostAsync<List<PermissionModel>>("WebApi:Permissions:Find", requestModel));
    }

    [HttpPost]
    public JsonResult AddPermission(AddPermissionRequestModel requestModel)
    {
      var loggedInUser = CookieHelper.GetCookie<UserModel>("LoggedInUser");
      requestModel.CreateUser = loggedInUser.Username;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Permissions:Add", requestModel));
    }

    [HttpPost]
    public JsonResult RemovePermission(RemovePermissionRequestModel requestModel)
    {
      WebApiCaller.PostAsync<string>("WebApi:Permissions:Remove", requestModel);

      return Json("Success");
    }

  }
}