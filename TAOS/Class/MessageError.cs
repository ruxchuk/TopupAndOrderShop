using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TAOS
{
    class MessageError
    {
        public bool showMessageBox(string message = "ข้อมูลผิดพลาด", MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBox.Show(message, "กรุณาตรวจสอบข้อมูล", MessageBoxButtons.OK, icon);
            return false;
        }
    }
}
