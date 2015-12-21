namespace Inventory_Sales.Forms
{
    partial class OpenSale
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gcAllSales = new DevExpress.XtraGrid.GridControl();
            this.gvAllSales = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcAllSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAllSales)).BeginInit();
            this.SuspendLayout();
            // 
            // gcAllSales
            // 
            this.gcAllSales.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcAllSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcAllSales.Location = new System.Drawing.Point(0, 0);
            this.gcAllSales.MainView = this.gvAllSales;
            this.gcAllSales.Name = "gcAllSales";
            this.gcAllSales.Size = new System.Drawing.Size(637, 308);
            this.gcAllSales.TabIndex = 0;
            this.gcAllSales.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAllSales});
            // 
            // gvAllSales
            // 
            this.gvAllSales.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn8,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7});
            this.gvAllSales.GridControl = this.gcAllSales;
            this.gvAllSales.Name = "gvAllSales";
            this.gvAllSales.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvAllSales.OptionsBehavior.Editable = false;
            this.gvAllSales.OptionsBehavior.ReadOnly = true;
            this.gvAllSales.OptionsCustomization.AllowColumnMoving = false;
            this.gvAllSales.OptionsCustomization.AllowColumnResizing = false;
            this.gvAllSales.OptionsCustomization.AllowFilter = false;
            this.gvAllSales.OptionsCustomization.AllowGroup = false;
            this.gvAllSales.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvAllSales.OptionsCustomization.AllowSort = false;
            this.gvAllSales.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvAllSales.OptionsView.ShowGroupPanel = false;
            this.gvAllSales.DoubleClick += new System.EventHandler(this.gvAllSales_DoubleClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Venta ID";
            this.gridColumn1.FieldName = "venta_id";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Documento Serie";
            this.gridColumn2.FieldName = "documento_Serie";
            this.gridColumn2.Name = "gridColumn2";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Documento Numero";
            this.gridColumn3.FieldName = "documento_Numero";
            this.gridColumn3.Name = "gridColumn3";
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Numero de Venta";
            this.gridColumn8.FieldName = "numero_venta";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.UnboundExpression = "[documento_Serie] + \'-\' + [documento_Numero]";
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 0;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Cliente";
            this.gridColumn4.FieldName = "razon_social";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Vendedor";
            this.gridColumn5.FieldName = "nombre";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Fecha";
            this.gridColumn6.DisplayFormat.FormatString = "d";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn6.FieldName = "fecha";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 2;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Total";
            this.gridColumn7.DisplayFormat.FormatString = "c";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "total";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 4;
            // 
            // OpenSale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 308);
            this.Controls.Add(this.gcAllSales);
            this.Name = "OpenSale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ventas en Espera";
            ((System.ComponentModel.ISupportInitialize)(this.gcAllSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAllSales)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcAllSales;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAllSales;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
    }
}