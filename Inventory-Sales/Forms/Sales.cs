﻿using System;
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
        DataTable dtSelectedProducts;
        DataTable dtPricesTypes;
        InventoryAPI API;
        Sale Sale;
        int selectedProductRowHandle = -1;

        public Sales()
        {
            InitializeComponent();
            
            //Instantiate Objects
            API = new InventoryAPI();
            Sale = new Sale();

            //Get prices types from API
            dtPricesTypes = API.GetPricesTypes();

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
            row["id"] = 2;
            row["status"] = "EN ESPERA";

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
            row["id"] = "BOLETAVENTA";
            row["document_type"] = "BOLETA DE VENTA";

            DataRow row2 = dt.NewRow();
            row["id"] = "NOTAVENTA";
            row["document_type"] = "NOTA DE VENTA";

            DataRow row3 = dt.NewRow();
            row["id"] = "FACTURA";
            row["document_type"] = "FACTURA";

            dt.Rows.Add(row);
            dt.Rows.Add(row2);
            dt.Rows.Add(row3);

            lueDocumentTypes.Properties.DataSource = dt;
            lueDocumentTypes.Properties.DisplayMember = "document_type";
            lueDocumentTypes.Properties.ValueMember = "id";
        }

        private void SetPaymentTypeDataSource()
        {
            luePaymentTypes.Properties.DataSource = API.GetPaymentConditions();
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
            dtSelectedProducts.Columns.Add("id_precio", typeof(int));
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
                row["id_precio"] = selectedPriceRow.Row["id_precio"];
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

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSaleDialog dialog = new SaveSaleDialog(txtTotal.Text);
            dialog.PaymentType = luePaymentTypes.Text;

            string saleStatus = lueSaleStatus.Text;
            bool success = false;
            if (saleStatus != "EN ESPERA")
            {
                DialogResult dialogResult = dialog.ShowDialog(this);

                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    success = SaveSale(dialog);
                }
            }
            else
            {
                success = SaveSale(dialog);
            }

            
        }

        private bool SaveSale(SaveSaleDialog dialog)
        {
            Sale.DocumentTypeInfo = API.GetDocumentSale(lueDocumentTypes.Text);
            Sale.ClientID = Convert.ToInt32(sleClients.Properties.GetDisplayValueByKeyValue(sleClients.EditValue));
            Sale.SubTotal = Convert.ToDecimal(txtSubTotal.Text);
            Sale.Tax = Convert.ToDecimal(txtTax.Text);
            Sale.GrandTotal = Convert.ToDecimal(txtTotal.Text);
            Sale.SalesmanID = 1; // TODO
            Sale.LocalID = 1; // TODO
            Sale.MoneyChange = Convert.ToDecimal(dialog.MoneyChange);
            Sale.AmountPaid = Convert.ToDecimal(dialog.MoneyPayed);
            Sale.SaleDate = DateTime.Now;
            Sale.DocumentType = lueDocumentTypes.Text;
            Sale.PaymentConditionID = Convert.ToInt32(luePaymentTypes.Properties.GetKeyValueByDisplayValue(luePaymentTypes.EditValue));
            Sale.SaleStatus = lueSaleStatus.Text;
            Sale.Products = gcSelectedProducts.DataSource as DataTable;

            return Sale.SaveSale();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Sale = null;
            Sale = new Sale();
            sleClients.Text = "";
            pceSearchProduct.Text = "";
            gcSelectedProducts.DataSource = InitializeGridControlDataSource();
            txtTotal.Text = "";
            txtSubTotal.Text = "";
            txtTax.Text = "";
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {

        }       
    }
}