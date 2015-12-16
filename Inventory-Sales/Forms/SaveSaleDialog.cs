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
        public string MoneyPayed  { get; set; }
        public string MoneyChange { get; set; }

        public string PaymentType { get; set; }

        public SaveSaleDialog(string total)
        {
            InitializeComponent();
            txtDialogTotal.Text = total;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            decimal moneyPaid = Convert.ToDecimal(txtDialogMoneyPayed.Text);
            decimal total = Convert.ToDecimal(txtDialogTotal.Text);

            if (moneyPaid < total)
            {
                MessageBox.Show("No se permite que el 'Importe' sea menor que 'Total a Pagar'", "Accion No Permitida", MessageBoxButtons.OK);
            }else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.SaleGrandTotal = txtDialogTotal.Text;
                this.MoneyPayed = txtDialogMoneyPayed.Text;
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
            decimal moneyPaid = Convert.ToDecimal(txtDialogMoneyPayed.Text);

            txtDialogMoneyChange.Text = moneyPaid - total;
        }
    }
}