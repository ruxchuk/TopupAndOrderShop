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
    public partial class AddBehindhand : DevExpress.XtraEditors.XtraForm
    {
        private string fTopupID;
        private string fCustomerID;
        private string fAmount;
        private string fDateTopup;
        private ConMySql connectDB;
        private MessageError messageError;

        public AddBehindhand(string topupID, string customerID, string amount, string dateTopup)
        {
            InitializeComponent();
            fTopupID = topupID;
            fCustomerID = customerID;
            fAmount = amount;
            fDateTopup = dateTopup;

            

            txtValueBaht.Properties.Mask.EditMask = "d";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = false;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;

            Debug.WriteLine(fTopupID);
            connectDB = new ConMySql();
            messageError = new MessageError();
            
        }

        private void addBehindhand()
        {
            if (!connectDB.addBehindhand(fTopupID, fCustomerID, fAmount, fDateTopup))
            {
                messageError.showMessageBox("การเพิ่มข้อมูลผิดพลาด");
            }
        }

        private void setCustomerList()
        {

        }

        private void btnAddBehindhand_Click(object sender, EventArgs e)
        {
            addBehindhand();
        }

    }
}