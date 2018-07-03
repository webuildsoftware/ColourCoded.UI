using System.Collections.Generic;
using ColourCoded.UI.Areas.Orders.Models.ResponseModels;

namespace ColourCoded.UI.Areas.Home.Models
{
  public class OrderQuotationViewModel
  {
    public OrderCustomerDetailModel CustomerDetail { get; set; }
    public AddressDetailsModel DeliveryAddress { get; set; }
    public CompanyProfileModel CompanyProfile { get; set; }
    public OrderDetailModel OrderTotals { get; set; }
  }
}
