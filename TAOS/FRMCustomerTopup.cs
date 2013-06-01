using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Diagnostics;

namespace TAOS
{
    public partial class FRMCustomerTopup : DevExpress.XtraEditors.XtraForm
    {
        private MainForm mainForm;

        public FRMCustomerTopup(MainForm mainForm)
        {
            // TODO: Complete member initialization
            this.mainForm = mainForm;
            InitializeComponent();

            txtValueBaht.Properties.Mask.EditMask = "d";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = false;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
        }

        private void txtValueBaht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                mainForm.txtValueBaht.Text = txtValueBaht.Text;
                mainForm.btnTopUpAdd_Click(null, EventArgs.Empty);
                mainForm.tabControlMain.SelectedTab = mainForm.tabPageTopUp;
                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void FRMCustomerTopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.btnTopUpClear_Click(null, EventArgs.Empty);
        }
    }
}