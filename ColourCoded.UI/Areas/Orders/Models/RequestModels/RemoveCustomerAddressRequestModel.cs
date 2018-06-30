namespace ColourCoded.UI.Areas.Orders.Models.RequestModels
{
  public class RemoveCustomerAddressRequestModel
  {
    public int CustomerId { get; set; }
    public int AddressDetailId { get; set; }
    public string Username { get; set; }
  }
}
