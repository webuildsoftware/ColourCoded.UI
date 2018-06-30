namespace ColourCoded.UI.Areas.Orders.Models.RequestModels
{
  public class AddCustomerOrderAddressRequestModel
  {
    public int AddressDetailId { get; set; }
    public string AddressType { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string Username { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
  }
}
