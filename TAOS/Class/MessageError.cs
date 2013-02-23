using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TAOS
{
    class MessageError
    {
        public DialogResult showMessageBox(string message = "ข้อมูลผิดพลาด", MessageBoxButtons button = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            return MessageBox.Show(message, "กรุณาตรวจสอบข้อมูล", button, icon);
        }
    }
}
