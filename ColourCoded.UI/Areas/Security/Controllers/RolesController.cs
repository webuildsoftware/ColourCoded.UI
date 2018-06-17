using Microsoft.AspNetCore.Mvc;
using ColourCoded.UI.Shared;
using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Areas.Security.Models.Role;
using ColourCoded.UI.Areas.Security.Models.Role.RequestModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ColourCoded.UI.Areas.Security.Controllers
{
  [Area("Security")]
  [Authorize]
  public class RolesController : Controller
  {
    protected IWebApiCaller WebApiCaller;

    public RolesController(IWebApiCaller webApiCaller)
    {
      WebApiCaller = webApiCaller;
    }

    public ViewResult Index()
    {
      return View();
    }

    public JsonResult GetAll()
    {
      return Json(WebApiCaller.PostAsync<List<RoleModel>>("WebApi:Role:GetAll", null));
    }

    [HttpPost]
    public JsonResult AddRole(AddRoleRequestModel requestModel)
    {
      requestModel.AuditUsername = string.Empty;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Role:AddRole", requestModel));
    }

    [HttpPost]
    public JsonResult EditRole(EditRoleRequestModel requestModel)
    {
      requestModel.AuditUsername = string.Empty;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Role:EditRole", requestModel));
    }

    [HttpPost]
    public JsonResult RemoveRole(RemoveRoleRequestModel requestModel)
    {
      WebApiCaller.PostAsync<string>("WebApi:Role:RemoveRole", requestModel);

      return Json("Success");
    }

    [HttpPost]
    public JsonResult GetRoleMembers(FindRoleMembersRequestModel requestModel)
    {
      return Json(WebApiCaller.PostAsync<List<RoleMemberModel>>("WebApi:Role:GetRoleMembers", requestModel));
    }

    [HttpPost]
    public JsonResult AddRoleMember(AddRoleMemberRequestModel requestModel)
    {
      requestModel.AuditUsername = string.Empty;

      return Json(WebApiCaller.PostAsync<ValidationResult>("WebApi:Role:AddRoleMember", requestModel));
    }

    [HttpPost]
    public JsonResult RemoveRoleMember(RemoveRoleMemberRequestModel requestModel)
    {
      WebApiCaller.PostAsync<string>("WebApi:Role:RemoveRoleMember", requestModel);

      return Json("Success");
    }

    public JsonResult FilterUsernames(string term)
    {
      return Json(WebApiCaller.PostAsync<List<string>>("WebApi:Role:SearchUsers", new SearchUsersRequestModel { SearchTerm = term}));
    }
  }
}