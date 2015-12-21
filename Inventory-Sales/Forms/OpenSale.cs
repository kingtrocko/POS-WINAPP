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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Inventory_Sales.Forms
{
    public partial class OpenSale : DevExpress.XtraEditors.XtraForm
    {
        private DataTable _saleProducts;
        private InventoryAPI API;
        public DataTable SaleProducts
        {
            get { return _saleProducts; }
        }

        public OpenSale(string local_id, string sale_status)
        {
            InitializeComponent();

            API = new InventoryAPI();

            gcAllSales.DataSource = API.GetSalesByStatus(local_id, sale_status);
        }

        private void gvAllSales_DoubleClick(object sender, EventArgs e)
        {   
            GridView gv = sender as GridView;
            Point pt = gv.GridControl.PointToClient(Control.MousePosition);
            this._saleProducts = GetRowProducts(gv, pt);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private DataTable GetRowProducts(GridView view, Point pt)
        {
            GridHitInfo info = view.CalcHitInfo(pt);
            DataTable products = new DataTable();
            if (info.InRow || info.InRowCell)
            {
                DataRowView selectedRow = (DataRowView)view.GetRow(info.RowHandle);
                int sale_id = Convert.ToInt32(selectedRow.Row["venta_id"]);
                products = API.GetSaleProducts(sale_id);
            }
            return products;
        }
    }
}