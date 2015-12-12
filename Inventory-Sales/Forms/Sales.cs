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

namespace Inventory_Sales.Forms
{
    public partial class Sales : DevExpress.XtraEditors.XtraForm
    {
        DataTable dtInvoiceProducts;
        InventoryAPI API;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public Sales()
        {
            InitializeComponent();
            API = new InventoryAPI();

            DataTable dt = API.GetAllProducts("1");
            DataTable ds1 = API.GetPricesTypes();
            DataTable pricesPerProduct = API.GetPricesPerProduct("1", "1");

            gcAllProducts.DataSource = dt;
            gcSelectedProducts.DataSource = InitializeGridControlDataSource();




            sleProducts.Properties.DataSource = dt;
            sleProducts.Properties.DisplayMember = "producto_nombre";
            sleProducts.Properties.ValueMember = "producto_id";
        }

        private DataTable InitializeGridControlDataSource()
        {
            dtInvoiceProducts = new DataTable();

            dtInvoiceProducts.Columns.Add("producto_id", typeof(int));
            dtInvoiceProducts.Columns.Add("item_number", typeof(int));
            dtInvoiceProducts.Columns.Add("producto_nombre", typeof(string));
            dtInvoiceProducts.Columns.Add("unidad_medida", typeof(string));
            dtInvoiceProducts.Columns.Add("cantidad", typeof(int));
            dtInvoiceProducts.Columns.Add("precio", typeof(decimal));
            return dtInvoiceProducts;
        }

        private void sleProducts_EditValueChanged(object sender, EventArgs e)
        {
            DataRow row = dtInvoiceProducts.NewRow();
            row["producto_id"] = sleProductsView.GetFocusedRowCellValue("producto_id");
            row["item_number"] = gvProducts.RowCount + 1;
            row["producto_nombre"] = sleProductsView.GetFocusedRowCellValue("producto_nombre");
            row["unidad_medida"] = "CAJA";
            row["cantidad"] = 1;
            row["precio"] = sleProductsView.GetFocusedRowCellValue("producto_costo_unitario");
            
            dtInvoiceProducts.Rows.Add(row);
            
            gcSelectedProducts.DataSource = dtInvoiceProducts;
            bindingSource1.DataSource = dtInvoiceProducts;


            gcSelectedProducts.Focus();
            gvProducts.FocusedRowHandle = gvProducts.RowCount - 1;
            gvProducts.FocusedColumn = gvProducts.VisibleColumns[3];
            gvProducts.ShowEditor();

        }

        private void gvProducts_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            txtSubTotal.Text = getSubtotal().ToString();
            
        }

        private void gvProducts_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            txtSubTotal.Text = getSubtotal().ToString();
        }

        private Decimal getSubtotal()
        {
            decimal subtotal = 0.0M;
            for (int i = 0; i < gvProducts.RowCount; i++)
                subtotal += Convert.ToDecimal(gvProducts.GetRowCellValue(i, "Total"));

            return subtotal;
        }

        private void gvAllProducts_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvAllProducts_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {

        }

        private void gvAllProducts_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {

        }

        private void gvAllProducts_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {

        }

       
    }
}