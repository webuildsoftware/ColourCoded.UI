namespace ColourCoded.UI.Areas.Orders.Models.InputModels
{
  public class OrderDetailInputModel
  {
    public string Product { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal LineTotal { get; set; }
  }
}


