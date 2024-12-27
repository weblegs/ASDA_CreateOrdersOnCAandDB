using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAsdaOrdertoCaApp
{
    public class Item
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal ShippingTaxPrice { get; set; }
    }

    public class Order
    {
        public int ProfileID { get; set; }
        public string SiteOrderID { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTaxPrice { get; set; }
        public decimal TotalShippingPrice { get; set; }
        public decimal TotalShippingTaxPrice { get; set; }

        public DateTime EstimatedShipDateUtc { get; set; }
        public string CheckoutStatus { get; set; }
        public string SiteName { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public string BuyerUserId { get; set; }
        public string BuyerEmailAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentCreditCardLast4 { get; set; }
        public string PaymentMerchantReferenceNumber { get; set; }
        public string ShippingTitle { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingSuffix { get; set; }
        public string ShippingCompanyName { get; set; }
        public string ShippingCompanyJobTitle { get; set; }
        public string ShippingDaytimePhone { get; set; }
        public string ShippingEveningPhone { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingStateOrProvince { get; set; }
        public string SecondarySiteOrderID { get; set; }

        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public string BillingTitle { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingSuffix { get; set; }
        public string BillingCompanyName { get; set; }
        public string BillingCompanyJobTitle { get; set; }
        public string BillingDaytimePhone { get; set; }
        public string BillingEveningPhone { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingStateOrProvince { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public List<Item> Items { get; set; }
    }
    public class CreateOrderRequest
    {
        public Order Order { get; set; }
    }
}
