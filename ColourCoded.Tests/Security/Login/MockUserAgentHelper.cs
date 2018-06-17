using ColourCoded.UI.Shared;

namespace ColourCoded.Tests.Security
{
  public class MockUserAgentHelper : IUserAgentHelper
  {
    public ClientBrowserHelper Browser => throw new System.Exception();

    public ClientOSHelper OS => throw new System.Exception();

    public void SetUserAgent(string userAgent)
    {
      throw new System.NotImplementedException();
    }
  }
}
