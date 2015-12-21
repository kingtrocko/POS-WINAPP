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
    public partial class SaveSaleDialog : DevExpress.XtraEditors.XtraForm
    {
        public string SaleGrandTotal { get; set; }
        public string MoneyPaid  { get; set; }
        public string MoneyChange { get; set; }

        public string PaymentType { get; set; }

        public SaveSaleDialog(string total, string paymentType)
        {
            InitializeComponent();
            txtDialogTotal.Text = total;
            txtDialogMoneyChange.Text = "";
            txtDialogMoneyPaid.Focus();
            Size size = new Size();

            if (paymentType.ToLower().Equals("contado"))
            {
                txtDialogMoneyPaid.Visible = true;
                txtDialogMoneyChange.Visible = true;
                size.Width = 460;
                size.Height = 340;
                
            }
            else
            {
                size.Width = 460;
                size.Height = 210;

                txtDialogMoneyPaid.Visible = false;
                txtDialogMoneyChange.Visible = false;
                btnSave.Location = new Point(55,113);
                btnCancel.Location = new Point(187, 113);
            }

            this.Size = size;
            this.MinimumSize = size;
            this.MaximumSize = size;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            decimal moneyPaid = Convert.ToDecimal(txtDialogMoneyPaid.Text);
            decimal total = Convert.ToDecimal(txtDialogTotal.Text);

            if (moneyPaid < total)
            {
                XtraMessageBox.Show("No se permite que el 'Importe' sea menor que 'Total a Pagar'", "Error", MessageBoxButtons.OK);
                txtDialogMoneyPaid.Focus();
            }else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.SaleGrandTotal = txtDialogTotal.Text;
                this.MoneyPaid = txtDialogMoneyPaid.Text;
                this.MoneyChange = txtDialogMoneyChange.Text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void txtDialogMoneyPayed_EditValueChanged(object sender, EventArgs e)
        {
            decimal total = Convert.ToDecimal(txtDialogTotal.Text);
            decimal moneyPaid = Convert.ToDecimal(txtDialogMoneyPaid.Text);

            txtDialogMoneyChange.Text = (moneyPaid - total).ToString();
        }
    }
}