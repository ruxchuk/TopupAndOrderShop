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

        public AddBehindhand(string topupID, string customerID)
        {
            InitializeComponent();
            fTopupID = topupID;
            fCustomerID = customerID;
            Debug.WriteLine(fTopupID);
        }
    }
}