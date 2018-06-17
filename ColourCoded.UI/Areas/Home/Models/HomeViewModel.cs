using System;
using System.Collections.Generic;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class HomeViewModel
  {
    public List<HomeOrdersModel> Orders { get; set; }
    public DateTime OrdersFromDate { get; set; }
    public DateTime OrdersToDate { get; set; }

    public string ErrorMessage { get; set; }

    public HomeViewModel()
    {
      Orders = new List<HomeOrdersModel>();
    }
  }
}
