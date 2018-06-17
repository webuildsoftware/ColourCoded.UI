namespace ColourCoded.UI.Areas.Security.Models.Role.RequestModels
{
  public class RemoveRoleMemberRequestModel
  {
    public int RoleId { get; set; }

    public int RoleMemberId { get; set; }
  }
}
