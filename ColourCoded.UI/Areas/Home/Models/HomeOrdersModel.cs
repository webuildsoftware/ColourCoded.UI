using System;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class HomeOrdersModel
  {
    public int OrderNo { get; set; }
    public string CustomerName { get; set; }
    public string DeliveryDate { get; set; }
    public string Total { get; set; }
  }
}
