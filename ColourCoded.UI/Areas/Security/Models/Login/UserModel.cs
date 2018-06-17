namespace ColourCoded.UI.Areas.Security.Models.Login
{
  public class UserModel
  {
    public string Username { get; set; }
    public string ApiSessionToken { get; set; }
    public bool IsAuthenticated { get; set; }
  }
}
