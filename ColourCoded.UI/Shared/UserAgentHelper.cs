namespace ColourCoded.UI.Shared
{
  public interface IUserAgentHelper
  {
    ClientBrowserHelper Browser { get; }
    ClientOSHelper OS { get; }
    void SetUserAgent(string userAgent);
  }
  public class UserAgentHelper : IUserAgentHelper
  {
    private string _userAgent;

    private ClientBrowserHelper _browser;
    public ClientBrowserHelper Browser
    {
      get
      {
        if (_browser == null)
        {
          _browser = new ClientBrowserHelper(_userAgent);
        }
        return _browser;
      }
    }

    private ClientOSHelper _os;
    public ClientOSHelper OS
    {
      get
      {
        if (_os == null)
        {
          _os = new ClientOSHelper(_userAgent);
        }
        return _os;
      }
    }

    public UserAgentHelper()
    {

    }

    public void SetUserAgent(string userAgent)
    {
      _userAgent = userAgent;
    }
  }
}
