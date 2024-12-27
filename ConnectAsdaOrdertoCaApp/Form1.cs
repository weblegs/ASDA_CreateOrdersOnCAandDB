using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Renci.SshNet;
using Renci.SshNet.Common;
using RestSharp;
using RestSharp.Serialization.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using WeblegsClasses;

namespace ConnectAsdaOrdertoCaApp
{
    public partial class Form1 : Form
    {
        string accesstoken = string.Empty;
        string asdaOrderXml = string.Empty;
        string ChannelJson = string.Empty;
        int CA_orderID = 0;
        int sequencenumber = 0;
        List<ProcessedAsdaFile> processedOrders = new List<ProcessedAsdaFile>();
        StreamWriter writerInvoiceOrders = null;
        SqlConnection con = new SqlConnection();
        string errorcontainingfiles = string.Empty;
        List<string> filetobeProcessed = new List<string>();
        string currentfilename = string.Empty;
        int retryConnect = 1;
        string errorfile = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {        
                  

            try
            {
                con.ConnectionString = ConfigurationManager.AppSettings["DBConn"].ToString();
                processedOrders = ReadFiles("OrderDetails");
                
                filetobeProcessed = GetFileNameandDownloadFromSFTP(); // For Live

                //filetobeProcessed.Add("cXML_OrderRequest_Y500054996.xml"); // For Local
                


                if (filetobeProcessed.Any())
                {
                    string fileName = DateTime.Now.ToString().Replace(":", "").Replace("-", "").Replace("/", "") + ".csv";
                    writerInvoiceOrders = new StreamWriter(Application.StartupPath + "/OrderDetails/" + fileName);
                    errorcontainingfiles = String.Join(",", filetobeProcessed);

                    Accesstoken accesstokens = GetAccessToken();
                    accesstoken = accesstokens.access_token;

                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString = ConfigurationManager.AppSettings["DBConn"].ToString();

                    DataTable dataasdacin = new DataTable();


                    using (SqlCommand cmd = new SqlCommand("select * from SKU_ASDACIN_MAPPING"))
                    {
                        cmd.Connection = connection;
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dataasdacin);
                    }


                    DataTable dt = new DataTable();
                    dt.Columns.Add("AsdaOrderID");
                    dt.Columns.Add("AsdaItemID");
                    dt.Columns.Add("ChannelOrderID");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("Price");
                    dt.Columns.Add("ShippingCost");
                    dt.Columns.Add("LineNumber");
                    dt.Columns.Add("FirstName");
                    dt.Columns.Add("LastName");
                    dt.Columns.Add("Email");
                    dt.Columns.Add("PhoneNumber");
                    dt.Columns.Add("Street");
                    dt.Columns.Add("City");
                    dt.Columns.Add("Postalcode");
                    dt.Columns.Add("Country");
                    dt.Columns.Add("Ordertype");
                    dt.Columns.Add("ReceviedAsdaXml");
                    dt.Columns.Add("ReceviedCAResponse");
                    dt.Columns.Add("Updatedon");
                    dt.Columns.Add("IsAcknowledge");
                    dt.Columns.Add("IsOrderCanceled");
                    dt.Columns.Add("IsPreadvicegenerated");
                    dt.Columns.Add("IsLabelSpecificationGenerated");
                    dt.Columns.Add("IsPartialConfirmShipmentGenerated");
                    dt.Columns.Add("IsFullConfirmShipmentGenerated");
                    dt.Columns.Add("SequenceValue");
                    dt.Columns.Add("Position");
                    dt.Columns.Add("IsMarkedAsShipped");
                    dt.Columns.Add("RequsitionID");

                    CXML cXML = null;

                    foreach (var stpfile in filetobeProcessed)
                    {
                        errorfile = stpfile;
                        cXML = GetandReadXml(Application.StartupPath + "/DownloadedFile/" + stpfile);

                        currentfilename = stpfile;

                        if (cXML == null)
                        {
                            StreamWriter writer = new StreamWriter(Application.StartupPath + "/SkippedFile/Skippedfilelog.txt", true);
                            writer.WriteLine(stpfile + "  " + DateTime.Now);
                            writer.Close();
                            writerInvoiceOrders.WriteLine(stpfile);
                            //SendErrorMail("Wrong XML recevied from ASDA [FileName] - " + stpfile);
                            SendMail("kamal.matrid77991@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Wrong XML recevied from ASDA [FileName] - " + stpfile);
                            SendMail("ramandeep.matrid33789@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Wrong XML recevied from ASDA [FileName] - " + stpfile);
                           

                            continue;
                        }

                        List<Item> Items = new List<Item>();
                        decimal totalorderprice = 0;
                        decimal totalshippingprice = 0;
                        Order order = new Order();
                        order.ProfileID = 12028221;

                        #region customer Address

                        //Add code for last Name if we receive last name from final XML

                        order.ShippingFirstName = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.DeliverTo;
                        order.ShippingAddressLine1 = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Street;
                        order.ShippingCity = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.City;
                        order.ShippingCountry = "GB";
                        order.ShippingStateOrProvince = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.State;
                        order.ShippingPostalCode = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.PostalCode;
                        if (cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone.Count > 0)
                            order.ShippingDaytimePhone = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone[0].TelephoneNumber.Number;


                        order.BillingFirstName = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.DeliverTo;
                        order.BillingAddressLine1 = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Street;
                        order.BillingCity = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.City;
                        order.BillingCountry = "GB";
                        order.BillingStateOrProvince = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.State;
                        order.BillingPostalCode = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.PostalCode;

                        if (cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone.Count > 0)
                            order.BillingDaytimePhone = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone[0].TelephoneNumber.Number;

                        #endregion

                        #region OrderCriteria

                        order.CheckoutStatus = "Completed";
                        order.PaymentStatus = "Cleared";
                        order.ShippingStatus = "Unshipped";
                        order.EstimatedShipDateUtc = Convert.ToDateTime(cXML.Request.OrderRequest.OrderRequestHeader.PromisedDeliveryDate);

                        if (cXML.Request.OrderRequest.OrderRequestHeader.FulfilmentType == "Delivery")
                            order.PaymentMethod = "ASDADelivery";
                        else if (cXML.Request.OrderRequest.OrderRequestHeader.FulfilmentType == "Collect")
                            order.PaymentMethod = "ASDACollect";

                        #endregion
                        bool skumap = true;
                        #region ItemDetails
                        foreach (var items in cXML.Request.OrderRequest.ItemOut)
                        {
                            Item item = new Item();
                            //DataSet Inventorymapping = Operations.ReadCSVFile(Application.StartupPath, "InventoryDetails.csv");//for live


                            var SkuMapping = (from mapds in dataasdacin.AsEnumerable()
                                              where mapds.Field<string>("ASDACIN") == items.ItemID.AsdaItemID
                                              select new
                                              {
                                                  SKU = mapds.Field<string>("Sku")
                                              }).ToList();

                            if (SkuMapping == null || SkuMapping.Count == 0)
                            {
                                //SendErrorMail("Skipped due to missing mapping------------------" + stpfile + "        ------+  ----------- ASDACIN: " + items.ItemID.AsdaItemID + "    -------------------" + DateTime.Now);
                                SendMail("kamal.matrid77991@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Skipped due to missing mapping------------------" + stpfile + "        ------+  ----------- ASDACIN: " + items.ItemID.AsdaItemID + "    -------------------" + DateTime.Now);
                                SendMail("ramandeep.matrid33789@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Skipped due to missing mapping------------------" + stpfile + "        ------+  ----------- ASDACIN: " + items.ItemID.AsdaItemID + "    -------------------" + DateTime.Now);
                               

                            }
                            else
                            {
                                item.Sku = SkuMapping[0].SKU;
                                item.UnitPrice = Convert.ToDecimal("6.50");
                                totalorderprice = totalorderprice + (Convert.ToDecimal("6.50") * Convert.ToInt32(items.Quantity));
                                item.Quantity = Convert.ToInt32(items.Quantity);
                                item.ShippingPrice = Convert.ToDecimal(items.Shipping.Money.Text);
                                totalshippingprice = totalshippingprice + Convert.ToDecimal(items.Shipping.Money.Text);
                                Items.Add(item);
                            }
                        }
                        #endregion
                        if (!skumap)
                            continue;

                        #region ChannelPostOrderObject
                        order.BuyerEmailAddress = string.IsNullOrEmpty(cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email)? "na@na.com" : cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email;
                        order.TotalShippingPrice = totalshippingprice;
                        order.TotalPrice = totalorderprice + totalshippingprice;
                        order.SiteName = "ASDA";
                        order.SiteOrderID = cXML.Request.OrderRequest.OrderRequestHeader.RequisitionID;
                        order.SecondarySiteOrderID = cXML.Request.OrderRequest.OrderRequestHeader.OrderID.ToString();
                        order.Items = Items;
                        #endregion


                        //bool issucessfullyCompleted = true; // For Local
                        bool issucessfullyCompleted = CreateOrder(order); // For Live

                        if (CA_orderID == 0)
                        {
                            //SendErrorMail(" CA Order Id is 0 for " + stpfile + "-------------------" + DateTime.Now);
                            //SendMail("kamal.matrid77991@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), " CA Order Id is 0 for " + stpfile + "-------------------" + DateTime.Now);
                            //SendMail("ramandeep.matrid33789@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), " CA Order Id is 0 for " + stpfile + "-------------------" + DateTime.Now);
                            //SendMail("medfosys.186@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), " CA Order Id is 0 for " + stpfile + "-------------------" + DateTime.Now);

                        }
                        if (issucessfullyCompleted)
                        {
                            int counter = 0;

                            foreach (var items in cXML.Request.OrderRequest.ItemOut)
                            {
                                counter++;
                                DataRow row = dt.NewRow();
                                row[0] = cXML.Request.OrderRequest.OrderRequestHeader.OrderID.ToString();
                                row[1] = items.ItemID.AsdaItemID;
                                row[2] = CA_orderID;
                                //row[2] = 3083670;
                                if (CA_orderID == 0)
                                {
                                    SendMail("kamal.matrid77991@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "CA Order Id is 0 for order: " + cXML.Request.OrderRequest.OrderRequestHeader.OrderID.ToString());
                                    SendMail("ramandeep.matrid33789@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "CA Order Id is 0 for order: " + cXML.Request.OrderRequest.OrderRequestHeader.OrderID.ToString());
                                }
                                //row[2] = 2663574;
                                row[3] = Convert.ToInt32(items.Quantity);
                                row[4] = items.ItemDetail.UnitPrice.Money.Text;
                                row[5] = items.Shipping.Money.Text.ToString();
                                row[6] = Convert.ToInt32(items.LineNumber);
                                row[7] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.DeliverTo;
                                row[8] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.DeliverTo;
                                row[9] = string.IsNullOrEmpty(cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email) ? "na@na.com" : cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Email;

                                if (cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone.Count > 0)
                                    row[10] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.Phone[0].TelephoneNumber.Number;

                                row[11] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Street;
                                row[12] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.City;
                                row[13] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.PostalCode;
                                row[14] = cXML.Request.OrderRequest.OrderRequestHeader.ShipTo.Address.PostalAddress.Country.Text;
                                row[15] = cXML.Request.OrderRequest.OrderRequestHeader.FulfilmentType;
                                row[16] = !string.IsNullOrEmpty(asdaOrderXml) ? asdaOrderXml : "Mannual";
                                row[17] = !string.IsNullOrEmpty(ChannelJson) ? ChannelJson : "Mannual";
                                row[18] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                                row[19] = false;
                                row[20] = false;

                                if (cXML.Request.OrderRequest.OrderRequestHeader.FulfilmentType == "Delivery")
                                {
                                    row[21] = true;
                                    row[22] = true;
                                }
                                else
                                {
                                    row[21] = false;
                                    row[22] = false;
                                }
                                row[23] = false;
                                row[24] = false;
                                row[25] = sequencenumber;
                                row[26] = counter;
                                row[27] = false;
                                row[28] = cXML.Request.OrderRequest.OrderRequestHeader.RequisitionID.ToString();
                                dt.Rows.Add(row);
                                writerInvoiceOrders.WriteLine(stpfile);
                            }
                        }
                    }
                    writerInvoiceOrders.Close();
                    bool isfeededInDatabase = InsertIntoDatabase(dt);

                    if (!isfeededInDatabase)
                    {
                        //SendErrorMail("Error in inserting data to database with file name:-" + currentfilename);
                        //SendErrorMail("Error in inserting data to database with file name:-" + currentfilename);
                        SendMail("kamal.matrid77991@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Error in inserting data to database with file name:-" + currentfilename);
                        SendMail("ramandeep.matrid33789@gmail.com", "Message from ASDA Create Order On Channel App - " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm"), "Error in inserting data to database with file name:-" + currentfilename);
                       

                    }

                }
                else
                {
                    SendMail("kamal.matrid77991@gmail.com", "ConnectAsdaOrdertoCaApp", "No Any New OrdersFile Found !");                   
                    SendMail("ramandeep.matrid33789@gmail.com", "ConnectAsdaOrdertoCaApp", "No Any New OrdersFile Found !");                   
                                   
                }
            }
            catch (Exception ex)
            {

                StreamWriter writer = new StreamWriter(Application.StartupPath + "/ASDAlog.txt", true);
                writer.WriteLine(ex.Message + "   " + ex.StackTrace.ToString() + "        ------" + DateTime.Now + "----" + errorfile);
                writer.Close();
                SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp [Form_Load]", ex.Message + "   " + errorcontainingfiles.ToString());               
                SendMail("ramandeep.matrid33789@gmail.com", "Error in ConnectAsdaOrdertoCaApp [Form_Load]", ex.Message + "   " + errorcontainingfiles.ToString());               
                  
            }
            timer1.Interval = 2000;
            timer1.Start();
        }

        #region SEND MAIL

       

        public static bool SendMail(string SendTo, string Subject, string Body)
        {
            try
            {
                // Initialize Gmail API service
                var service = InitializeGmailService();

                // Create an email message
                var message = EmailHelper.CreateEmail(SendTo, "support@weblegs.co.uk", Subject, Body);

                // Send the email via Gmail API
                EmailHelper.SendMessage(service, "me", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
            return true;
        }

        // Method to initialize the Gmail API service
        static GmailService InitializeGmailService()
        {
            UserCredential credential;

            // Correct path separator for credentials.json
            var credPath = Path.Combine(Application.StartupPath, "credentials.json");

            using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                //string tokenPath = "token.json";
                string tokenPath = Path.Combine(Application.StartupPath + "/token.json/");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { GmailService.Scope.GmailSend },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true)).Result;

                Console.WriteLine("Credential file saved to: " + tokenPath);
            }

            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gmail API .NET Quickstart",
            });
        }

        // Helper class for creating and sending the email
        public static class EmailHelper
        {
            public static Google.Apis.Gmail.v1.Data.Message CreateEmail(string to, string from, string subject, string bodyText)
            {
                var email = new Google.Apis.Gmail.v1.Data.Message();
                email.Raw = Base64UrlEncode(CreateEmailRaw(to, from, subject, bodyText));
                return email;
            }

            private static string CreateEmailRaw(string to, string from, string subject, string bodyText)
            {
                var rawEmail = new StringBuilder();
                rawEmail.AppendLine($"To: {to}");
                rawEmail.AppendLine($"From: {from}");
                rawEmail.AppendLine($"Subject: {subject}");
                rawEmail.AppendLine("Content-Type: text/html; charset=utf-8");
                rawEmail.AppendLine("Content-Transfer-Encoding: quoted-printable");
                rawEmail.AppendLine();
                rawEmail.AppendLine(bodyText);
                return rawEmail.ToString();
            }

            private static string Base64UrlEncode(string input)
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(inputBytes)
                  .Replace('+', '-')
                  .Replace('/', '_')
                  .Replace("=", "");
            }

            public static void SendMessage(GmailService service, string userId, Google.Apis.Gmail.v1.Data.Message email)
            {
                try
                {
                    service.Users.Messages.Send(email, userId).Execute();
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    throw;
                }
            }
        }


        

        #endregion
        public bool CreateOrder(Order order)
        {
            try
            {
                CreateOrderRequest createOrderRequest = new CreateOrderRequest();
                createOrderRequest.Order = order;
                string Url = "https://api.channeladvisor.com/v1/Orders/Create?access_token=" + accesstoken;
                string responses = PostOrderData(Url, createOrderRequest);
                ChannelJson = responses;
                OrderCreatedResponse orderCreatedres = null;

                if (!string.IsNullOrEmpty(responses))
                {
                    orderCreatedres = JsonConvert.DeserializeObject<OrderCreatedResponse>(responses);
                    CA_orderID = orderCreatedres.ID;
                    return true;
                }
                else
                {
                    //SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp [CreateOrder] for FileName--" + currentfilename, "Unable to CreateOrder on Channel Advisor or May be Order placed but data is not inserted into table");                  
                    StreamWriter writer = new StreamWriter(Application.StartupPath + "/AlreadyCreated.txt", true);
                    writer.WriteLine("Already created------------------" + order.SecondarySiteOrderID + "        ------" + DateTime.Now);
                    writer.Close();
                    return false;

                }

            }
            catch (Exception ex)
            {
                SendMail("kamal.matrid77991@gmail.com", "Error in CreateOrder " , ex.Message+DateTime.Now);
                SendMail("ramandeep.matrid33789@gmail.com", "Error in CreateOrder " , ex.Message+DateTime.Now);
               
                return false;
            }
        }
        public static Accesstoken GetAccessToken()
        {
            string URL = "oauth2/token";
            string refreshtoken = "a7ux0ZzwOaF4CSksc36kYmJCqTYdiuBTT47Tka94-uE";
            string applicationid = "gpiwrw4f9jhn7zkz0f7m9v7l1pdurgyq";
            string secretid = "24N9ocV6AEaii4IfGWDMJw";
            string authorize = applicationid + ":" + secretid;
            string encode = EncodeTo64(authorize);
            string encodeauthorize = "Basic " + encode;
            Accesstoken accesstoken = PostForAccesstoken(URL, refreshtoken, encodeauthorize);
            return accesstoken;
        }
        private static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public string PostOrderData(string URL, object data)
        {
            string res = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    //client.MaxResponseContentBufferSize = 256000;
                    var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented,
                      new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(URL, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        res = result.ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }
        private CXML GetandReadXml(string path)
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string xmlAsda = r.ReadToEnd();
                    asdaOrderXml = xmlAsda;
                }
                CXML test = null;
                XmlSerializer serializer = new XmlSerializer(typeof(CXML));
                test = (CXML)serializer.Deserialize(new XmlTextReader(path));
                return test;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public string GetData(string URL)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                string text = "";
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    text = reader.ReadToEnd();
                }
                SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp", text + "   " + errorcontainingfiles.ToString());               
                SendMail("ramandeep.matrid33789@gmail.com", "Error in ConnectAsdaOrdertoCaApp", text + "   " + errorcontainingfiles.ToString());               
                           
                return text;
            }

        }

        public List<string> GetFileNameandDownloadFromSFTP()
        {
            List<string> fileneedtoprocessed = new List<string>();
            #region SFTP Part
            try
            {
                //string host = "ftp-int.ecommera.com";
                //string username = "asda_071271_prod";
                //string password = "v7yD3Dxs6t9yttk4s8Crve9";

                string host = "mft.asda.uk";
                int port = 22;
                string username = "user_dsv_71271";
                string privateKeyFilePath = @"C:\Users\weblegs\Documents\perfkeyfile"; // Path to your PuTTYgen private key file

                var privateKeyFile = new PrivateKeyFile(privateKeyFilePath);
                var privateKeyFileProvider = new PrivateKeyFile[] { privateKeyFile };

                var connectionInfo = new ConnectionInfo(host, port, username, new PrivateKeyAuthenticationMethod(username, privateKeyFileProvider));


                string remoteDirectory = "/PO5/inbox/";

                string localDirectory = Application.StartupPath + "/DownloadedFile/";

                using (SftpClient sftpClient = new SftpClient(connectionInfo))
                {
                    bool connected = true;
                    try
                    {
                        sftpClient.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(600000);

                        sftpClient.Connect();

                        StreamWriter writer1 = new StreamWriter(Application.StartupPath + "/Connected.txt", true);
                        writer1.WriteLine("Connected " + "------" + DateTime.Now + "  " + sftpClient.KeepAliveInterval.ToString());
                        writer1.Close();
                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        connected = false;
                        StreamWriter writer2 = new StreamWriter(Application.StartupPath + "/ASDAlog.txt", true);
                        writer2.WriteLine(ex.Message + "    retry " + retryConnect.ToString() + "------" + DateTime.Now);
                        writer2.Close();
                        if (retryConnect <= 10)
                        {
                            retryConnect = retryConnect + 1;
                            GetFileNameandDownloadFromSFTP();
                        }
                    }
                    if (connected)
                    {

                        var files = sftpClient.ListDirectory(remoteDirectory);

                        StreamWriter writer1 = new StreamWriter(Application.StartupPath + "/Connected.txt", true);
                        writer1.WriteLine("Connected 2" + "------" + files.Count() + "--------" + DateTime.Now);
                        writer1.Close();
                        connected = true;
                        StreamWriter writer = new StreamWriter(Application.StartupPath + "/ASDAseverlog.txt", true);
                        foreach (var file in files)
                        {
                            string remoteFileName = file.Name;

                            if ((!file.Name.StartsWith(".")) && file.Name != "..")
                            {
                                if (file.LastWriteTime < DateTime.Now.AddDays(-15))
                                {
                                    StreamWriter writer3 = new StreamWriter(Application.StartupPath + "/Deleted.txt", true);
                                    writer3.WriteLine("Connected 2" + "------" + file.FullName + "--------" + DateTime.Now);
                                    writer3.Close();
                                    sftpClient.DeleteFile(file.FullName);

                                }
                                else
                                {
                                    errorfile = remoteFileName;
                                    if ((from order in processedOrders where order.localfileName == remoteFileName select order).Count() > 0)
                                        continue;

                                    writer.WriteLine(remoteFileName + "     " + file.LastWriteTime.Date.ToString() + "      " + DateTime.Now);

                                    using (Stream stream = File.OpenWrite(localDirectory + remoteFileName))
                                    {
                                        try
                                        {

                                            sftpClient.DownloadFile(remoteDirectory + remoteFileName, stream);
                                            fileneedtoprocessed.Add(file.Name);

                                        }
                                        catch (SshConnectionException e)
                                        {
                                            //var sftp1 = ConnectSftp(host, 7104, username, password);
                                            //sftp1.DownloadFile(remoteDirectory + remoteFileName, stream);
                                            //fileneedtoprocessed.Add(file.Name);
                                        }

                                        //sftpClient.DownloadFile(remoteDirectory + remoteFileName, stream);
                                        //fileneedtoprocessed.Add(file.Name);
                                    }
                                }
                            }
                        }
                        sftpClient.Dispose();
                        writer.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                StreamWriter writer = new StreamWriter(Application.StartupPath + "/ASDAlog.txt", true);
                writer.WriteLine("here's the error: " + exp.Message + "------" + DateTime.Now + "------------" + errorfile);
                writer.Close();
            }
            #endregion
            return fileneedtoprocessed;
        }

        public static SftpClient ConnectSftp(string host, int port, string username, string password)
        {
            var connectionInfo = new PasswordConnectionInfo(host, port, username, password);

            SftpClient sftp = new SftpClient(connectionInfo);

            try
            {
                sftp.Connect();
                return sftp;
            }
            catch (SshConnectionException e)
            {
                return null;
            }
        }

        public static Accesstoken PostForAccesstoken(string URL, string refreshtoken, string encodeauthorize)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            string URI = "https://api.channeladvisor.com/" + URL;
            var client = new RestClient(URI);
            var request = new RestSharp.RestRequest();
            request.Method = RestSharp.Method.POST;
            request.Parameters.Clear();
            request.AddHeader("Authorization", encodeauthorize);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshtoken);
            JsonDeserializer deserial = new JsonDeserializer();
            Accesstoken x = deserial.Deserialize<Accesstoken>(client.Execute<Accesstoken>(request));
            return x;
        }
        public List<ProcessedAsdaFile> ReadFiles(string FolderName)
        {
            List<ProcessedAsdaFile> processedOrders = new List<ProcessedAsdaFile>();
            try
            {
                DirectoryInfo info = new DirectoryInfo(Application.StartupPath + "/" + FolderName);
                FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files)
                {

                    using (StreamReader reader = new StreamReader(file.FullName))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                processedOrders.Add(new ProcessedAsdaFile { localfileName = line.Split(',')[0] });
                            }
                            catch { }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                StreamWriter writer = new StreamWriter(Application.StartupPath + "/ASDAlog.txt", true);
                writer.WriteLine("In ReadFiles() method - " + exp.Message + DateTime.Now);
                writer.Close();
                SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp [ReadFiles]", exp.Message);
                SendMail("ramandeep.matrid33789@gmail.com", "Error in ConnectAsdaOrdertoCaApp [ReadFiles]", exp.Message);
               
            
            }
            return processedOrders;
        }
        public bool InsertIntoDatabase(DataTable orderTable)
        {
            bool isExecuted = false;
            int? retryattempt = 0;
            retry:
            try
            {
                if (orderTable.Rows.Count > 0)
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateAsdaOrderDetails", con))
                    {
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Table", orderTable);
                        cmd.CommandTimeout = 300;  //5 minutes
                        cmd.ExecuteNonQuery();
                        isExecuted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retryattempt += 1;
                //make 3 retry attempts only
                if (retryattempt <= 3)
                {
                    SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp [InsertIntoDatabase]", "Retry " + ex.Message + "      " + "Error in possible file" + errorcontainingfiles);
                    SendMail("ramandeep.matrid33789@gmail.com", "Error in ConnectAsdaOrdertoCaApp [InsertIntoDatabase]", "Retry " + ex.Message + "      " + "Error in possible file" + errorcontainingfiles);
                    System.Threading.Thread.Sleep(10000); // 10-second delay
                    goto retry; // Retry operation
                }


                //if still it doesn't work, collect all the xml of orders and store it in the skipped orders directory
                //on next run, fetch skipped channel order ids
                //xml name , channel order id


                StreamWriter writer = new StreamWriter(Application.StartupPath + "/ASDAlog.txt", true);
                writer.WriteLine("In InsertIntoDatabase() - " + ex.Message + "    ---" + DateTime.Now);
                writer.Close();
                SendMail("kamal.matrid77991@gmail.com", "Error in ConnectAsdaOrdertoCaApp [InsertIntoDatabase]", ex.Message + "      " + "Error in possible file" + errorcontainingfiles);               
                SendMail("ramandeep.matrid33789@gmail.com", "Error in ConnectAsdaOrdertoCaApp [InsertIntoDatabase]", ex.Message + "      " + "Error in possible file" + errorcontainingfiles);               
               
            }
            return isExecuted;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
            Application.ExitThread();
            Application.Exit();
        }
    }
    public class Accesstoken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class ProcessedAsdaFile
    {
        public string localfileName { get; set; }
    }
}
