using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

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
    

        public string getNetwork(string str_html)
        {
            string str_imageOne2Call = "images/logo_aisS.gif";
            string str_imageDTAC = "images/logo_dtacS.gif";
            string str_imageTRUE_MOVE = "images/logo_truemoveS.gif";

            if (str_html.IndexOf(str_imageOne2Call) > 0)
            {
                return "One 2 Call";
            }
            else if (str_html.IndexOf(str_imageDTAC) > 0)
            {
                return "DTAC";
            }
            else if (str_html.IndexOf(str_imageTRUE_MOVE) > 0)
            {
                return "TrueMove";
            }
            else
            {
                return "null";
            }
        }

        public string getPathImages(string str_network)
        {
            switch (str_network)
            {
                case "One 2 Call": return "Files\\Images\\One2Call.jpg";
                case "DTAC": return "Files\\Images\\DTAC.jpg";
                case "TrueMove": return "Files\\Images\\TrueMove.png";
                default:
                    return "Files\\Images\\Error.png";
            }
        }

        public string getPathIconImages(string str_network)
        {
            switch (str_network)
            {
                case "One 2 Call": return "Files\\Images\\icoOne2Call.png";
                case "DTAC": return "Files\\Images\\icoDTAC.png";
                case "TrueMove": return "Files\\Images\\icoTrueMove.png";
                default:
                    return "Files\\Images\\ErrorIcon.png";
            }
        }

        public string stringConvertPhoneNumber(string phoneNumber)
        {
            string newStrPhoneNumber = phoneNumber;
            try
            {
                phoneNumber = phoneNumber.Replace("-", "");
                newStrPhoneNumber = phoneNumber[0] + "" + phoneNumber[1] + "" + phoneNumber[2] +
                    "-" + phoneNumber[3] + "" + phoneNumber[4] + "" + phoneNumber[5] +
                    "-" + phoneNumber[6] + "" + phoneNumber[7] + "" + phoneNumber[8] +
                    "" + phoneNumber[9];
            }
            catch
            {
            }
            return newStrPhoneNumber;
        }


        public TextEdit setTextboxPhoneNumber(TextEdit textbox)
        {
            textbox.Properties.Mask.EditMask = "((\\+\\d|10)?\\(\\d{3}\\))?\\d{3}-\\d{3}-\\d{4}";
            textbox.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            textbox.Properties.Mask.IgnoreMaskBlank = true;
            textbox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            return textbox;
        }
        
    }
}
