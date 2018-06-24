namespace ColourCoded.UI.Areas.Security.Models.Role.RequestModels
{
  public class AddRoleMemberRequestModel
  {
    public int RoleId { get; set; }
    public string Username { get; set; }
    public string CreateUser { get; set; }
  }
}
