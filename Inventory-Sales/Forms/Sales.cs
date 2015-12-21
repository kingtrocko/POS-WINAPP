using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Inventory_Sales.Forms
{
    public partial class Sales : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dtSelectedProducts;
        private DataTable dtPricesTypes;
        private DataTable dtPaymentTypes;
        private InventoryAPI API;
        private Sale Sale;
        private int selectedProductRowHandle = -1;
        private decimal taxPercentage;

        public Sales()
        {
            InitializeComponent();
            
            //Instantiate Objects
            API = new InventoryAPI();
            Sale = new Sale();

            //Get prices types from API
            dtPricesTypes = API.GetPricesTypes();

            //Get tax percentage
            taxPercentage = Convert.ToDecimal(API.GetTax().Rows[0]["porcentaje_impuesto"]) / 100;

            //Sets datasource for all products GridControl
            gcAllProducts.DataSource = API.GetAllProducts("1");
            gcSelectedProducts.DataSource = InitializeGridControlDataSource();

            //sets datasource to clients GridView
            sleClients.Properties.DataSource = API.GetClients();
            sleClients.Properties.DisplayMember = "razon_social";
            sleClients.Properties.ValueMember = "id_cliente";

            //Some settings for gridviews
            gvAllProducts.BestFitColumns(true);
            gvPrecioVenta.BestFitColumns(true);
            gvPrecioDescuento.BestFitColumns(true);
            gvSelectedProducts.BestFitColumns(true);

            SetPaymentTypeDataSource();
            SetDocumentTypesDataSource();
            SetSaleStatusDataSource();

            txtSaleId.Text = "";
        }

        private void SetSaleStatusDataSource()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("status", typeof(string));

            DataRow row = dt.NewRow();
            row["id"] = 1;
            row["status"] = "COMPLETADO";

            DataRow row2 = dt.NewRow();
            row2["id"] = 2;
            row2["status"] = "EN ESPERA";

            dt.Rows.Add(row);
            dt.Rows.Add(row2);

            lueSaleStatus.Properties.DataSource = dt;
            lueSaleStatus.Properties.DisplayMember = "status";
            lueSaleStatus.Properties.ValueMember = "id";
        }

        private void SetDocumentTypesDataSource()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("document_type", typeof(string));

            DataRow row = dt.NewRow();
            row["id"] = "BOLETA DE VENTA";
            row["document_type"] = "BOLETA DE VENTA";

            DataRow row2 = dt.NewRow();
            row2["id"] = "NOTA DE PEDIDO";
            row2["document_type"] = "NOTA DE PEDIDO";

            DataRow row3 = dt.NewRow();
            row3["id"] = "FACTURA";
            row3["document_type"] = "FACTURA";

            dt.Rows.Add(row);
            dt.Rows.Add(row2);
            dt.Rows.Add(row3);

            lueDocumentTypes.Properties.DataSource = dt;
            lueDocumentTypes.Properties.DisplayMember = "document_type";
            lueDocumentTypes.Properties.ValueMember = "id";
        }

        private void SetPaymentTypeDataSource()
        {
            dtPaymentTypes = API.GetPaymentConditions();
            luePaymentTypes.Properties.DataSource = dtPaymentTypes;
            luePaymentTypes.Properties.DisplayMember = "nombre_condiciones";
            luePaymentTypes.Properties.ValueMember = "id_condiciones";
        }

        private DataTable InitializeGridControlDataSource()
        {
            dtSelectedProducts = new DataTable();

            dtSelectedProducts.Columns.Add("producto_id", typeof(int));
            dtSelectedProducts.Columns.Add("item_number", typeof(int));
            dtSelectedProducts.Columns.Add("producto_nombre", typeof(string));
            dtSelectedProducts.Columns.Add("unidad_medida", typeof(string));
            dtSelectedProducts.Columns.Add("cantidad", typeof(int));
            dtSelectedProducts.Columns.Add("precio", typeof(decimal));
            dtSelectedProducts.Columns.Add("id_unidad", typeof(int));
            //dtSelectedProducts.Columns.Add("id_precio", typeof(int));
            dtSelectedProducts.Columns.Add("producto_costo_unitario", typeof(decimal));

            return dtSelectedProducts;
        }

        private void gvSelectedProducts_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            txtSubTotal.Text = getSubtotal().ToString();
        }

        private void gvSelectedProducts_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            txtSubTotal.Text = getSubtotal().ToString();
        }

        private Decimal getSubtotal()
        {
            decimal subtotal = 0.0M;
            for (int i = 0; i < gvSelectedProducts.RowCount; i++)
                subtotal += Convert.ToDecimal(gvSelectedProducts.GetRowCellValue(i, "Total"));

            return subtotal;
        }

        private void gvAllProducts_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvAllProducts_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            ColumnView columnView = (ColumnView)sender;
            var product_id = Convert.ToString(columnView.GetRowCellValue(e.RowHandle, "producto_id"));
            var price_id = Convert.ToString(dtPricesTypes.Rows[e.RelationIndex]["id_precio"]);

            IList list = API.GetPricesPerProduct(product_id, price_id).DefaultView;

            e.ChildList = list;
        }

        private void gvAllProducts_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = dtPricesTypes.Rows.Count;
        }

        private void gvAllProducts_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            int index = e.RelationIndex;
            string name = dtPricesTypes.Rows[index]["nombre_precio"].ToString();
            if(name.Equals("Precio Venta"))
                e.RelationName = "Precio_Venta";
            else
                e.RelationName = "Precio_Descuento";                
        }

        private void gvAllProducts_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView grid = sender as GridView;
            grid.ExpandMasterRow(e.FocusedRowHandle);
            selectedProductRowHandle = e.FocusedRowHandle;
        }

        private void gvPrecioVenta_DoubleClick(object sender, EventArgs e)
        {
            GridView gv = sender as GridView;
            Point pt = gv.GridControl.PointToClient(Control.MousePosition);
            DoDoubleClick(gv, pt);
        }

        private void DoDoubleClick(GridView view, Point p)
        {
            GridHitInfo info = view.CalcHitInfo(p);
            if (info.InRow || info.InRowCell)
            {
                DataRowView selectedPriceRow = (DataRowView)view.GetRow(info.RowHandle);
                DataRowView selectedProductRow = (DataRowView)view.SourceRow;

                string precio = Convert.ToString(selectedPriceRow.Row["precio"]);
                string nombre_unidad = Convert.ToString(selectedPriceRow.Row["nombre_unidad"]);

                DataRow row = dtSelectedProducts.NewRow();
                row["producto_id"] = selectedProductRow.Row["producto_id"];
                row["item_number"] = gvSelectedProducts.RowCount + 1;
                row["producto_nombre"] = selectedProductRow.Row["producto_nombre"];
                row["unidad_medida"] = nombre_unidad;
                row["cantidad"] = 1;
                row["precio"] = precio;
                row["id_unidad"] = selectedPriceRow.Row["id_unidad"];
                //row["id_precio"] = selectedPriceRow.Row["id_precio"];
                row["producto_costo_unitario"] = selectedProductRow.Row["producto_costo_unitario"];

                dtSelectedProducts.Rows.Add(row);

                gcSelectedProducts.DataSource = dtSelectedProducts;
                pceSearchProduct.ClosePopup();
                pceSearchProduct.Text = Convert.ToString(selectedProductRow.Row["producto_nombre"]) + " - " + nombre_unidad;

                gcSelectedProducts.Focus();
                gvSelectedProducts.FocusedRowHandle = gvSelectedProducts.RowCount - 1;
                gvSelectedProducts.FocusedColumn = gvSelectedProducts.VisibleColumns[3];
                gvSelectedProducts.ShowEditor();
            }
        }

        private void gvAllProducts_ColumnFilterChanged(object sender, EventArgs e)
        {

        }

        private void sleClients_EditValueChanged(object sender, EventArgs e)
        {
            string client_id = Convert.ToString(sleClientsView.GetFocusedRowCellValue("id_cliente"));
            string client_name = Convert.ToString(sleClientsView.GetFocusedRowCellValue("razon_social"));

            //sleClients.Text = String.Format("{0} - {1}", client_id, client_name);
            pceSearchProduct.Focus();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenSale openSale = new OpenSale("1", "EN ESPERA");
            DialogResult result = openSale.ShowDialog(this);

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DataTable products = openSale.SaleProducts;
                this.txtSaleId.Text = products.Rows[0]["venta_id"].ToString();
                this.lueDocumentTypes.EditValue = products.Rows[0]["nombre_tipo_documento"].ToString();
                this.sleClients.EditValue = products.Rows[0]["cliente_id"];
                this.luePaymentTypes.EditValue = products.Rows[0]["id_condiciones"];
                this.lueSaleStatus.EditValue = 1;

                dtSelectedProducts = products;
                this.gcSelectedProducts.DataSource = dtSelectedProducts;

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string validationMessage = ValidateRequeriedFields();

            if (validationMessage.Equals(string.Empty))
            {
                string paymentType = luePaymentTypes.Text;
                SaveSaleDialog dialog = new SaveSaleDialog(txtTotal.Text, paymentType);
                dialog.PaymentType = luePaymentTypes.Text;

                string saleStatus = lueSaleStatus.Text;
                bool success = false;

                if (txtSaleId.Text == string.Empty)
                {
                    if (saleStatus != "EN ESPERA")
                    {
                        DialogResult dialogResult = dialog.ShowDialog(this);

                        if (dialogResult == System.Windows.Forms.DialogResult.OK)
                            success = SaveSale(dialog);
                    }
                    else
                        success = SaveSale(dialog);
                }
                else //UPDATE sale
                {

                }
                
                

                if (success){
                    XtraMessageBox.Show("Felicidades! La venta se ha Guardado Exitosamente");
                    ClearFields();
                }
                else
                    XtraMessageBox.Show("Ocurrio un error al momento de Guardar la Venta", "ERROR");
            }
            else
            {
                XtraMessageBox.Show(validationMessage, "Acciones Requeridas");
            }

           
        }

        private string ValidateRequeriedFields()
        {
            string message = "";
            DataTable selectedProducts = ((DataTable)gcSelectedProducts.DataSource);
            StringBuilder sb = new StringBuilder();

            if (sleClients.Text == string.Empty && luePaymentTypes.Text == string.Empty && 
                lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty && selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Seleccione un cliente");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Documento" );
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (sleClients.Text == string.Empty && luePaymentTypes.Text == string.Empty &&
                lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Seleccione un cliente");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
            }
            else if (sleClients.Text == string.Empty && luePaymentTypes.Text == string.Empty && lueDocumentTypes.Text == string.Empty)
            {
                sb.Append("- Seleccione un cliente");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Documento");
            }
            else if (sleClients.Text == string.Empty && luePaymentTypes.Text == string.Empty)
            {
                sb.Append("- Seleccione un cliente");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
            }
            else if (sleClients.Text == string.Empty)
            {
                sb.Append("- Seleccione un cliente");
            }
            else if (luePaymentTypes.Text == string.Empty &&
               lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty && selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (luePaymentTypes.Text == string.Empty && lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
            }
            else if (luePaymentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
            }
            else if (luePaymentTypes.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
            }
            else if (lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty && selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (lueDocumentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
            }
            else if (lueDocumentTypes.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Documento");
            }
            else if (lueSaleStatus.Text == string.Empty && selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Especifique el Estado de la Venta");
            }
            else if (selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (selectedProducts.Rows.Count <= 0 && sleClients.Text == string.Empty)
            {
                sb.Append("- Seleccione un cliente");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (selectedProducts.Rows.Count <= 0 && luePaymentTypes.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (selectedProducts.Rows.Count <= 0 && lueDocumentTypes.Text == string.Empty)
            {
                sb.Append("- Especifique el tipo de Documento");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (selectedProducts.Rows.Count <= 0 && lueSaleStatus.Text == string.Empty)
            {
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            }
            else if (lueDocumentTypes.Text == string.Empty && luePaymentTypes.Text == string.Empty)
            {
                sb.Append("- Especifique el Tipo de Documento");
                sb.AppendLine();
                sb.Append("- Especifique el Tipo de Pago (Contado o Crédito)");
            }
            else if (lueSaleStatus.Text == string.Empty && sleClients.Text == string.Empty)
            {
                sb.Append("- Seleccione un Cliente");
                sb.AppendLine();
                sb.Append("- Seleccione el Estado de la Venta");                
            }
            else if (luePaymentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty && selectedProducts.Rows.Count <= 0)
            {
                sb.Append("- Especifique el Tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
                sb.AppendLine();
                sb.Append("- Agregue por lo menos un producto a la Venta");
            } else if(sleClients.Text == string.Empty && luePaymentTypes.Text == string.Empty && lueSaleStatus.Text == string.Empty){
                sb.Append("- Seleccione un Cliente");
                sb.AppendLine();
                sb.Append("- Especifique el Tipo de Pago (Contado o Crédito)");
                sb.AppendLine();
                sb.Append("- Especifique el Estado de la Venta");
            }

            return sb.ToString();
        }

        private bool SaveSale(SaveSaleDialog dialog)
        {
            Sale.DocumentTypeInfo = API.GetDocumentSale(lueDocumentTypes.Text);
            Sale.ClientID = Convert.ToInt32(sleClients.EditValue);
            Sale.SubTotal = Convert.ToDecimal(txtSubTotal.Text);
            Sale.Tax = Convert.ToDecimal(txtTax.Text);
            Sale.GrandTotal = Convert.ToDecimal(txtTotal.Text);
            Sale.SalesmanID = 11; // TODO
            Sale.LocalID = 1; // TODO
            Sale.MoneyChange = Convert.ToDecimal(dialog.MoneyChange);
            Sale.AmountPaid = Convert.ToDecimal(dialog.MoneyPaid);
            Sale.SaleDate = DateTime.Now;
            Sale.DocumentType = lueDocumentTypes.Text;
            Sale.PaymentConditionID = Convert.ToInt32(luePaymentTypes.EditValue);
            Sale.SaleStatus = lueSaleStatus.Text;
            Sale.Products = gcSelectedProducts.DataSource as DataTable;

            DataRowView row = luePaymentTypes.Properties.GetDataSourceRowByKeyValue(luePaymentTypes.EditValue) as DataRowView;
            Sale.PaymentConditionDays = Convert.ToInt32(row["dias"]);

            if (lueSaleStatus.Text == "EN ESPERA")
            {
                Sale.QuotaNumber = 1;
                Sale.AmountPerQuota = Convert.ToDecimal(this.txtTotal.Text);
            }

            return Sale.SaveSale();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            Sale = null;
            Sale = new Sale();
            sleClients.Text = "";
            sleClientsView.SelectRow(-1);
            pceSearchProduct.Text = "";
            gvAllProducts.SelectRow(0);
            gcSelectedProducts.DataSource = InitializeGridControlDataSource();
            txtTotal.Text = "0.00";
            txtSubTotal.Text = "0.00";
            txtTax.Text = "0.00";
            txtSaleId.Text = "";
            sleClients.Focus();
            //lueDocumentTypes.Text = "";
            //luePaymentTypes.Text = "";
            //lueSaleStatus.Text = "";
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {

        }

        private void txtSubTotal_EditValueChanged(object sender, EventArgs e)
        {
            if (txtSubTotal.Text != string.Empty)
            {
                decimal subtotal = Convert.ToDecimal(txtSubTotal.Text);
                decimal totalTax = subtotal * taxPercentage;

                txtTax.Text = totalTax.ToString();
                txtTotal.Text = (subtotal + totalTax).ToString();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}