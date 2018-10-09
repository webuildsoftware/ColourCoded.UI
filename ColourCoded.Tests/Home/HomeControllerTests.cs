using System;
using ColourCoded.UI.Areas.Home.Controllers;
using ColourCoded.UI.Areas.Security.Models.Login;
using ColourCoded.UI.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ColourCoded.Tests.Home
{
  [TestClass]
  public class HomeControllerTests
  {
    private class Resources
    {
      public string TestUsername { get; set; }
      public HomeController Controller;
      public MockApiCaller MockApiCaller;
      public Mock<ICookieHelper> MockCookieHelper;

      public Resources()
      {
        TestUsername = "testuser";
        MockApiCaller = new MockApiCaller();
        MockCookieHelper = new Mock<ICookieHelper>();
        MockCookieHelper.Setup(x => x.GetCookie<UserModel>("LoggedInUser")).Returns(new UserModel { Username = TestUsername, ApiSessionToken = Guid.NewGuid().ToString(), IsAuthenticated = true, CompanyProfileId = 1 });
        Controller = new HomeController(MockApiCaller, MockCookieHelper.Object);
      }
    }


  }
}
