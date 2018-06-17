
namespace ColourCoded.UI.Shared
{
  public class GlobalErrorModel
  {
    public bool IsError { get; set; }
    public string Message { get; set; }
    public string BaseMessage { get; set; }
    public string StackTrace { get; set; }

    public GlobalErrorModel()
    {
      Message = "An Error occurred";
      IsError = true;
    }

    public GlobalErrorModel(string message, string stackTrace, string innerException)
    {
      Message = message;
      StackTrace = stackTrace;
      BaseMessage = innerException;

      IsError = true;
    }

  }
}
