using System;
using System.Collections.Generic;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class HomeViewModel
  {
    public List<HomeOrdersModel> Orders { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ErrorMessage { get; set; }

    public HomeViewModel()
    {
      Orders = new List<HomeOrdersModel>();
    }
  }
}
