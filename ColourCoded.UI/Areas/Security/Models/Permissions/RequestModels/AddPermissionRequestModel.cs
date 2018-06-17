namespace ColourCoded.UI.Areas.Security.Models.Permissions.RequestModels
{
  public class AddPermissionRequestModel
  {
    public int ArtifactId { get; set; }
    public int RoleId { get; set; }
    public string CreateUser { get; set; }
  }
}
