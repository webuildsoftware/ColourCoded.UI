namespace ColourCoded.UI.Areas.Orders.Models.ResponseModels
{
  public class OrderQuotationViewModel
  {
    public OrderCustomerDetailModel CustomerDetail { get; set; }
    public AddressDetailsModel DeliveryAddress { get; set; }
    public CompanyProfileModel CompanyProfile { get; set; }
    public OrderDetailModel OrderTotals { get; set; }
  }
}
