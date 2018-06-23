namespace ColourCoded.UI.Areas.Orders.Models
{
  public class OrderDetailInputModel
  {
    public string Product { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
  }
}


