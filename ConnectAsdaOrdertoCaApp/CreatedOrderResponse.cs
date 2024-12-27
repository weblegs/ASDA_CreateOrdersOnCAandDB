using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAsdaOrdertoCaApp
{
    public class OrderCreatedResponse
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public int UserDataPresent { get; set; }
        public object UserDataRemovalDateUTC { get; set; }
        public object SiteAccountID { get; set; }
        public string SiteOrderID { get; set; }
        public object SecondarySiteOrderID { get; set; }
        public object SellerOrderID { get; set; }
        public object CheckoutSourceID { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime ImportDateUtc { get; set; }
        public object PublicNotes { get; set; }
        public string PrivateNotes { get; set; }
        public string SpecialInstructions { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxPrice { get; set; }
        public double TotalShippingPrice { get; set; }
        public double TotalShippingTaxPrice { get; set; }
        public double TotalInsurancePrice { get; set; }
        public double TotalGiftOptionPrice { get; set; }
        public double TotalGiftOptionTaxPrice { get; set; }
        public double AdditionalCostOrDiscount { get; set; }
        public DateTime EstimatedShipDateUtc { get; set; }
        public object DeliverByDateUtc { get; set; }
        public object RequestedShippingCarrier { get; set; }
        public object RequestedShippingClass { get; set; }
        public object ResellerID { get; set; }
        public int FlagID { get; set; }
        public object FlagDescription { get; set; }
        public object OrderTags { get; set; }
        public string DistributionCenterTypeRollup { get; set; }
        public string CheckoutStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime CheckoutDateUtc { get; set; }
        public DateTime PaymentDateUtc { get; set; }
        public object ShippingDateUtc { get; set; }
        public string BuyerUserId { get; set; }
        public string BuyerEmailAddress { get; set; }
        public bool BuyerEmailOptIn { get; set; }
        public string OrderTaxType { get; set; }
        public string ShippingTaxType { get; set; }
        public string GiftOptionsTaxType { get; set; }
        public string PaymentMethod { get; set; }
        public object PaymentTransactionID { get; set; }
        public object PaymentPaypalAccountID { get; set; }
        public string PaymentCreditCardLast4 { get; set; }
        public object PaymentMerchantReferenceNumber { get; set; }
        public object ShippingTitle { get; set; }
        public string ShippingFirstName { get; set; }
        public object ShippingLastName { get; set; }
        public object ShippingSuffix { get; set; }
        public object ShippingCompanyName { get; set; }
        public object ShippingCompanyJobTitle { get; set; }
        public string ShippingDaytimePhone { get; set; }
        public object ShippingEveningPhone { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public object ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public object ShippingStateOrProvince { get; set; }
        public object ShippingStateOrProvinceName { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public object BillingTitle { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingSuffix { get; set; }
        public object BillingCompanyName { get; set; }
        public object BillingCompanyJobTitle { get; set; }
        public string BillingDaytimePhone { get; set; }
        public object BillingEveningPhone { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; }
        public object BillingStateOrProvince { get; set; }
        public object BillingStateOrProvinceName { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public object PromotionCode { get; set; }
        public double PromotionAmount { get; set; }
    }
}
