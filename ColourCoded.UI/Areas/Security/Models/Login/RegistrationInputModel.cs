using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ColourCoded.UI.Areas.Security.Models.Login
{
  public class RegistrationInputModel
  {
    [Required(ErrorMessage = "The username is required")]
    [MinLength(6)]
    public string Username { get; set; }

    [Required(ErrorMessage = "The password is required")]
    [PasswordPropertyText(true)]
    [MinLength(6)]
    public string Password { get; set; }

    [Required(ErrorMessage = "The password is required")]
    [PasswordPropertyText(true)]
    [MinLength(6)]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "The first name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "The last name is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "The email address is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string EmailAddress { get; set; }

    public string ErrorMessage { get; set; }
  }
}