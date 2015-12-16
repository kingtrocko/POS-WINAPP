using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace Inventory_Sales
{
    class InventoryAPI
    {
        private RestClient client;

        public InventoryAPI()
        {
            client = new RestClient();
            client.BaseUrl = new Uri("https://inventory-pos-api.herokuapp.com");
        }

        public DataTable GetAllProducts(string local_id)
        {
            var request = new RestRequest("api/products", Method.GET);
            request.AddParameter("local_id", local_id, ParameterType.QueryString);

            IRestResponse response = client.Execute(request);
            var content = response.Content;
            
            JObject obj = JObject.Parse(content);

            if ( string.IsNullOrEmpty(Convert.ToString(obj["error_description"])) )
            {
                DataSet ds = JsonConvert.DeserializeObject<DataSet>(content);
                return ds.Tables["products"];
            }
            else
                return new DataTable("products");
        }

        public DataSet GetProductStock(string product_id, string client_id, string local_id)
        {
            var request = new RestRequest("/api/productstock/{product_id}/", Method.GET);
            request.AddParameter("product_id", product_id, ParameterType.UrlSegment);
            request.AddParameter("client_id", client_id, ParameterType.QueryString);
            request.AddParameter("local_id", local_id, ParameterType.QueryString);

            IRestResponse res = client.Execute(request);
            var content = res.Content;

             JObject obj = JObject.Parse(content);

             if (string.IsNullOrEmpty(Convert.ToString(obj["error_description"])))
             {
                 DataSet ds = JsonConvert.DeserializeObject<DataSet>(content);
                 return ds;
             }
             else
                 return new DataSet();
        }
       
        public DataTable GetPricesPerProduct(string product_id, string price_id)
        {
            var req = new RestRequest("/api/products/{product_id}/prices/{price_id}", Method.GET);
            req.AddParameter("product_id", product_id, ParameterType.UrlSegment);
            req.AddParameter("price_id", price_id, ParameterType.UrlSegment);

            IRestResponse res = client.Execute(req);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);
                return ds.Tables["prices"];
            }
            else
                return new DataTable("prices");
        }

        public DataTable GetPricesTypes()
        {
            var req = new RestRequest("/api/prices/types", Method.GET);

            IRestResponse res = client.Execute(req);
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);

            return ds.Tables["prices_types"];
        }

        public DataTable GetClients()
        {
            var req = new RestRequest("/api/clients", Method.GET);

            IRestResponse res = client.Execute(req);
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);
            return ds.Tables["clients"];
        }

        public DataTable GetDocumentSale(string document_type_name)
        {
            List<Parameter> plist = new List<Parameter>();
            Parameter p = new Parameter();
            p.Name = "document_type";
            p.Value = document_type_name;
            p.Type = ParameterType.QueryString;
            plist.Add(p);


            IRestResponse res = MakeHTTPRequest("/api/document-sale", Method.GET, plist);
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);
            return ds.Tables["document_sale"];
        }

        public DataTable GetPaymentConditions()
        {
            IRestResponse res = MakeHTTPRequest("/api/payment-conditions", Method.GET, null);
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);
            return ds.Tables["payment_conditions"];
        }

        public DataTable GetSales(string local_id, int sale_status)
        {
            List<Parameter> plist = new List<Parameter>();
            Parameter p = new Parameter();
            p.Name = "local_id";
            p.Value = local_id;
            p.Type = ParameterType.QueryString;

            Parameter p2 = new Parameter();
            p.Name = "status";
            p.Value = sale_status;
            p.Type = ParameterType.QueryString;

            plist.Add(p); plist.Add(p2);

            IRestResponse res = MakeHTTPRequest("/api/sales", Method.GET, plist);
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(res.Content);
            return ds.Tables["sales"];
        }

        private IRestResponse MakeHTTPRequest(string uri, Method method, List<Parameter> parameters)
        {
            var request = new RestRequest(uri, method);
            if (parameters != null)
                request.Parameters.AddRange(parameters);

            return client.Execute(request);
        }
    }
}
