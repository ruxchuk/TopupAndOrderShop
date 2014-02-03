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
    public partial class FRMAddCredit : DevExpress.XtraEditors.XtraForm
    {
        private string fTopupID;
        private string fCustomerID;
        private string fCustomerName;
        private string fAmount;
        private string fDateTopup;
        private ConMySql connectDB;
        private MessageError messageError;

        public FRMAddCredit(string topupID, string customerID, string amount, string dateTopup, string customerName)
        {
            InitializeComponent();
            fTopupID = topupID;
            fCustomerID = customerID;
            fCustomerName = customerName;
            fAmount = amount;
            fDateTopup = dateTopup;

            

            txtValueBaht.Properties.Mask.EditMask = "d";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = false;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtValueBaht.Properties.MaxLength = 4;

            Debug.WriteLine(fTopupID);
            connectDB = new ConMySql();
            messageError = new MessageError();
            txtValueBaht.Text = fAmount;
            if (fCustomerName != "ไม่มีชื่อ")
            {
                txtCustomerName.Text = fCustomerName;
                //txtValueBaht.Select();
            }
            //else
            //{
                txtCustomerName.Select();
            //}
        }

        private void btnAddBehindhand_Click(object sender, EventArgs e)
        {
            if (!connectDB.addBehindhand(fTopupID, txtCustomerName.Text, fCustomerID, txtValueBaht.Text))
            {
                messageError.showMessageBox("การเพิ่มข้อมูลผิดพลาด");
            }
            else
            {
                MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว", "บันทึกสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        private void txtCustomerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {

                txtValueBaht.Select();
                //if (txtCustomerName.Text != "" && txtValueBaht.Text != "")
                //{
                //    btnAddBehindhand_Click(EventArgs.Empty, null);
                //}
                //else if (txtCustomerName.Text == "")
                //{
                //    txtCustomerName.Select();
                //}
                //else if (txtValueBaht.Text == "")
                //{
                //    txtValueBaht.Select();
                //}
            }
        }

        private void txtValueBaht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (txtValueBaht.Text != "" && txtCustomerName.Text != "")
                {
                    btnAddBehindhand_Click(EventArgs.Empty, null);
                }
                else if (txtCustomerName.Text == "")
                {
                    txtCustomerName.Select();
                }
                else if (txtValueBaht.Text == "")
                {
                    txtValueBaht.Select();
                }
            }
        }

    }
}