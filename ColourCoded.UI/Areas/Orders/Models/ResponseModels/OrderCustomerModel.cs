using System;

namespace ColourCoded.UI.Areas.Orders.Models.ResponseModels
{
  public class AddCustomerOrderModel
  {
    public int CustomerId { get; set; }
    public int ContactId { get; set; }
    public int OrderId { get; set; }
    public string OrderNo { get; set; }
  }
}
