using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using ColourCoded.UI.Areas.Security.Models.Login;
using System;
using Newtonsoft.Json;

namespace ColourCoded.UI.Shared
{
  public interface ICookieHelper
  {
    Task<bool> SignIn(UserModel usermodel);
    Task<bool> SignOut();
    T GetCookie<T>(string key);
    void SetCookie(string key, string value);
    void RemoveCookie(string key);
  }

  public class CookieHelper : ICookieHelper
  {
    protected IConfiguration Configuration;
    protected IHttpContextAccessor HttpContext;

    public CookieHelper(IConfiguration configuration, IHttpContextAccessor httpContext)
    {
      HttpContext = httpContext;
      Configuration = configuration;
    }

    public async Task<bool> SignIn(UserModel usermodel)
    {
      bool result = false;

      var claims = new List<Claim>
        {
          new Claim(ClaimTypes.Name, usermodel.Username),
          new Claim("SessionToken", usermodel.ApiSessionToken),
        };

      var claimsIdentity = new ClaimsIdentity(claims, Configuration["CookieSecurityScheme"]);

      await HttpContext.HttpContext.SignInAsync(
          Configuration["CookieSecurityScheme"],
          new ClaimsPrincipal(claimsIdentity),
          new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24) });

      result = true; return result;
    }

    public async Task<bool> SignOut()
    {
      bool result = false;

      await HttpContext.HttpContext.SignOutAsync(Configuration["CookieSecurityScheme"]);

      result = true; return result;
    }

    public T GetCookie<T>(string key)
    {
      if (HttpContext.HttpContext.Request.Cookies[key] != null)
        return JsonConvert.DeserializeObject<T>(HttpContext.HttpContext.Request.Cookies[key]);
      else
        return default(T);
    }

    public void SetCookie(string key, string value)
    {
      var option = new CookieOptions();
      option.Expires = DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["CookieExpireTimeSpan"]));

      HttpContext.HttpContext.Response.Cookies.Append(key, value, option);
    }

    public void RemoveCookie(string key)
    {
      HttpContext.HttpContext.Response.Cookies.Delete(key);
    }

  }
}
