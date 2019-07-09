using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using RestSharp.Serializers;
using RestSharp.Serialization.Json;
using RestSharp.Deserializers;
using Newtonsoft.Json.Linq;
using System.Net;
using Foodicsapi.Classess;
using System.Data.SqlClient;
namespace Foodicsapi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static readonly HttpClient client = new HttpClient();
        public string access_token;
        string sAccTocken_Foodics =""; //for tawa
        string Business_id = ""; //for tawa
        string Branch_id = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            Acctkn.SelectedIndex = 0;
            var values = new Dictionary<string, string>
            {

               { "secret", "39K7I6GJ7ID137CUI2UF" }

            };
            //sAccTocken_Foodics = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcHAiLCJhcHAiOjIyNCwiYnVzIjpudWxsLCJjb21wIjpudWxsLCJzY3J0IjoiNllKS1lOIn0.dOPgF_BwVhSNAl3R9FJDnQs8gXTRFVm9peWZEJ76-XM";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync("https://dash.foodics.com/api/v2/token", content);


            string json = response.Result.Content.ReadAsStringAsync().Result;
            dynamic obj = JObject.Parse(json);
            access_token = obj.access_token;

        }
        public string get_acctkn() {

            if (Acctkn.SelectedIndex == 0) {
                sAccTocken_Foodics = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcHAiLCJhcHAiOjIyNCwiYnVzIjpudWxsLCJjb21wIjpudWxsLCJzY3J0IjoiNllKS1lOIn0.dOPgF_BwVhSNAl3R9FJDnQs8gXTRFVm9peWZEJ76-XM";
            }
            else if (Acctkn.SelectedIndex == 1)
            {
                sAccTocken_Foodics = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcHAiLCJhcHAiOjIyMywiYnVzIjpudWxsLCJjb21wIjpudWxsLCJzY3J0IjoiUDhIUkxHIn0.-UxHkRqMzN2alIZcFP7dZDxtjYIjybETMmOhXD74OSU";
            }
            return sAccTocken_Foodics;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
        }
        public string Get_Business(string sAccTocken_Foodics)
        {
            
            try
            {
                var url = "https://dash.foodics.com/api/v2/businesses";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                RootObject result = JsonConvert.DeserializeObject<RootObject>(content);
                var sMysql = "";
                // business
                foreach (var bs in result.businesses)
                {
                    sMysql = string.Format("if exists (SELECT hid FROM Business WHERE hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            sMysql = string.Format("Insert into Business(name,hid)values('{0}', '{1}' )", bs.name, bs.hid);
                            ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                    Business_id = bs.hid;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Business_id;
        }
        private string Get_Branch(string sAccTocken_Foodics,string Business_id)
        {
            try
            {
                var url = "https://dash.foodics.com/api/v2/branches?filters[type]=1";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootBranches result = JsonConvert.DeserializeObject<RootBranches>(content);
                var sMysql = "";
                //Branches
                foreach (var bs in result.branches)
                {
                    sMysql = string.Format("if exists (SELECT hid FROM Branches WHERE hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            sMysql = string.Format("Insert into Branches(name,hid)values('{0}', '{1}' )", bs.name.en, bs.hid);
                            ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                    Branch_id = bs.hid;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Branch_id;
        }
        private void Get_Products(string sAccTocken_Foodics, string Business_id)
        {
            try
            {
                var url = "https://dash.foodics.com/api/v2/products";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootProducts result = JsonConvert.DeserializeObject<RootProducts>(content);
                var sMysql = "";
                //Branches
                foreach (var bs in result.products)
                {
                    sMysql = string.Format("if exists (SELECT hid FROM Products WHERE hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            sMysql = string.Format("Insert into Products(name,hid)values('{0}', '{1}' )", bs.name.en, bs.hid);
                            ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void Get_PaymentMethods(string sAccTocken_Foodics, string Business_id)
        {
            try
            {
                var url = "https://dash.foodics.com//api/v2/payment-methods";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootPaymentMethods result = JsonConvert.DeserializeObject<RootPaymentMethods>(content);
                var sMysql = "";
                //Branches
                foreach (var bs in result.payment_methods)
                {
                    sMysql = string.Format("if exists (SELECT hid FROM PaymentMethods WHERE hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            sMysql = string.Format("Insert into PaymentMethods(name,hid)values(N'{0}', '{1}' )", bs.name, bs.hid);
                            ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Get_Orders(string sAccTocken_Foodics, string Business_id,string Branch_id)
        {
            try
            {
                string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                var url = "https://dash.foodics.com/api/v2/orders?filters[business_date]="+date+"&filters[status]=4&filters[branch_hid]=" + Branch_id;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
              
                JObject obj = JObject.Parse(content);
                RootOrders result = JsonConvert.DeserializeObject<RootOrders>(content);
                var sMysql = "";
                string waiterid = "";
                string cashier = "";
                string device = "";
                string customer = "";
                string tableid = "";
                string discount = "";
                foreach (var bs in result.orders)
                {
                    sMysql = string.Format("if exists (SELECT Order_hid FROM Orders WHERE Order_hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {

                            if (bs.waiter == null && bs.table==null || bs.discount==null) {

                                sMysql = string.Format("insert into Orders(Order_RefNo, Order_hid, Order_sequence, Order_type, Order_source, Status, Persons, Order_price, Order_Final_price, Delivery_price, Discount_amount, Delay_in_sec, Total_tax, Business_date, Branch_hid, Cashier_hid, Waiter_hid, Device_hid, Customer, Table_hid, Discount, Tax_amount)" +
                                          "values('{0}','{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}')",
                                          bs.reference, bs.hid, bs.sequence, bs.type, bs.source, bs.status, bs.persons, bs.price, bs.final_price, bs.delivery_price, bs.discount_amount, bs.delay_in_seconds, bs.total_tax, bs.business_date, bs.branch.hid, bs.cashier.hid, waiterid, bs.device.hid, bs.customer, tableid, discount, bs.total_tax);
                                ClsSql.ExcuteCommand(sMysql);
                            }

                            else {
                                sMysql = string.Format("insert into Orders(Order_RefNo, Order_hid, Order_sequence, Order_type, Order_source, Status, Persons, Order_price, Order_Final_price, Delivery_price, Discount_amount, Delay_in_sec, Total_tax, Business_date, Branch_hid, Cashier_hid, Waiter_hid, Device_hid, Customer, Table_hid, Discount, Tax_amount)" +
                                           "values('{0}','{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}')",
                                           bs.reference, bs.hid, bs.sequence, bs.type, bs.source, bs.status, bs.persons, bs.price, bs.final_price, bs.delivery_price, bs.discount_amount, bs.delay_in_seconds, bs.total_tax, bs.business_date, bs.branch.hid, bs.cashier.hid, bs.waiter.hid, bs.device.hid, bs.customer, bs.table.hid, bs.discount, bs.total_tax);
                                ClsSql.ExcuteCommand(sMysql);
                            }
                            foreach (var pr in bs.products)
                            {
                               sMysql= string.Format("insert into OrderItems(Order_refNo,Order_hid, Status, Quantity, Returned_Quantity, Product_hid, Product_size_hid, Original_Price, Final_Price, Delay_in_seconds, Cost, Discount_amount,Displayable_Original_Price, Displayable_Final_Price, Business_date, Taxable)" +
                                      "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}','{14}','{15}')", bs.reference,bs.hid, pr.status, pr.quantity, pr.returned_quantity, pr.product_hid, pr.product_size_hid, pr.original_price, pr.final_price, pr.delay_in_seconds, pr.cost, pr.discount_amount, pr.displayable_original_price, pr.displayable_final_price, pr.business_date, pr.taxable);
                                ClsSql.ExcuteCommand(sMysql);   
                            }
                            foreach (var pay in bs.payments)
                            {
                                sMysql = string.Format("insert into Order_PaymentMethods(Order_refNo,Order_hid, hid, payment_method_hid, Amount, Tendered)" +
                                  "values('{0}', '{1}', '{2}', '{3}', '{4}','{5}')", bs.reference, bs.hid, pay.hid, pay.payment_method.hid, pay.amount, pay.tendered);

                                ClsSql.ExcuteCommand(sMysql);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void Get_Inventory(string sAccTocken_Foodics, string Business_id, string Branch_id)
        {
            try
            {
                string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                var url = "https://dash.foodics.com/api/v2/inventory-transactions?filters[business_date]="+date+"&filters[status]=1&filters[branch_hid]=" + Branch_id;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootInventory result = JsonConvert.DeserializeObject<RootInventory>(content);
                var sMysql = "";

                foreach (var bs in result.transactions)
                {
                    sMysql = string.Format("if exists (SELECT hid FROM InventoryTransaction WHERE hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            foreach (var it in bs.items)
                            {

                                sMysql = string.Format("insert into InventoryTransaction(hid,Invoice_number,Status,Invoice_date,Business_date,Quantity,Cost,Items_hid,Expiration_date,Inventory_item_hid,Branch_hid,Other_branch_hid,Supplier_hid,Orders_hid,User_hid)"+
                                 "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}','{13}','{14}')",bs.hid,bs.invoice_number,bs.type,bs.invoice_date,bs.business_date,it.quantity,it.cost,it.hid,it.expiration_date,it.inventory_item.hid,bs.branch.hid,bs.other_branch,bs.supplier,bs.order.hid,bs.user);
                                ClsSql.ExcuteCommand(sMysql);
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Get_Customers(string sAccTocken_Foodics, string Business_id, string Branch_id)
        {
            try
            {

                var url = "https://dash.foodics.com//api/v2/customers?filters[branch_hid]=" + Branch_id;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootCustomers result = JsonConvert.DeserializeObject<RootCustomers>(content);
                var sMysql = "";

                foreach (var bs in result.customers)
                {
                    sMysql = string.Format("if exists (SELECT customer_hid FROM Customers WHERE customer_hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {


                            sMysql = string.Format("insert into Customers(customer_name, customer_hid, phone, Email, Blacklist, country_hid, country_code)values(N'{0}','{1}','{2}','{3}','{4}','{5}','{6}')", bs.name,bs.hid,bs.phone,bs.email,bs.blacklist,bs.country.hid,bs.country.code);
      
                            ClsSql.ExcuteCommand(sMysql);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Get_Devices(string sAccTocken_Foodics, string Business_id, string Branch_id)
        {
            try
            {

                var url = "https://dash.foodics.com/api/v2/devices?filters[branch_hid]=" + Branch_id;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootDevices result = JsonConvert.DeserializeObject<RootDevices>(content);
                var sMysql = "";

                foreach (var bs in result.devices)
                {
                    sMysql = string.Format("if exists (SELECT Device_hid FROM Devices WHERE Device_hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                                sMysql = string.Format("insert into Devices(Device_hid,Device_name,Branch_hid)values('{0}', '{1}', '{2}')", bs.hid, bs.name,bs.branch.hid);
                                ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Get_Countries(string sAccTocken_Foodics, string Business_id)
        {
            try
            {

                var url = "https://dash.foodics.com/api/v2/countries";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                RootCountries result = JsonConvert.DeserializeObject<RootCountries>(content);
                var sMysql = "";

                foreach (var bs in result.countries)
                {
                    sMysql = string.Format("if exists (SELECT Country_hid FROM Countries WHERE Country_hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                            sMysql = string.Format("insert into Countries(Country_name,Country_hid,Country_code,currency)values(N'{0}', '{1}', '{2}','{3}')", bs.name.en,bs.hid,bs.code,bs.currency);
                            ClsSql.ExcuteCommand(sMysql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Get_Users(string sAccTocken_Foodics, string Business_id)
        {
            try
            {

                var url = "https://dash.foodics.com/api/v2/users";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UnsafeAuthenticatedConnectionSharing = true;
                request.Headers["Authorization"] = "Bearer " + sAccTocken_Foodics;
                request.Headers["X-business"] = Business_id;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();

                JObject obj = JObject.Parse(content);
                Rootusers result = JsonConvert.DeserializeObject<Rootusers>(content);
                var sMysql = "";
                string company_role = "";
                string business_role = "";
                string branch_role = "";
                string branch_app_role = "";
                string business_access = "";
                string branches_access = "";
                string branches_app_access = "";
                foreach (var bs in result.users)
                {
                    sMysql = string.Format("if exists (SELECT user_hid FROM Users WHERE user_hid = '{0}') select '1' else select '0' ", bs.hid);
                    var sTmp = ClsSql.GetQryVal(sMysql);
                    if (Convert.ToInt16(sTmp) == 0)
                    {
                        if (bs.hid != null)
                        {
                           if(bs.type==3){
                                sMysql = string.Format("insert into Users(name, user_name, user_hid, user_type, dial_code, mobile, email, employee_number, language, mail_confirmed, company_role, business_role, branch_role, branch_app_role_hid, business_access, branches_access, branches_app_access_hid)" +
                                 "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                 bs.name, bs.username, bs.hid, bs.type, bs.dial_code, bs.mobile, bs.email, bs.employee_number, bs.lang, bs.mail_confirmed, company_role, business_role, branch_role,branch_app_role, business_access,branches_access, branches_app_access);
                                ClsSql.ExcuteCommand(sMysql);
                            }
                            if (bs.type == 4)
                            {
                                sMysql = string.Format("insert into Users(name, user_name, user_hid, user_type, dial_code, mobile, email, employee_number, language, mail_confirmed, company_role, business_role, branch_role, branch_app_role_hid, business_access, branches_access, branches_app_access_hid)" +
                                 "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')", 
                                 bs.name, bs.username, bs.hid, bs.type, bs.dial_code, bs.mobile, bs.email, bs.employee_number, bs.lang, bs.mail_confirmed, bs.company_role.hid, bs.business_role.hid, bs.branch_role.hid,branch_app_role, bs.businesses_access, bs.branches_access, bs.branches_app_access);
                                ClsSql.ExcuteCommand(sMysql);
                            }
                            if (bs.type == 5)
                            {
                                sMysql = string.Format("insert into Users(name, user_name, user_hid, user_type, dial_code, mobile, email, employee_number, language, mail_confirmed, company_role, business_role, branch_role, branch_app_role_hid, business_access, branches_access, branches_app_access_hid)" +
                                 "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')", bs.name, bs.username, bs.hid, bs.type, bs.dial_code, bs.mobile, bs.email, bs.employee_number, bs.lang, bs.mail_confirmed, company_role, business_role, branch_role, bs.branch_app_role.hid, business_access, branches_access, bs.branches_access, bs.branches_app_access);
                                ClsSql.ExcuteCommand(sMysql);
                            }

                            else {
                                sMysql = string.Format("insert into Users(name, user_name, user_hid, user_type, dial-code, mobile, email, employee_number, language, mail_confirmed, company_role, business_role, branch_role, branch_app_role_hid, business_access, branches_access, branches_app_access_hid)" +
                                 "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                 bs.name, bs.username, bs.hid, bs.type, bs.dial_code, bs.mobile, bs.email, bs.employee_number, bs.lang, bs.mail_confirmed, bs.company_role.hid, bs.business_role.hid, bs.branch_role.hid, bs.businesses_access, bs.branches_access, bs.branches_app_access);
                                ClsSql.ExcuteCommand(sMysql);
                            }

                         
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public class AppsConfigs
        {
            public List<object> waiter { get; set; }
        }

        public class ReceiptConfigs
        {
            public string header { get; set; }
            public string footer { get; set; }
        }

        public class Names
        {
            public string en { get; set; }
            //public string ar { get; set; }
        }

        public class Country
        {
            public Names name { get; set; }
            public string code { get; set; }
            public int decimal_places { get; set; }
            public string currency { get; set; }
            public string dial_code { get; set; }
            public object created_at { get; set; }
            public object updated_at { get; set; }
            public string hid { get; set; }
        }

        public class Business
        {
            public string name { get; set; }
            public string reference { get; set; }
            public string slug { get; set; }
            public int delivery_price { get; set; }
            public bool prices_include_taxes { get; set; }
            public bool advanced_reports_enabled { get; set; }
            public int max_call_center_agents { get; set; }
            public string image_path { get; set; }
            public List<string> supported_locales { get; set; }
            public AppsConfigs apps_configs { get; set; }
            public ReceiptConfigs receipt_configs { get; set; }
            public string waiter_welcome { get; set; }
            public string display_welcome { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public string splash_path { get; set; }
            public Country country { get; set; }
        }

        public class RootObject
        {
            public List<Business> businesses { get; set; }
        }
        public class BusinessName
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class businessReceiptConfigs
        {
            public string header { get; set; }
            public string footer { get; set; }
        }

        public class City
        {
            public string timezone { get; set; }
            public string hid { get; set; }
        }

        public class Tax
        {
            public string hid { get; set; }
        }

        public class Branch
        {
            public string reference { get; set; }
            public BusinessName name { get; set; }
            public string phone { get; set; }
            public businessReceiptConfigs receipt_configs { get; set; }
            public int type { get; set; }
            public object latitude { get; set; }
            public object longitude { get; set; }
            public string open_from { get; set; }
            public string open_till { get; set; }
            public bool accepts_online_orders { get; set; }
            public int pickup_promising_time { get; set; }
            public int delivery_promising_time { get; set; }
            public int service_fees { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public List<object> disabled_order_types { get; set; }
            public City city { get; set; }
            public List<object> delivery_zones { get; set; }
            public List<Tax> taxes { get; set; }
            public List<object> tags { get; set; }
        }

        public class RootBranches
        {
            public List<Branch> branches { get; set; }
        }
        public class ProductName
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class Description
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class Category
        {
            public string hid { get; set; }
        }

        public class Name2
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class Size
        {
            public Name2 name { get; set; }
            public int price { get; set; }
            public int index { get; set; }
            public string barcode { get; set; }
            public string sku { get; set; }
            public int cost { get; set; }
            public bool has_fixed_cost { get; set; }
            public int calories { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public string costingType { get; set; }
            public List<object> ingredients { get; set; }
            public List<object> special_branch_prices { get; set; }
        }

        public class Product
        {
            public ProductName name { get; set; }
            public Description description { get; set; }
            public object preparation_time_in_minutes { get; set; }
            public object calories { get; set; }
            public int index { get; set; }
            public int pricing_type { get; set; }
            public int selling_type { get; set; }
            public bool is_active { get; set; }
            public object barcode { get; set; }
            public object sku { get; set; }
            public bool taxable { get; set; }
            public bool is_combo { get; set; }
            public string image_path { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public Category category { get; set; }
            public List<object> tags { get; set; }
            public List<object> modifiers { get; set; }
            public List<Size> sizes { get; set; }
            public List<object> timed_events { get; set; }
            public List<object> combo_items { get; set; }
        }

        public class RootProducts
        {
            public List<Product> products { get; set; }
        }
        public class PaymentMethod
        {
            public object code { get; set; }
            public string name { get; set; }
            public int type { get; set; }
            public bool auto_open_cash_drawer { get; set; }
            public bool is_active { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
        }

        public class RootPaymentMethods
        {
            public List<PaymentMethod> payment_methods { get; set; }
        }
        public class OrderedProduct
        {
            public string guid { get; set; }
            public int status { get; set; }
            public bool should_return_ingredients { get; set; }
            public int quantity { get; set; }
            public int returned_quantity { get; set; }
            public string notes { get; set; }
            public double original_price { get; set; }
            public double final_price { get; set; }
            public string kitchen_received_at { get; set; }
            public string kitchen_done_at { get; set; }
            public string actual_date { get; set; }
            public int delay_in_seconds { get; set; }
            public int cost { get; set; }
            public List<object> kitchen_times { get; set; }
            public int discount_amount { get; set; }
            public int displayable_original_price { get; set; }
            public int displayable_final_price { get; set; }
            public bool taxable { get; set; }
            public bool is_combo { get; set; }
            public string hid { get; set; }
            public string void_reason { get; set; }
            public object business_date { get; set; }
            public string product_hid { get; set; }
            public string product_size_hid { get; set; }
            public List<object> removed_ingredients { get; set; }
            public List<object> options { get; set; }
            public object discount { get; set; }
            public object combo { get; set; }
            public object combo_option_size { get; set; }
        }

        public class OrderBranch
        {
            public string hid { get; set; }
            public List<object> disabled_order_types { get; set; }
        }

        public class Cashier
        {
            public string hid { get; set; }
            public string pin { get; set; }
        }

        public class Waiter
        {
            public string hid { get; set; }
            public string pin { get; set; }
        }

        public class Device
        {
            public string hid { get; set; }
        }

        public class Table
        {
            public string hid { get; set; }
        }

        public class OrderPaymentMethod
        {
            public string hid { get; set; }
         
           
        }
       
        public class Employee
        {
            public string hid { get; set; }
            public string pin { get; set; }
        }

        public class OrderPayment
        {
            public string guid { get; set; }
            public int amount { get; set; }
            public int tendered { get; set; }
            public string actual_date { get; set; }
            public string details { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public string business_date { get; set; }
            public OrderPaymentMethod payment_method { get; set; }
            public Employee employee { get; set; }
        }

        public class RelationshipData
        {
            public double amount { get; set; }
            public int percentage { get; set; }
        }

        public class OrderTax
        {
            public string hid { get; set; }
            public RelationshipData relationship_data { get; set; }
        }

        public class Order
        {
            public string guid { get; set; }
            public string reference { get; set; }
            public int sequence { get; set; }
            public int status { get; set; }
            public int type { get; set; }
            public int source { get; set; }
            public int persons { get; set; }
            public string notes { get; set; }
            public double price { get; set; }
            public int delivery_price { get; set; }
            public int discount_amount { get; set; }
            public int final_price { get; set; }
            public string kitchen_received_at { get; set; }
            public string kitchen_done_at { get; set; }
            public int delay_in_seconds { get; set; }
            public object due_time { get; set; }
            public string opened_at { get; set; }
            public string closed_at { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string address { get; set; }
            public int number { get; set; }
            public int rounding { get; set; }
            public object driver_collected_at { get; set; }
            public object delivered_at { get; set; }
            public List<object> kitchen_times { get; set; }
            public int tips { get; set; }
            public int service_fees { get; set; }
            public double total_tax { get; set; }
            public object dispatched_at { get; set; }
            public string hid { get; set; }
            public string void_reason { get; set; }
            public string business_date { get; set; }
            public List<OrderedProduct> products { get; set; }
            public OrderBranch branch { get; set; }
            public object application { get; set; }
            public Cashier cashier { get; set; }
            public object driver { get; set; }
            public Waiter waiter { get; set; }
            public object online_orders_agent { get; set; }
            public Device device { get; set; }
            public object customer { get; set; }
            public Table table { get; set; }
            public object discount { get; set; }
            public List<OrderPayment> payments { get; set; }
            public object delivery_address { get; set; }
            public List<OrderTax> taxes { get; set; }
            public List<object> tags { get; set; }
        }

        public class RootOrders
        {
            public List<Order> orders { get; set; }
        }
        public class InventoryItem
        {
            public string hid { get; set; }
        }

        public class Item
        {
            public double quantity { get; set; }
            public int cost { get; set; }
            public object stocktaking_original_quantity { get; set; }
            public object stocktaking_entered_quantity { get; set; }
            public string hid { get; set; }
            public object expiration_date { get; set; }
            public InventoryItem inventory_item { get; set; }
        }

        public class InventoryBranch
        {
            public string hid { get; set; }
            public List<object> disabled_order_types { get; set; }
        }

        public class InventoryOrder
        {
            public string hid { get; set; }
            public string void_reason { get; set; }
            public object business_date { get; set; }
        }

        public class Transaction
        {
            public string reference { get; set; }
            public int direction { get; set; }
            public int type { get; set; }
            public object invoice_number { get; set; }
            public object invoice_date { get; set; }
            public string notes { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public bool is_pending { get; set; }
            public bool is_declined { get; set; }
            public string business_date { get; set; }
            public int paid_tax { get; set; }
            public string hid { get; set; }
            public List<Item> items { get; set; }
            public InventoryBranch branch { get; set; }
            public object other_branch { get; set; }
            public object other_transaction { get; set; }
            public InventoryOrder order { get; set; }
            public object user { get; set; }
            public object supplier { get; set; }
        }

        public class RootInventory
        {
            public List<Transaction> transactions { get; set; }
        }
        public class CustomerCountry
        {
            public string code { get; set; }
            public string hid { get; set; }
        }

        public class Customer
        {
            public string name { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string notes { get; set; }
            public bool blacklist { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public CustomerCountry country { get; set; }
        }

        public class RootCustomers
        {
            public List<Customer> customers { get; set; }
        }
        public class DeviceBranch
        {
            public string hid { get; set; }
            public List<object> disabled_order_types { get; set; }
        }

        public class Devices
        {
            public int? model { get; set; }
            public int license_type { get; set; }
            public int is_blocked { get; set; }
            public bool is_code_in_use { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string system_version { get; set; }
            public string app_version { get; set; }
            public string reference { get; set; }
            public string name { get; set; }
            public bool can_receive_online_orders { get; set; }
            public string last_data_download { get; set; }
            public string last_orders_upload { get; set; }
            public object last_shifts_upload { get; set; }
            public object last_tills_upload { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public DeviceBranch branch { get; set; }
        }

        public class RootDevices
        {
            public List<Devices> devices { get; set; }
        }

        public class CountryName
        {
            public string en { get; set; }
            public string ar { get; set; }
        }

        public class Countrylist
        {
            public CountryName name { get; set; }
            public string code { get; set; }
            public int decimal_places { get; set; }
            public string currency { get; set; }
            public string dial_code { get; set; }
            public object created_at { get; set; }
            public object updated_at { get; set; }
            public string hid { get; set; }
        }

        public class RootCountries
        {
            public List<Countrylist> countries { get; set; }
        }

        public class CompanyRole
        {
            public string hid { get; set; }
        }

        public class BusinessRole
        {
            public string hid { get; set; }
        }

        public class BranchRole
        {
            public string hid { get; set; }
        }

        public class BranchAppRole
        {
            public string hid { get; set; }
        }
        public class branchname {

            public string en { get; set; }
            public string ar { get; set; }
        }
        public class userbranches
        {
            public string reference { get; set; }
            public branchname name { get; set; }
            public string hid { get; set; }
        }
        public class business_access
        {
            public string hid { get; set; }

        }

        public class User
        {
            public int type { get; set; }
            public string name { get; set; }
            public string dial_code { get; set; }
            public string mobile { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public string employee_number { get; set; }
            public string lang { get; set; }
            public bool mail_confirmed { get; set; }
            public int timezone { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string hid { get; set; }
            public string pin { get; set; }
            public CompanyRole company_role { get; set; }
            public BusinessRole business_role { get; set; }
            public BranchRole branch_role { get; set; }
            public BranchAppRole branch_app_role { get; set; }
            public List<business_access> businesses_access { get; set; }
            public List<userbranches> branches_access { get; set; }
            public List<object> branches_app_access { get; set; }
        }

        public class Rootusers
        {
            public List<User> users { get; set; }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics,Business_id);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_Products(sAccTocken_Foodics,Business_id);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_Orders(sAccTocken_Foodics, Business_id, Branch_id);
        }

        private void Acctkn_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_Inventory(sAccTocken_Foodics, Business_id, Branch_id);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_PaymentMethods(sAccTocken_Foodics, Business_id);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_Customers(sAccTocken_Foodics, Business_id,Branch_id);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Branch(sAccTocken_Foodics, Business_id);
            Get_Devices(sAccTocken_Foodics, Business_id, Branch_id);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Countries(sAccTocken_Foodics, Business_id);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            get_acctkn();
            Get_Business(sAccTocken_Foodics);
            Get_Users(sAccTocken_Foodics, Business_id);
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }
    }
}
