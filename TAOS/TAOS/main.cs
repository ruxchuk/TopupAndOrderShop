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
using System.Diagnostics;


namespace TAOS
{
    public partial class main : XtraForm
    {
        public main()
        {
            InitializeComponent();

        }

        private string str_formName = "ร้านแดงตาก้อง"; 
        private Helper helper;
        private string saveTextBarcode = "";
        private ConMySql ConnectMySql;

        private string idSelect = "";
        private double SumPriceAll = 0;

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

        private void tbxSerialAndAddPrice_KeyUp(object sender, KeyEventArgs e)
        {
            string newTextBarcode = "";
            string priceadd = tbxSerialAndAddPrice.Text.Trim();
            bool checkBarcodeInput = false;
            bool checkPriceInput = false;

            if (helper.checkTypeInputKey(e.KeyData))
            {
                saveTextBarcode += (e.KeyValue - 48).ToString();
            }
            else if (helper.checkTypeInputKeyPad(e.KeyData))
            {
                saveTextBarcode += (e.KeyValue - 96).ToString();
            }
            else if (e.KeyData == Keys.Return)
            {
                tbxSerialAndAddPrice.Text = saveTextBarcode.Trim();
                newTextBarcode = saveTextBarcode.Trim();
                priceadd = saveTextBarcode;
                saveTextBarcode = "";
                if (priceadd.Length > 6)
                {
                    checkBarcodeInput = true;
                }
                else
                {
                    checkPriceInput = true;
                }
            }

            if (e.KeyData == Keys.Add)
            {
                saveTextBarcode = "";
                tbxSerialAndAddPrice.Modified = false;
                bool checkVal1 = false;
                string serial = tbxSerialAndAddPrice.Text;
                for (int i = 0; i < serial.Length; i++)
                {
                    checkVal1 = false;
                    for (int j = 0; j < 10; j++)
                    {
                        if (serial[i] == '+' || serial[i] + "" == j.ToString() || serial[i] == '.')
                        {
                            checkVal1 = true;
                        }
                    }
                    if (!checkVal1)
                    {
                        return;
                    }
                }

                if (checkVal1)
                {
                    int rowcount = dataGridViewBuyProduct.RowCount + 1;
                    priceadd = priceadd.Replace("+", "");
                    if (priceadd == "")
                    {
                        tbxSerialAndAddPrice.Clear();
                        tbxSerialAndAddPrice.Modified = true;
                        return;
                    }
                    else
                    {
                        try
                        {
                            dataGridViewBuyProduct.Rows.Add(helper.addRowDataGrid(
                                rowcount.ToString(),
                                "- - -", "สินค้า อื่นๆ",
                                helper.ConvertToNumberformat(double.Parse(priceadd)) + " บาท",
                                "1 ชิ้น"
                            ));
                            tbxSerialAndAddPrice.Clear();
                        }
                        catch
                        {
                            tbxSerialAndAddPrice.Clear();
                            return;
                        }
                    }
                }
                readDataGridSum();
                dataGridViewBuyProduct.Rows[dataGridViewBuyProduct.Rows.Count - 1].Selected = true;
                tbxSerialAndAddPrice.Modified = true;
            }
            else if (e.KeyData == Keys.Return)
            {
                if (priceadd == "")
                {
                    return;
                }

                if (checkPriceInput)
                {
                    priceadd = priceadd.Replace("+", "");
                    calCanged(double.Parse(priceadd));
                    tbxSerialAndAddPrice.Clear();

                }
                else if (checkBarcodeInput)
                {
                    List<string>[] list = ConnectMySql.getProduct(priceadd);
                    if (list[0].Count == 0)
                    {
                        tbxSerialAndAddPrice.Clear();
                        tbxSerialAddProduct.Text = priceadd;

                        tabControlProduct.SelectedTab = tabPageShowProduct;
                        tbxNameAddProduct.Focus();
                        selectProduct(priceadd);

                        //เคลียร์ค่าหน้า add product
                        tbxNameAddProduct.Clear();
                        tbxPriceAddProduct.Text = "0";
                        cmbValueProduct.Text = "";
                        return;
                    }
                    int rowcount = dataGridViewBuyProduct.RowCount + 1;
                    try
                    {
                        dataGridViewBuyProduct.Rows.Add(helper.addRowDataGrid(
                            rowcount.ToString(), //ลำดับ
                            list[4][0], //barcode
                            list[1][0], //ชื่อสินค้า
                            helper.ConvertToNumberformat(double.Parse(list[2][0])) + " บาท", //จำนวนเงิน
                            "1 ชิ้น" //จำนวนสินค้า
                        ));
                        tbxSerialAndAddPrice.Clear();
                    }
                    catch
                    {
                        MessageBox.Show("error");
                        tbxSerialAndAddPrice.Clear();
                        return;
                    }
                }
                readDataGridSum();
            }
            else if (e.KeyData == Keys.Up)
            {
                int cellselect = int.Parse(dataGridViewBuyProduct.SelectedRows[0].Cells[0].Value.ToString());
                if (cellselect - 1 > 0)
                {
                    dataGridViewBuyProduct.Rows[cellselect - 2].Selected = true;
                }
            }
            else if (e.KeyData == Keys.Down)
            {
                int cellselect = int.Parse(dataGridViewBuyProduct.SelectedRows[0].Cells[0].Value.ToString());
                if (cellselect < dataGridViewBuyProduct.Rows.Count)
                {
                    dataGridViewBuyProduct.Rows[cellselect].Selected = true;
                }

            }
            else if (e.KeyData == Keys.Delete)
            {
                int countorder = dataGridViewBuyProduct.Rows.Count;
                if (tbxSerialAndAddPrice.Text == "" && countorder > 0)
                {
                    int cellselect = int.Parse(dataGridViewBuyProduct.SelectedRows[0].Cells[0].Value.ToString());
                    dataGridViewBuyProduct.Rows.RemoveAt(dataGridViewBuyProduct.SelectedRows[0].Index);
                    countorder--;
                    for (int i = 0; i < countorder; i++)
                    {
                        dataGridViewBuyProduct.Rows[i].Cells[0].Value = (i + 1).ToString();
                    }
                    if (countorder > 0)
                    {
                        if (cellselect - 1 == countorder)
                        {
                            dataGridViewBuyProduct.Rows[cellselect - 2].Selected = true;
                        }
                        else
                        {
                            dataGridViewBuyProduct.Rows[cellselect - 1].Selected = true;
                        }
                        readDataGridSum();
                    }
                    else
                    {
                        clearDataAll();
                    }
                }
            }
            else if (e.KeyCode == Keys.End)
            {
                if (dataGridViewBuyProduct.SelectedRows.Count == 1)
                {

                }
            }

            //ทำให้ focus จุดที่ select ใน row
            if (dataGridViewBuyProduct.Rows.Count > 0)
            {
                dataGridViewBuyProduct.FirstDisplayedScrollingRowIndex = dataGridViewBuyProduct.SelectedRows[0].Index;
            }
        }

        private void selectProduct(string barcode = "all", bool checkClick = false)
        {
            lbCountProduct.Text = "จำนวนสินค้าทั้งหมด = " + ConnectMySql.countProduct() + " รายการ";
            List<string>[] list;
            if (barcode == "all")
            {
                list = ConnectMySql.getAllProduct();
                dataGridViewProduct.Rows.Clear();
                btnAddProduct.Visible = true;
                btnUpdateProduct.Visible = false;
            }
            else
            {
                list = ConnectMySql.getProduct(barcode);
                if (!checkClick)
                {
                    dataGridViewProduct.Rows.Clear();
                }

                if (list[0].Count == 0)
                {
                    return;
                }
                tbxNameAddProduct.Text = list[1][0];
                tbxPriceAddProduct.Text = list[2][0];
                tbxSerialAddProduct.Text = list[4][0];
                cmbValueProduct.Text = list[5][0];
                btnAddProduct.Visible = false;
                btnUpdateProduct.Visible = true;
                tbxPriceAddProduct.Focus();
                if (checkClick)
                {
                    return;
                }
            }
            getDataProduct(list);
            if (dataGridViewProduct.SelectedRows.Count == 1)
            {
                idSelect = dataGridViewProduct.SelectedRows[0].Cells[6].Value.ToString();
            }
        }

        private void getDataProduct(List<string>[] list)
        {
            for (int i = 0; i < list[0].Count; i++)
            {
                int number = dataGridViewProduct.Rows.Add();
                dataGridViewProduct.Rows[number].Cells[0].Value = (number + 1).ToString();// ลำดับ
                dataGridViewProduct.Rows[number].Cells[1].Value = list[4][i];//barcode
                dataGridViewProduct.Rows[number].Cells[2].Value = list[1][i];//name
                dataGridViewProduct.Rows[number].Cells[3].Value = helper.ConvertToNumberformat(double.Parse(list[2][i])) + " บาท";//price
                dataGridViewProduct.Rows[number].Cells[4].Value = list[5][i];//type_value
                dataGridViewProduct.Rows[number].Cells[5].Value = list[3][i];//date_modified
                dataGridViewProduct.Rows[number].Cells[6].Value = list[0][i];//id
            }
        }

        //sum datagrid
        private void readDataGridSum()
        {
            if (dataGridViewBuyProduct.Rows.Count == 0)
            {
                return;
            }
            double sumprice = 0;
            for (int i = 0; i < dataGridViewBuyProduct.Rows.Count; i++)
            {
                string valprice = "" + dataGridViewBuyProduct.Rows[i].Cells[3].Value;
                valprice = valprice.Replace(",", "");
                valprice = valprice.Replace(" บาท", "");
                sumprice += double.Parse(valprice);
            }
            SumPriceAll = sumprice;
            lbSumPrice.Text = helper.ConvertToNumberformat(sumprice);

            // ถ้าลูกค้าจ่ายเงินมาแล้ว แต่ต้องการซื้อของเพิ่ม
            if (lbPriceAddChange.Text != "0")
            {
                calCanged(double.Parse(lbPriceAddChange.Text));
            }
        }

        //clear data all
        private void clearDataAll()
        {
            dataGridViewBuyProduct.Rows.Clear();
            tbxSerialAndAddPrice.Clear();
            lbPriceAddChange.Text = "0";
            lbPriceChange.ForeColor = Color.Lime;
            lbPriceChange.Text = "0";
            lbSumPrice.Text = "0";
            SumPriceAll = 0;
        }

        //เงินทอน
        private void calCanged(double expend)
        {
            double ans = expend - SumPriceAll;
            if (ans >= 0)
            {
                lbPriceChange.ForeColor = Color.Lime;
                lbPriceChange.Text = helper.ConvertToNumberformat(ans);
            }
            //else if (ans == 0)
            //{
            //    lbPriceChange.Text = "";
            //}
            else
            {
                lbPriceChange.ForeColor = Color.Red;
                lbPriceChange.Text = "เงินขาด " + helper.ConvertToNumberformat(ans * -1);
            }
            lbPriceAddChange.Text = helper.ConvertToNumberformat(expend);
        }


        #region เติมเงิน

        private string saveTypePhoneNumber = "";
        private List<string>[] AllPhoneNumber;
        private bool checkSearchNetwork = false;
        private string strSearchNetwork = "";
        MaskProperties properties = null;

        private string searchPhoneNumber(string text)
        {
            if (text == "")
                return "";
            char[] strToChar = text.ToCharArray();
            string strReturn = "";
            if (strToChar[0] == '0')
            {
                int i = 0;
                foreach (char ch in strToChar)
                {
                    strReturn += ch;
                    i++;
                    if (i == 3 || i == 6)
                    {
                        strReturn += "-";
                    }
                }
                return strReturn;
            }
            else
            {
                return text;
            }
        }

        private void getListPhoneNumber(bool checkGetData = false, string strCheck = "")
        {
            if (checkGetData)
            {
                AllPhoneNumber = ConnectMySql.getAllPhoneNumber();
            }
            if (AllPhoneNumber == null)
                return;
            listBoxTopUpPhoneNumber.Items.Clear();

            if (strCheck != "")
            {
                string str = "abcdefghijk";
                int idx;


                idx = str.IndexOf("def");
                Debug.WriteLine(idx);
                for (int i = 0; i < AllPhoneNumber[0].Count; i++)
                {
                    string phoneNumber = AllPhoneNumber[1][i];
                    int index = AllPhoneNumber[1][i].IndexOf(strCheck);
                    if (index > -1)
                    {
                        listBoxTopUpPhoneNumber.Items.Add(phoneNumber);
                        if (strCheck.Length == 10)
                        {
                            switch (AllPhoneNumber[2][i])
                            {
                                case "One 2 Call": cmbTopUpNetwork.SelectedIndex = 0; break;
                                case "DTAC": cmbTopUpNetwork.SelectedIndex = 1; break;
                                case "TrueMove": cmbTopUpNetwork.SelectedIndex = 2; break;
                                default: cmbTopUpNetwork.SelectedIndex = -1; wbsSearchNetwork(strCheck); break;
                            }
                            txtValueBaht.Focus();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < AllPhoneNumber[0].Count; i++)
                {
                    string phoneNumber = AllPhoneNumber[1][i];
                    listBoxTopUpPhoneNumber.Items.Add(phoneNumber);
                }
            }

            if (strCheck != "" && strCheck.Length < 10)
            {
                listBoxTopUpPhoneNumber.Visible = true;
            }
            else
            {
                listBoxTopUpPhoneNumber.Visible = false;
            }
            listBoxTopUpPhoneNumber.Size = new Size(192, 200);
        }

        private void wbsSearchNetwork(string phoneNumber)//other function
        {
            try
            {
                webBrowserTopUpCheckBer.Document.GetElementById("q").SetAttribute("value", phoneNumber);
                webBrowserTopUpCheckBer.Document.GetElementById("go").InvokeMember("click");
                checkSearchNetwork = true;
            }
            catch
            {

            }
        }

        private void txtTopupPhoneNumber_EditValueChanged(object sender, EventArgs e)
        {
            lbTopUpPhoneNumber.Text = txtTopupPhoneNumber.Text;

            string phonenumber = txtTopupPhoneNumber.Text.Replace("_", "").Replace("-", "");
            if (phonenumber.Length > 2)
            {
                getListPhoneNumber(false, phonenumber);
            }
            else if (phonenumber.Length == 0)
            {
                listBoxTopUpPhoneNumber.Visible = false;
            }
        }


        private void pictureBox3_Click(object sender, EventArgs e)
        {
            txtTopupPhoneNumber.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            txtValueBaht.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            cmbTopUpNetwork.Focus();
        }
        #endregion

    }
}