using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConnectAsdaOrdertoCaApp
{
	[XmlRoot(ElementName = "Credential")]
	public class Credential
	{
		[XmlElement(ElementName = "Identity")]
		public string Identity { get; set; }
		[XmlAttribute(AttributeName = "domain")]
		public string Domain { get; set; }
	}

	[XmlRoot(ElementName = "From")]
	public class From
	{
		[XmlElement(ElementName = "Credential")]
		public Credential Credential { get; set; }
	}

	[XmlRoot(ElementName = "To")]
	public class To
	{
		[XmlElement(ElementName = "Credential")]
		public Credential Credential { get; set; }
	}

	[XmlRoot(ElementName = "Sender")]
	public class Sender
	{
		[XmlElement(ElementName = "Credential")]
		public Credential Credential { get; set; }
		[XmlElement(ElementName = "UserAgent")]
		public string UserAgent { get; set; }
	}

	[XmlRoot(ElementName = "Header")]
	public class Header
	{
		[XmlElement(ElementName = "From")]
		public From From { get; set; }
		[XmlElement(ElementName = "To")]
		public To To { get; set; }
		[XmlElement(ElementName = "Sender")]
		public Sender Sender { get; set; }
	}

	[XmlRoot(ElementName = "Name")]
	public class Name
	{
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }
	}

	[XmlRoot(ElementName = "Country")]
	public class Country
	{
		[XmlAttribute(AttributeName = "isoCountryCode")]
		public string IsoCountryCode { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "PostalAddress")]
	public class PostalAddress
	{
		[XmlElement(ElementName = "DeliverTo")]
		public string DeliverTo { get; set; }
		[XmlElement(ElementName = "Street")]
		public string Street { get; set; }
		[XmlElement(ElementName = "City")]
		public string City { get; set; }
		[XmlElement(ElementName = "State")]
		public string State { get; set; }
		[XmlElement(ElementName = "PostalCode")]
		public string PostalCode { get; set; }
		[XmlElement(ElementName = "Country")]
		public Country Country { get; set; }
	}

	[XmlRoot(ElementName = "CountryCode")]
	public class CountryCode
	{
		[XmlAttribute(AttributeName = "isoCountryCode")]
		public string IsoCountryCode { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "TelephoneNumber")]
	public class TelephoneNumber
	{
		[XmlElement(ElementName = "CountryCode")]
		public CountryCode CountryCode { get; set; }
		[XmlElement(ElementName = "AreaOrCityCode")]
		public string AreaOrCityCode { get; set; }
		[XmlElement(ElementName = "Number")]
		public string Number { get; set; }
	}

	[XmlRoot(ElementName = "Phone")]
	public class Phone
	{
		[XmlElement(ElementName = "TelephoneNumber")]
		public TelephoneNumber TelephoneNumber { get; set; }
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
	}

	[XmlRoot(ElementName = "Address")]
	public class Address
	{
		[XmlElement(ElementName = "Name")]
		public Name Name { get; set; }
		[XmlElement(ElementName = "PostalAddress")]
		public PostalAddress PostalAddress { get; set; }
		[XmlElement(ElementName = "Phone")]
		public List<Phone> Phone { get; set; }
		[XmlElement(ElementName = "Email")]
		public string Email { get; set; }
	}

	[XmlRoot(ElementName = "ShipTo")]
	public class ShipTo
	{
		[XmlElement(ElementName = "Address")]
		public Address Address { get; set; }
	}

	[XmlRoot(ElementName = "Comments")]
	public class Comments
	{
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }
	}

	[XmlRoot(ElementName = "OrderRequestHeader")]
	public class OrderRequestHeader
	{
		[XmlElement(ElementName = "ShipTo")]
		public ShipTo ShipTo { get; set; }
		[XmlElement(ElementName = "Comments")]
		public Comments Comments { get; set; }
		[XmlAttribute(AttributeName = "orderID")]
		public string OrderID { get; set; }
		[XmlAttribute(AttributeName = "requisitionID")]
		public string RequisitionID { get; set; }
		[XmlAttribute(AttributeName = "orderDate")]
		public string OrderDate { get; set; }
		[XmlAttribute(AttributeName = "ReqShipDate")]
		public string ReqShipDate { get; set; }
		[XmlAttribute(AttributeName = "PromisedDeliveryDate")]
		public string PromisedDeliveryDate { get; set; }
		[XmlAttribute(AttributeName = "FulfilmentType")]
		public string FulfilmentType { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "ItemID")]
	public class ItemID
	{
		[XmlElement(ElementName = "AsdaItemID")]
		public string AsdaItemID { get; set; }
		[XmlElement(ElementName = "SupplierProductID")]
		public string SupplierProductID { get; set; }
	}

	[XmlRoot(ElementName = "Money")]
	public class Money
	{
		[XmlAttribute(AttributeName = "currency")]
		public string Currency { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "UnitPrice")]
	public class UnitPrice
	{
		[XmlElement(ElementName = "Money")]
		public Money Money { get; set; }
	}

	[XmlRoot(ElementName = "Extrinsic")]
	public class Extrinsic
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Description")]
	public class Description
	{
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Classification")]
	public class Classification
	{
		[XmlAttribute(AttributeName = "domain")]
		public string Domain { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "ItemDetail")]
	public class ItemDetail
	{
		[XmlElement(ElementName = "UnitPrice")]
		public UnitPrice UnitPrice { get; set; }
		[XmlElement(ElementName = "Extrinsic")]
		public Extrinsic Extrinsic { get; set; }
		[XmlElement(ElementName = "Description")]
		public Description Description { get; set; }
		[XmlElement(ElementName = "UnitOfMeasure")]
		public string UnitOfMeasure { get; set; }
		[XmlElement(ElementName = "Classification")]
		public Classification Classification { get; set; }
		[XmlElement(ElementName = "ManufacturerPartID")]
		public string ManufacturerPartID { get; set; }
	}

	[XmlRoot(ElementName = "Shipping")]
	public class Shipping
	{
		[XmlElement(ElementName = "Money")]
		public Money Money { get; set; }
	}

	[XmlRoot(ElementName = "ItemOut")]
	public class ItemOut
	{
		[XmlElement(ElementName = "ItemID")]
		public ItemID ItemID { get; set; }
		[XmlElement(ElementName = "ItemDetail")]
		public ItemDetail ItemDetail { get; set; }
		[XmlElement(ElementName = "Shipping")]
		public Shipping Shipping { get; set; }
		[XmlAttribute(AttributeName = "quantity")]
		public string Quantity { get; set; }
		[XmlAttribute(AttributeName = "lineNumber")]
		public string LineNumber { get; set; }
	}

	[XmlRoot(ElementName = "OrderRequest")]
	public class OrderRequest
	{
		[XmlElement(ElementName = "OrderRequestHeader")]
		public OrderRequestHeader OrderRequestHeader { get; set; }
		[XmlElement(ElementName = "ItemOut")]
		public List<ItemOut> ItemOut { get; set; }
	}

	[XmlRoot(ElementName = "Request")]
	public class Request
	{
		[XmlElement(ElementName = "OrderRequest")]
		public OrderRequest OrderRequest { get; set; }
		[XmlAttribute(AttributeName = "deploymentMode")]
		public string DeploymentMode { get; set; }
	}

	[XmlRoot(ElementName = "cXML")]
	public class CXML
	{
		[XmlElement(ElementName = "Header")]
		public Header Header { get; set; }
		[XmlElement(ElementName = "Request")]
		public Request Request { get; set; }
		[XmlAttribute(AttributeName = "payloadID")]
		public string PayloadID { get; set; }
		[XmlAttribute(AttributeName = "timestamp")]
		public string Timestamp { get; set; }
		[XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Lang { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
	}
}
