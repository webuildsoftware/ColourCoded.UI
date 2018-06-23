namespace ColourCoded.UI.Areas.Orders.Models.RequestModels
{
  public class AddOrderDetailRequestModel
  {
    public int OrderId { get; set; }
    public string ItemDescription { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal Vat { get; set; }
    public decimal LineTotal { get; set; }
  }
}
