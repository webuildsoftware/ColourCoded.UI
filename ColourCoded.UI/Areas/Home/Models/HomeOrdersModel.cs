using System;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class HomeOrdersModel
  {
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
    public string CreateDate { get; set; }
    public string Total { get; set; }
    public string Status { get; set; }
  }
}
