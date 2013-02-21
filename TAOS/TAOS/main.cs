using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;


namespace TAOS
{
    public partial class main : XtraForm
    {
        public main()
        {
            InitializeComponent();

        }

        private string str_formName = "ร้านแดงตาก้อง";

        private void main_Load(object sender, EventArgs e)
        {
            this.Text = str_formName;
            txtTopupPhoneNumber.Properties.Mask.EditMask = "((\\+\\d|10)?\\(\\d{3}\\))?\\d{3}-\\d\\d\\d-\\d\\d\\d\\d";
            txtTopupPhoneNumber.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtTopupPhoneNumber.Properties.Mask.IgnoreMaskBlank = false;
            txtTopupPhoneNumber.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;

            txtValueBaht.Properties.Mask.EditMask = "d4";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = true;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
        }

    }
}