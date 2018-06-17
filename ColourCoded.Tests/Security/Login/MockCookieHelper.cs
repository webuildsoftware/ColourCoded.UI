using System.Threading.Tasks;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;

namespace ColourCoded.Tests.Security
{
  public class MockCookieHelper : ICookieHelper
  {
    public T GetCookie<T>(string key)
    {
      throw new System.NotImplementedException();
    }

    public void RemoveCookie(string key)
    {
      
    }

    public void SetCookie(string key, string value)
    {
      
    }

    public async Task<bool> SignIn(UserModel usermodel)
    {
      return true;
    }

    public async Task<bool> SignOut()
    {
      return true;
    }
  }
}
