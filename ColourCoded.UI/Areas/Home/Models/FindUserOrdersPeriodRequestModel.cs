using System;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class FindUserOrdersPeriodRequestModel
  {
    public string Username { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}
