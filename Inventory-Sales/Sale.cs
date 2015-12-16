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
        private MySqlTransaction transaction;

        public Sale()
        {
            connection = new MySqlConnection(ConnectionString);
        }

       
        public void SaveSale()
        {
            transaction = connection.BeginTransaction();
            try
            {
                this.DocumentID = InsertDocumentSale();
                this.SaleID = InserSaleHeader();
                InsertSaleDetail();

                this.transaction.Commit();
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

        private long InsertDocumentSale()
        {
            string query = "INSERT INTO documento_venta(nombre_tipo_documento, documento_Serie, documento_Numero) VALUES(@name, @serie, @number)";
            MySqlCommand command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@name", this.DocumentType);
            command.Parameters.AddWithValue("@serie", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["SERIE"]));
            command.Parameters.AddWithValue("@number", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["NUMERO"]));

            int id = Convert.ToInt32(command.ExecuteScalar());
            return command.LastInsertedId;
        }

        private int InserSaleHeader()
        {
            string query = @"INSERT INTO venta (fecha,id_cliente,id_vendedor,condicion_pago,venta_status,local_id,subtotal,total_impuesto,total,numero_documento,pagado,vuelto)
                               VALUES(@date,@client_id,@salesman_id,@payment_condition,@status,@local_id,@subtotal,@tax,@total,@doc_number,@payed,@change)";
            MySqlCommand command = new MySqlCommand(query, connection, transaction);
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

        private void InsertSaleDetail()
        {
            string query = "";
            foreach (DataRow row in this.Products.Rows)
            {
                int product_id = Convert.ToInt32(row["id_producto"]);
                var currentStock = GetCurrentProductStock(product_id);
                int currentQuantityInStock = Convert.ToInt32(currentStock.Rows[0]["cantidad"]);
                int currentFractionInStock = Convert.ToInt32(currentStock.Rows[0]["fraccion"]);

                int productQuantityInSale = Convert.ToInt32(row["cantidad"]);
                int unitIdInSale = Convert.ToInt32(row["id_unidad"]);
                decimal productPrice = Convert.ToDecimal(row["precio"]);
                decimal productTotal = Convert.ToDecimal(row["Total"]);
                decimal productUnitCost = Convert.ToDecimal(row["producto_costo_unitario"]);

                DataTable productUnits = GetProductUnitMeasure(product_id);

                DataRow minUnitRow = productUnits.Rows[0];
                DataRow maxUnitRow = productUnits.Rows[productUnits.Rows.Count - 1];

                DataRow unitsInSaleRow =  productUnits.AsEnumerable().FirstOrDefault(x => x.Field<int>("id_unidad") == unitIdInSale);

                int totalProductUnits = Convert.ToInt32(unitsInSaleRow["unidades"]) * productQuantityInSale;
                int totalProductUnitsInStock = (Convert.ToInt32(maxUnitRow["unidades"]) * currentQuantityInStock) + Convert.ToInt32(currentFractionInStock);

                int totalUnits = totalProductUnitsInStock - totalProductUnits;

                int maxProductUnit = Convert.ToInt32(maxUnitRow["unidades"]);

                int newQuantity = 0;
                int newFraction = 0;

                if (totalUnits >= maxProductUnit)
                {
                    int result = totalUnits / maxProductUnit;
                    int mod = totalUnits % maxProductUnit;
                    
                    newQuantity = result;
                    newFraction = mod;
                }
                else
                {
                    if (totalUnits < maxProductUnit)
                    {
                        newQuantity = 0;
                        newFraction = totalUnits;
                    }
                    else
                    {
                        newQuantity = currentQuantityInStock;
                        newFraction = totalUnits;
                    }
                }

                if (unitIdInSale == Convert.ToInt32(maxUnitRow["id_unidad"]))
                {
                    newQuantity = currentQuantityInStock - productQuantityInSale;
                    newFraction = currentFractionInStock;
                }
                if (unitIdInSale == Convert.ToInt32(minUnitRow["id_unidad"]))
                {
                    if (totalUnits >= maxProductUnit)
                    {
                        int re = totalUnits / maxProductUnit;
                        int mod = totalUnits % maxProductUnit;

                        newQuantity = re;
                        newFraction = mod;
                    }
                    else
                    {
                        if (totalUnits < maxProductUnit)
                        {
                            newQuantity = 0;
                            newFraction = totalUnits;
                        }
                        else
                        {
                            if (currentQuantityInStock > 0)
                            {
                                newQuantity = currentQuantityInStock;
                                newFraction = totalUnits;
                            }
                            else
                            {
                                if (productUnits.Rows.Count > 1)
                                {
                                    newQuantity = 0;
                                    newFraction = productQuantityInSale;
                                }
                                else
                                {
                                    newQuantity = productQuantityInSale;
                                    newFraction = 0;
                                }
                            }
                        }
                    }
                }

                decimal saleAverage;
                if (productUnitCost == null || productUnitCost == 0)
                {
                    DataTable dtsaleAverage = CalculateSaleAverage(product_id);
                    if (dtsaleAverage.Rows.Count > 0)
                    {
                        DataRow r = dtsaleAverage.Rows[0];
                        decimal unitCost = (Convert.ToDecimal(r["precio"]) / Convert.ToInt32(r["unidades"])) * maxProductUnit;
                        saleAverage = (unitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow["unidades"]);
                    }
                    else
                    {
                        saleAverage = 0;
                    }
                }
                else
                {
                    saleAverage = (productUnitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow["unidades"]);
                }

                decimal utility = (productPrice - saleAverage) * productQuantityInSale;

                query = @"INSERT INTO detalle_venta (id_venta,id_producto,precio,catidad,unidad_medida,detalle_costo_promedio,detalle_utilidad,detalle_importe)
                                    VALUES(@sale_id,@product_id,@price,@quantity,@unit_id,@sale_average,@sale_utility,@product_total)";

                MySqlCommand command = new MySqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@sale_id", this.SaleID);
                command.Parameters.AddWithValue("@product_id", product_id);
                command.Parameters.AddWithValue("@price", productPrice);
                command.Parameters.AddWithValue("@quantity", productQuantityInSale);
                command.Parameters.AddWithValue("@unit_id", unitIdInSale);
                command.Parameters.AddWithValue("@sale_average", saleAverage);
                command.Parameters.AddWithValue("@sale_utility", utility);
                command.Parameters.AddWithValue("@product_total", productTotal);

                command.ExecuteScalar();

                if (currentStock.Rows.Count > 0)
                {
                    int stock_id = Convert.ToInt32(currentStock.Rows[0]["id_inventario"]);
                    UpdateStock(stock_id, newQuantity, newFraction);
                }
                else
                {
                    InsertStock(newQuantity, newFraction);
                }
            }
        }

        private void InsertStock(int quantity, int fraction)
        {
            string query = "INSERT INTO inventario (cantidad, fraccion) VALUES(@quantity, @fraction)";
            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@fraction", fraction);
            command.ExecuteNonQuery();
        }

        private void UpdateStock(int stock_id, int quantity, int fraction)
        {
            string query = "UPDATE inventario SET cantidad=@quantity,fraccion=@fraction WHERE id_inventario=@stock_id";
            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@fraction", fraction);
            command.Parameters.AddWithValue("@stock_id", stock_id);

            command.ExecuteNonQuery();
        }

        private DataTable CalculateSaleAverage(int product_id)
        {
            string query = @"SELECT detalleingreso.*, ingreso.fecha_registro, unidades_has_producto.* 
	                            FROM detalleingreso
                            JOIN ingreso ON ingreso.id_ingreso=detalleingreso.id_ingreso
                            JOIN unidades ON unidades.id_unidad=detalleingreso.unidad_medida
                            JOIN unidades_has_producto ON unidades_has_producto.id_unidad=detalleingreso.unidad_medida
                            AND unidades_has_producto.producto_id=detalleingreso.id_producto
                            WHERE detalleingreso.id_producto=@product_id
	                            AND fecha_registro=
                                (SELECT MAX(fecha_registro) FROM ingreso
			                            JOIN detalleingreso 
				                            ON detalleingreso.id_ingreso=ingreso.id_ingreso 
			                            WHERE detalleingreso.id_producto=@product_id)";

            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@product_id", product_id);
            var dt = new DataTable();
            MySqlDataReader reader = command.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        private DataTable GetCurrentProductStock(int product_id) 
        {
            string query = @"SELECT id_inventario, cantidad, fraccion
						        FROM inventario where id_producto=@product_id and id_local=@local_id";
            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@product_id", product_id);
            command.Parameters.AddWithValue("@local_id", this.LocalID);

            var dt = new DataTable();
            MySqlDataReader reader = command.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        private DataTable GetProductUnitMeasure(int product_id)
        {
            string query = "SELECT * FROM unidades_has_producto WHERE producto_id=@product_id order by orden asc";
            MySqlCommand command = new MySqlCommand(query, this.connection, this.transaction);
            command.Parameters.AddWithValue("@product_id", product_id);

            var dt = new DataTable();
            MySqlDataReader rdr = command.ExecuteReader();
            dt.Load(rdr);
            return dt;
        }
    }
}
