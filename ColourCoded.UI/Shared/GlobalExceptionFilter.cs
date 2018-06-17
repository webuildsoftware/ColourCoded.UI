using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ColourCoded.UI.Shared
{
  public class GlobalExceptionFilter : ExceptionFilterAttribute
  {
    public override void OnException(ExceptionContext context)
    {
      var errorMessage = context.Exception.Message;
      string stackTrace = context.Exception.StackTrace;
      var innerException = context.Exception.GetBaseException();
      var webApiError = new GlobalErrorModel(errorMessage, stackTrace, innerException.Message);

      context.HttpContext.Response.StatusCode = 500;

      Log.Error(webApiError.Message);
      Log.Error("InnerException: " + webApiError.BaseMessage);
      Log.Error(webApiError.StackTrace);

      context.Result = new JsonResult(webApiError);

      base.OnException(context);
    }
  }
}
