namespace Inventory_Sales.Forms
{
    partial class SaveSaleDialog
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtDialogTotal = new DevExpress.XtraEditors.TextEdit();
            this.txtDialogMoneyPayed = new DevExpress.XtraEditors.TextEdit();
            this.txtDialogMoneyChange = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogTotal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogMoneyPayed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogMoneyChange.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.labelControl1.Location = new System.Drawing.Point(27, 63);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(88, 18);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Total a Pagar";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.labelControl2.Location = new System.Drawing.Point(62, 101);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(53, 18);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Importe";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.labelControl3.Location = new System.Drawing.Point(75, 141);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(40, 18);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Vuelto";
            // 
            // txtDialogTotal
            // 
            this.txtDialogTotal.Location = new System.Drawing.Point(137, 57);
            this.txtDialogTotal.Name = "txtDialogTotal";
            this.txtDialogTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtDialogTotal.Properties.Appearance.Options.UseFont = true;
            this.txtDialogTotal.Properties.DisplayFormat.FormatString = "c";
            this.txtDialogTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDialogTotal.Properties.ReadOnly = true;
            this.txtDialogTotal.Size = new System.Drawing.Size(169, 24);
            this.txtDialogTotal.TabIndex = 3;
            // 
            // txtDialogMoneyPayed
            // 
            this.txtDialogMoneyPayed.Location = new System.Drawing.Point(137, 95);
            this.txtDialogMoneyPayed.Name = "txtDialogMoneyPayed";
            this.txtDialogMoneyPayed.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtDialogMoneyPayed.Properties.Appearance.Options.UseFont = true;
            this.txtDialogMoneyPayed.Properties.DisplayFormat.FormatString = "c";
            this.txtDialogMoneyPayed.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDialogMoneyPayed.Size = new System.Drawing.Size(169, 24);
            this.txtDialogMoneyPayed.TabIndex = 4;
            this.txtDialogMoneyPayed.EditValueChanged += new System.EventHandler(this.txtDialogMoneyPayed_EditValueChanged);
            // 
            // txtDialogMoneyChange
            // 
            this.txtDialogMoneyChange.Location = new System.Drawing.Point(137, 135);
            this.txtDialogMoneyChange.Name = "txtDialogMoneyChange";
            this.txtDialogMoneyChange.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtDialogMoneyChange.Properties.Appearance.Options.UseFont = true;
            this.txtDialogMoneyChange.Properties.DisplayFormat.FormatString = "c";
            this.txtDialogMoneyChange.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtDialogMoneyChange.Properties.ReadOnly = true;
            this.txtDialogMoneyChange.Size = new System.Drawing.Size(169, 24);
            this.txtDialogMoneyChange.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(70, 180);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(91, 32);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "GUARDAR";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(177, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "CANCELAR";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.labelControl4.Location = new System.Drawing.Point(106, 12);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(147, 23);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "Terminar Venta";
            // 
            // SaveSaleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 233);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDialogMoneyChange);
            this.Controls.Add(this.txtDialogMoneyPayed);
            this.Controls.Add(this.txtDialogTotal);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Name = "SaveSaleDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Guardar Venta";
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogTotal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogMoneyPayed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDialogMoneyChange.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtDialogTotal;
        private DevExpress.XtraEditors.TextEdit txtDialogMoneyPayed;
        private DevExpress.XtraEditors.TextEdit txtDialogMoneyChange;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelControl4;
    }
}