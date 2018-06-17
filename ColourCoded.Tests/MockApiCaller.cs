using System.Collections.Generic;
using ColourCoded.UI.Shared.WebApiCaller;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ColourCoded.Tests
{
  public class MockApiCaller : IWebApiCaller
  {
    public List<MockApiResponseModel> Responses;

    public MockApiCaller()
    {
      Responses = new List<MockApiResponseModel>();
    }

    public void AddMockResponse(string webApiUrl, object requestModel, object responseModel)
    {
      Responses.Add(new MockApiResponseModel { WepApiUrl = webApiUrl, RequestModel = requestModel, ResponseContent = responseModel });
    }

    public T PostAsync<T>(string url, object requestModel)
    {
      return (T)Responses.Find(x => x.WepApiUrl == url && JsonConvert.SerializeObject(x.RequestModel).ToUpper() == JsonConvert.SerializeObject(requestModel).ToUpper()).ResponseContent;
    }
  }
}
