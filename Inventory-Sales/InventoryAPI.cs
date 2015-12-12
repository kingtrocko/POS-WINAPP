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
            var request = new RestRequest("/api/productstock/{product_id}/");
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
            var req = new RestRequest("/api/products/{product_id}/prices/{price_id}");
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
    }
}
