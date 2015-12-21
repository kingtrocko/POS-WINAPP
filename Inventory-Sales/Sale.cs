using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics;

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
        public int PaymentConditionDays { get; set; }
        public string SaleStatus { get; set; }
        public int QuotaNumber { get; set; }
        public decimal AmountPerQuota { get; set; }
        
        private long SaleID { get; set; }
        
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

       
        public bool SaveSale()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            try
            {
                this.DocumentID = InsertDocumentSale();
                this.SaleID = InserSaleHeader();
                InsertSaleDetail();

                this.transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine("==================== ERROR MESSAGE ====================");
                Debug.WriteLine(e.Message);
                Debug.WriteLine("==================== ERROR STACK TRACE ====================");
                Debug.WriteLine(e.StackTrace);
                
                return false;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public bool UpdateExistingSale()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            try
            {
                RemoveProducItemsFromInventory();
                UpdateSaleHeader();
                DeleteSaleDetail();


            }
            catch (Exception e)
            {

            }
        }

        private long InsertDocumentSale()
        {
            string query = @"INSERT INTO documento_venta (nombre_tipo_documento, documento_Serie, documento_Numero) 
                            VALUES(@name, @serie, @number); select last_insert_id();";
            MySqlCommand command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@name", this.DocumentType);
            command.Parameters.AddWithValue("@serie", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["SERIE"]));
            command.Parameters.AddWithValue("@number", Convert.ToInt32(this.DocumentTypeInfo.Rows[0]["NUMERO"]));

            int id = Convert.ToInt32(command.ExecuteScalar());
            Debug.WriteLine("INSERTED in documento_venta with ID: " + id.ToString());

            return id;
        }

        private long InserSaleHeader()
        {
            string query = @"INSERT INTO venta (fecha,id_cliente,id_vendedor,condicion_pago,venta_status,local_id,subtotal,total_impuesto,total,numero_documento,pagado,vuelto)
                               VALUES(@date,@client_id,@salesman_id,@payment_condition,@status,@local_id,@subtotal,@tax,@total,@doc_number,@payed,@change);
                                select last_insert_id();";
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
            Debug.WriteLine("INSERTED in venta with ID: " + id.ToString());
            return id;
        }

        private void InsertSaleDetail()
        {
            string query = "";
            foreach (DataRow row in this.Products.Rows)
            {
                int product_id = Convert.ToInt32(row["producto_id"]);
                var currentStock = GetExistingInventory(product_id);
                int currentQuantityInStock = 0; 
                int currentFractionInStock = 0;

                if (currentStock.Rows.Count > 0)
                {
                    if (currentStock.Rows[0]["cantidad"] != null)
                        currentQuantityInStock = Convert.ToInt32(currentStock.Rows[0]["cantidad"]);

                    if (currentStock.Rows[0]["fraccion"] != null)
                        currentFractionInStock = Convert.ToInt32(currentStock.Rows[0]["fraccion"]);

                }

                int productQuantityInSale = Convert.ToInt32(row["cantidad"]);
                int unitIdInSale = Convert.ToInt32(row["id_unidad"]);
                decimal productPrice = Convert.ToDecimal(row["precio"]);
                decimal productTotal = productQuantityInSale * productPrice;  //Convert.ToDecimal(row["Total"]);
                decimal productUnitCost = row["producto_costo_unitario"] is DBNull ? 0 : Convert.ToDecimal(row["producto_costo_unitario"]);

                DataTable productUnits = GetProductUnitMeasure(product_id);

                DataRow minUnitRow = productUnits.Rows[0];
                DataRow maxUnitRow = productUnits.Rows[productUnits.Rows.Count - 1];

                DataRow[] unitsInSaleRow = productUnits.Select("id_unidad = " + unitIdInSale.ToString());

                int totalProductUnits = Convert.ToInt32(unitsInSaleRow[0]["unidades"]) * productQuantityInSale;
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
                        saleAverage = (unitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow[0]["unidades"]);
                    }
                    else
                    {
                        saleAverage = 0;
                    }
                }
                else
                {
                    saleAverage = (productUnitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow[0]["unidades"]);
                }

                decimal utility = (productPrice - saleAverage) * productQuantityInSale;

                query = @"INSERT INTO detalle_venta (id_venta,id_producto,precio,cantidad,unidad_medida,detalle_costo_promedio,detalle_utilidad,detalle_importe)
                                    VALUES(@sale_id,@product_id,@price,@quantity,@unit_id,@sale_average,@sale_utility,@product_total);
                            select last_insert_id();";

                MySqlCommand command = new MySqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@sale_id", this.SaleID);
                command.Parameters.AddWithValue("@product_id", product_id);
                command.Parameters.AddWithValue("@price", productPrice);
                command.Parameters.AddWithValue("@quantity", productQuantityInSale);
                command.Parameters.AddWithValue("@unit_id", unitIdInSale);
                command.Parameters.AddWithValue("@sale_average", saleAverage);
                command.Parameters.AddWithValue("@sale_utility", utility);
                command.Parameters.AddWithValue("@product_total", productTotal);

                int id = Convert.ToInt32(command.ExecuteScalar());

                Debug.WriteLine("INSERTED in detalle_venta with ID " + id.ToString());

                if (currentStock.Rows.Count > 0)
                {
                    int stock_id = Convert.ToInt32(currentStock.Rows[0]["id_inventario"]);
                    UpdateInventory(stock_id, newQuantity, newFraction);
                }
                else
                {
                    InsertInventory(newQuantity, newFraction);
                }
            }

            if (this.PaymentConditionDays < 1)
                InsertCashSale();
            else
                InsertCreditSale();
        }

        private void InsertCashSale()
        {
            string query = "INSERT INTO contado (id_venta, status, montopagado) VALUES(@sale_id, @status, @grand_total); select last_insert_id();";
            MySqlCommand cmd = new MySqlCommand(query, this.connection, this.transaction);
            cmd.Parameters.AddWithValue("@sale_id", this.SaleID);
            cmd.Parameters.AddWithValue("@status", "PagoCancelado");
            cmd.Parameters.AddWithValue("@grand_total", this.GrandTotal);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            Debug.WriteLine("INSERTED in contado with ID: " + id.ToString());
        }

        private void InsertCreditSale()
        {
            string query = @"INSERT INTO credito (id_venta, int_credito_nrocuota, dec_credito_montocuota, var_credito_estado, dec_credito_montodebito) 
                            VALUES(@sale_id, @num_cuota, @monto_cuota, @status, @monto_debito);
                            select last_insert_id();";
            MySqlCommand cmd = new MySqlCommand(query, this.connection, this.transaction);
            cmd.Parameters.AddWithValue("@sale_id", this.SaleID);
            cmd.Parameters.AddWithValue("@num_cuota", this.QuotaNumber);
            cmd.Parameters.AddWithValue("@monto_cuota", this.AmountPerQuota);
            cmd.Parameters.AddWithValue("@status", "Debito");
            cmd.Parameters.AddWithValue("@monto_debito", 0.0);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            Debug.WriteLine("INSERTED in credito with ID: " + id.ToString());
        }

        private void InsertInventory(int quantity, int fraction)
        {
            string query = "INSERT INTO inventario (cantidad, fraccion) VALUES(@quantity, @fraction); select last_insert_id();";
            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@fraction", fraction);
            
            int id = Convert.ToInt32(command.ExecuteScalar());

            Debug.WriteLine("INSERTED in inventario with ID: " + id.ToString());
        }

        private void UpdateInventory(int stock_id, int quantity, int fraction)
        {
            string query = "UPDATE inventario SET cantidad=@quantity,fraccion=@fraction WHERE id_inventario=@stock_id";
            MySqlCommand command = new MySqlCommand(query, this.connection, transaction);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@fraction", fraction);
            command.Parameters.AddWithValue("@stock_id", stock_id);

            int rowsAffected = command.ExecuteNonQuery();
            Debug.WriteLine("UPDATED! in inventario. rows affected: " + rowsAffected.ToString());
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

        private DataTable GetExistingInventory(int product_id) 
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

        private void RemoveProducItemsFromInventory()
        {
            DataTable details = GetSaleDetail();
            foreach (DataRow row in details.Rows)
            {
                this.LocalID = Convert.ToInt32(row["local_id"]);
                int maxUnit = Convert.ToInt32(row["unidades"]);
                int productId = Convert.ToInt32(row["id_producto"]);
                int unitId = Convert.ToInt32(row["unidad_medida"]);
                int productQuantity = Convert.ToInt32(row["cantidad"]);

                DataTable existingInventory = GetExistingInventory(productId);

                int inventoryId = Convert.ToInt32(existingInventory.Rows[0]["id_inventario"]);
                int currentQuantityInStock = Convert.ToInt32(existingInventory.Rows[0]["cantidad"]);
                int currentFractionInStock = Convert.ToInt32(existingInventory.Rows[0]["fraccion"]);

                DataTable productUnits = GetProductUnitMeasure(productId);
                DataRow[] unitsRow = productUnits.Select("id_unidad = " + unitId.ToString());

                int minUnitsInStock = 0;

                if(currentQuantityInStock >= 1){
                    minUnitsInStock = (currentQuantityInStock * maxUnit) + currentFractionInStock;
                }else{
                    minUnitsInStock = currentFractionInStock;
                }

                int minUnitsInDetail = Convert.ToInt32(unitsRow["unidades"]) * productQuantity;
                int sum = minUnitsInStock + minUnitsInDetail;
                int cont = 0;
                int newQuantity = 0;
                int newFraction = 0;

                while(sum >= maxUnit){
                    cont++;
                    sum = sum - maxUnit
                }
                if(cont < 1){
                    newQuantity = 0;
                    newFraction = sum;
                }else{
                    newQuantity = cont;
                    newFraction = sum;
                }

                UpdateInventory(inventoryId, newQuantity, newFraction);
            }
        }

        private DataTable GetSaleDetail()
        {
            string query = @"SELECT *
                            FROM detalle_venta
                                    JOIN producto ON producto.producto_id = detalle_venta.id_producto
                                    LEFT JOIN unidades_has_producto ON unidades_has_producto.producto_id = producto.producto_id AND unidades_has_producto.orden = 1
                                    LEFT JOIN unidades ON unidades.id_unidad = unidades_has_producto.id_unidad
                                    JOIN venta ON venta.venta_id = detalle_venta.id_venta
                            WHERE detalle_venta.id_venta = @sale_id;";

            MySqlCommand cmd = new MySqlCommand(query, this.connection, this.transaction);
            cmd.Parameters.AddWithValue("@sale_id", this.SaleID);

            var dt = new DataTable();
            MySqlDataReader reader = cmd.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        private void UpdateSaleHeader()
        {
            string query = @"UPDATE venta SET fecha=@date,id_vendedor=@salesman_id, local_id=@local_id, subtotal=@subtotal,total_impuesto=@total_tax,total=@grand_total,
                            pagado=@money_paid, vuelto=@money_change, id_cliente=@client_id,venta_status=@status,@condicion_pago=@payment_condition
                            WHERE venta id_venta = @sale_id";
            MySqlCommand command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@date", this.SaleDate);
            command.Parameters.AddWithValue("@salesman_id", this.SalesmanID);
            command.Parameters.AddWithValue("@local_id", this.LocalID);
            command.Parameters.AddWithValue("@subtotal", this.SubTotal);
            command.Parameters.AddWithValue("@total_tax", this.Tax);
            command.Parameters.AddWithValue("@grand_total", this.GrandTotal);
            command.Parameters.AddWithValue("@money_paid", this.AmountPaid);
            command.Parameters.AddWithValue("@,pney_change", this.MoneyChange);
            command.Parameters.AddWithValue("@client_id", this.ClientID);
            command.Parameters.AddWithValue("@status", this.SaleStatus);
            command.Parameters.AddWithValue("@payment_condition", this.PaymentConditionID);
            int rowsAffected = command.ExecuteNonQuery();

            Debug.WriteLine("UPDATED venta. rows affected = " + rowsAffected.ToString());

        }

        private void DeleteSaleDetail()
        {
            string query = "DELETE FROM detalle_venta where id_venta = @sale_id";
            MySqlCommand cmd = new MySqlCommand(query, this.connection, this.transaction);
            int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

            Debug.WriteLine("DELETED from detalle_venta. rows affected: " + rowsAffected.ToString());
        }

        private void UpdateSaleDetail()
        {
            string query = "";
            foreach (DataRow row in this.Products.Rows)
            {
                int product_id = Convert.ToInt32(row["producto_id"]);
                var currentStock = GetExistingInventory(product_id);
                int currentQuantityInStock = 0;
                int currentFractionInStock = 0;

                if (currentStock.Rows.Count > 0)
                {
                    if (currentStock.Rows[0]["cantidad"] != null)
                        currentQuantityInStock = Convert.ToInt32(currentStock.Rows[0]["cantidad"]);

                    if (currentStock.Rows[0]["fraccion"] != null)
                        currentFractionInStock = Convert.ToInt32(currentStock.Rows[0]["fraccion"]);

                }

                int productQuantityInSale = Convert.ToInt32(row["cantidad"]);
                int unitIdInSale = Convert.ToInt32(row["id_unidad"]);
                decimal productPrice = Convert.ToDecimal(row["precio"]);
                decimal productTotal = productQuantityInSale * productPrice;  //Convert.ToDecimal(row["Total"]);
                decimal productUnitCost = row["producto_costo_unitario"] is DBNull ? 0 : Convert.ToDecimal(row["producto_costo_unitario"]);

                DataTable productUnits = GetProductUnitMeasure(product_id);

                DataRow minUnitRow = productUnits.Rows[0];
                DataRow maxUnitRow = productUnits.Rows[productUnits.Rows.Count - 1];
                DataRow[] unitsInSaleRow = productUnits.Select("id_unidad = " + unitIdInSale.ToString());

                int totalMinUnits = Convert.ToInt32(unitsInSaleRow[0]["unidades"]) * productQuantityInSale;
                //int totalProductUnitsInStock = (Convert.ToInt32(maxUnitRow["unidades"]) * currentQuantityInStock) + Convert.ToInt32(currentFractionInStock);
                int maxProductUnit = Convert.ToInt32(maxUnitRow["unidades"]);
                int totalUnits = 0;
                int newQuantity = 0;
                int newFraction = 0;


                if (currentFractionInStock < totalMinUnits){
                    totalUnits = totalMinUnits - currentFractionInStock;
                }else {
                    totalUnits = currentFractionInStock - totalMinUnits;
                }

                if (totalUnits >= maxProductUnit)
                {
                    int result = totalUnits / maxProductUnit;
                    int mod = totalUnits % maxProductUnit;

                    newQuantity = result - currentQuantityInStock;
                    newFraction = mod;
                }
                else
                {

                    newQuantity = currentQuantityInStock;
                    newFraction = totalUnits;
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

                        newQuantity = re - currentQuantityInStock;
                        newFraction = mod;
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

                decimal saleAverage;
                if (productUnitCost == null || productUnitCost == 0)
                {
                    DataTable dtsaleAverage = CalculateSaleAverage(product_id);
                    if (dtsaleAverage.Rows.Count > 0)
                    {
                        DataRow r = dtsaleAverage.Rows[0];
                        decimal unitCost = (Convert.ToDecimal(r["precio"]) / Convert.ToInt32(r["unidades"])) * maxProductUnit;
                        saleAverage = (unitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow[0]["unidades"]);
                    }
                    else
                    {
                        saleAverage = 0;
                    }
                }
                else
                {
                    saleAverage = (productUnitCost / maxProductUnit) * Convert.ToInt32(unitsInSaleRow[0]["unidades"]);
                }

                decimal utility = (productPrice - saleAverage) * productQuantityInSale;

                query = @"INSERT INTO detalle_venta (id_venta,id_producto,precio,cantidad,unidad_medida,detalle_costo_promedio,detalle_utilidad,detalle_importe)
                                    VALUES(@sale_id,@product_id,@price,@quantity,@unit_id,@sale_average,@sale_utility,@product_total);
                            select last_insert_id();";

                MySqlCommand command = new MySqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@sale_id", this.SaleID);
                command.Parameters.AddWithValue("@product_id", product_id);
                command.Parameters.AddWithValue("@price", productPrice);
                command.Parameters.AddWithValue("@quantity", productQuantityInSale);
                command.Parameters.AddWithValue("@unit_id", unitIdInSale);
                command.Parameters.AddWithValue("@sale_average", saleAverage);
                command.Parameters.AddWithValue("@sale_utility", utility);
                command.Parameters.AddWithValue("@product_total", productTotal);

                int id = Convert.ToInt32(command.ExecuteScalar());

                Debug.WriteLine("INSERTED in detalle_venta with ID " + id.ToString());

                if (currentStock.Rows.Count > 0)
                {
                    int stock_id = Convert.ToInt32(currentStock.Rows[0]["id_inventario"]);
                    UpdateInventory(stock_id, newQuantity, newFraction);
                }
                else
                {
                    InsertInventory(newQuantity, newFraction);
                }
            }

            if (this.PaymentConditionDays < 1)
                InsertCashSale();
            else
                InsertCreditSale();
        }
    
    }
}
