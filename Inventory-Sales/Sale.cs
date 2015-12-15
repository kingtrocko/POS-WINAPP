using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Inventory_Sales
{
    public class Sale
    {
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public int ClientID { get; set; }
        public int SalesmanID { get; set; }
        public int LocalID { get; set; }
        public decimal MoneyChange { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime SaleDate { get; set; }
        public DataTable Products { get; set; }
        public DataTable DocumentTypeInfo { get; set; }
        public string DocumentType { get; set; }
        public int PaymentConditionID { get; set; }
        public string SaleStatus { get; set; }
        
        private int SaleID { get; set; }
        
        private long DocumentID { get; set; }
        private string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["teayudop_bdinventarioConnectionString"].ConnectionString;
            }
        }

        private MySqlConnection connection;

        public Sale()
        {
            connection = new MySqlConnection(ConnectionString);
        }

       
        public void SaveSale()
        {
            MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                this.DocumentID = InsertDocumentSale(transaction);
                this.SaleID = InserSaleHeader(transaction);
            }
            catch (Exception e)
            {
                transaction.Rollback();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

        }

        private long InsertDocumentSale(MySqlTransaction t)
        {
            string query = "INSERT INTO documento_venta(nombre_tipo_documento, documento_Serie, documento_Numero) VALUES(@name, @serie, @number)";
            MySqlCommand command = new MySqlCommand(query, connection, t);
            command.Parameters.AddWithValue("@name", this.DocumentType);
            command.Parameters.AddWithValue("@serie", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["SERIE"]));
            command.Parameters.AddWithValue("@number", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["NUMERO"]));

            int id = Convert.ToInt32(command.ExecuteScalar());
            return command.LastInsertedId;
        }

        private int InserSaleHeader(MySqlTransaction t)
        {
            string query = @"INSERT INTO venta (fecha,id_cliente,id_vendedor,condicion_pago,venta_status,local_id,subtotal,total_impuesto,total,numero_documento,pagado,vuelto)
                               VALUES(@date,@client_id,@salesman_id,@payment_condition,@status,@local_id,@subtotal,@tax,@total,@doc_number,@payed,@change)";
            MySqlCommand command = new MySqlCommand(query, connection, t);
            command.Parameters.AddWithValue("@date", this.SaleDate);
            command.Parameters.AddWithValue("@client_id", this.ClientID);
            command.Parameters.AddWithValue("@salesman_id", this.SalesmanID);
            command.Parameters.AddWithValue("@payment_condition", this.PaymentConditionID);
            command.Parameters.AddWithValue("@status", this.SaleStatus);
            command.Parameters.AddWithValue("@local_id", this.LocalID);
            command.Parameters.AddWithValue("@subtotal", this.SubTotal);
            command.Parameters.AddWithValue("@tax", this.Tax);
            command.Parameters.AddWithValue("@total", this.GrandTotal);
            command.Parameters.AddWithValue("@doc_number", this.DocumentID);
            command.Parameters.AddWithValue("@payed", this.AmountPaid);
            command.Parameters.AddWithValue("@change", this.MoneyChange);

            int id = Convert.ToInt32(command.ExecuteScalar());
            return id;
        }

        private void InsertSaleDetail(MySqlTransaction t)
        {
            string query = "";
            foreach (DataRow row in this.Products.Rows)
            {
                int product_id = Convert.ToInt32(row["id_producto"]);
                var currentStock = GetCurrentProductStock(t, product_id);
                var currentQuantity = currentStock.Rows[0]["cantidad"];
                var currentFraction = currentStock.Rows[0]["fraccion"];

                int productQuantityInSale = Convert.ToInt32(row["cantidad"]);
                int measureUnitId = Convert.ToInt32(row["unidad_id"]);
                decimal productPrice = Convert.ToDecimal(row["precio"]);
                decimal productTotal = Convert.ToDecimal(row["Total"]);
            }
        }

        private DataTable GetCurrentProductStock(MySqlTransaction t, int product_id) 
        {
            string query = @"SELECT id_inventario, cantidad, fraccion
						        FROM inventario where id_producto=@product_id and id_local=@local_id";
            MySqlCommand command = new MySqlCommand(query, this.connection, t);
            command.Parameters.AddWithValue("@product_id", product_id);
            command.Parameters.AddWithValue("@local_id", this.LocalID);

            var dt = new DataTable();
            MySqlDataReader reader = command.ExecuteReader();
            dt.Load(reader);
            return dt;
        }
    }
}
