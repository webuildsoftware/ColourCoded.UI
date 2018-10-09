using System;

namespace ColourCoded.UI.Areas.Orders.Models.RequestModels
{
  public class GetHomeOrdersPeriodRequestModel
  {
    public string Username { get; set; }
    public int CompanyProfileId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}
