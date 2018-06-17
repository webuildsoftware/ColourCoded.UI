namespace ColourCoded.UI.Areas.Security.Models.Role.RequestModels
{
  public class EditRoleRequestModel
  {
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public string AuditUsername { get; set; }
  }
}
