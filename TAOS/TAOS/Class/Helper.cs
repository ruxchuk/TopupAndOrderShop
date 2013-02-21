using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TAOS
{
    class Helper
    {
        //เช็คการป้อนข้อมูลในช่อง barcode
        public bool checkTypeInputKey(Keys input)
        {
            switch (input){
                case Keys.D0: return true;
                case Keys.D1: return true;
                case Keys.D2: return true;
                case Keys.D3: return true;
                case Keys.D4: return true;
                case Keys.D5: return true;
                case Keys.D6: return true;
                case Keys.D7: return true;
                case Keys.D8: return true;
                case Keys.D9: return true;
            }
            return false;
        }

        //เช็คการป้อนข้อมูลในช่อง barcode
        public bool checkTypeInputKeyPad(Keys input)
        {
            switch (input)
            {
                case Keys.NumPad0: return true;
                case Keys.NumPad1: return true;
                case Keys.NumPad2: return true;
                case Keys.NumPad3: return true;
                case Keys.NumPad4: return true;
                case Keys.NumPad5: return true;
                case Keys.NumPad6: return true;
                case Keys.NumPad7: return true;
                case Keys.NumPad8: return true;
                case Keys.NumPad9: return true;
            }
            return false;
        }

        //add row
        public string[] addRowDataGrid(string rowCount, string barcode, string nameProduct, string valueBath, string amount)
        {
            string[] row =
                new string[] 
                { 
                    rowCount, 
                    nameProduct,
                    amount,
                    valueBath,
                    barcode
                };
            return row;
        }

        // แปลง double เป็น numberformat เช่า 1,000.00
        public string ConvertToNumberformat(double val)
        {
            if (val < 10)
            {
                return String.Format("{0:0.00}", val);
            }
            else
            {
                return String.Format("{0:0,0.00}", val);
            }
        }
    }
}
