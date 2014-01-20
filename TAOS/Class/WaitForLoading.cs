using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Utils;

namespace TAOS
{
    class WaitForLoading
    {
        WaitDialogForm dlg;
        public bool showLoading(string message)
        {
            try
            {
                dlg = new DevExpress.Utils.WaitDialogForm(message, "โปรแกรมกำลังทำงาน");
                dlg.Show();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool cloadLoading()
        {
            try
            {
                dlg.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
