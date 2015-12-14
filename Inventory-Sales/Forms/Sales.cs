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
        DataTable dtInvoiceProducts;
        DataTable dtPricesTypes;
        InventoryAPI API;
        int selectedProductRowHandle = -1;

        public Sales()
        {
            InitializeComponent();
            API = new InventoryAPI();

            DataTable dtAllProducts = API.GetAllProducts("1");
            dtPricesTypes = API.GetPricesTypes();

            gcAllProducts.DataSource = dtAllProducts;
            gcSelectedProducts.DataSource = InitializeGridControlDataSource();


            //Settings fro gridviews
            gvAllProducts.BestFitColumns(true);
            gvPrecioVenta.BestFitColumns(true);
            gvPrecioDescuento.BestFitColumns(true);
            gvSelectedProducts.BestFitColumns(true);
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
           
            
            dtInvoiceProducts.Rows.Add(row);
            
            gcSelectedProducts.DataSource = dtInvoiceProducts;
            bindingSource1.DataSource = dtInvoiceProducts;


            gcSelectedProducts.Focus();
            gvSelectedProducts.FocusedRowHandle = gvSelectedProducts.RowCount - 1;
            gvSelectedProducts.FocusedColumn = gvSelectedProducts.VisibleColumns[3];
            gvSelectedProducts.ShowEditor();

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

                DataRow row = dtInvoiceProducts.NewRow();
                row["producto_id"] = selectedProductRow.Row["producto_id"];
                row["item_number"] = gvSelectedProducts.RowCount + 1;
                row["producto_nombre"] = selectedProductRow.Row["producto_nombre"];
                row["unidad_medida"] = nombre_unidad;
                row["cantidad"] = 1;
                row["precio"] = precio;

                dtInvoiceProducts.Rows.Add(row);

                gcSelectedProducts.DataSource = dtInvoiceProducts;
                pceSearchProduct.ClosePopup();
                pceSearchProduct.Text = Convert.ToString(selectedProductRow.Row["producto_nombre"]) + " - " + nombre_unidad;

                
            }
        }

        private void gvAllProducts_ColumnFilterChanged(object sender, EventArgs e)
        {

        }

       
    }
}