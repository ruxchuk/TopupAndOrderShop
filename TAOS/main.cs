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
using System.IO;


namespace TAOS
{
    public partial class MainForm : XtraForm
    {

        private string str_formName = "ร้านแดงตาก้อง";
        private Helper helper;
        private string saveTextBarcode = "";
        private ConMySql ConnectMySql;
        private string idSelect = "";
        private double SumPriceAll = 0;
        private string str_urlCheckNetwork = "http://www.checkber.com/default.asp?q=";
        private Bitmap bmImageNetwork;
        private MessageError messageError;
        private bool checkClickTopup = false;

        public MainForm()
        {
            InitializeComponent();
            readSetting();
            helper = new Helper();
            loadSetting();
            getListPhoneNumber(true, "");
            showListWaitTopup();
        }

        private void loadSetting()
        {
            this.Text = str_formName;
            this.KeyPreview = true;

            messageError = new MessageError();

            txtTopupPhoneNumber.Properties.Mask.EditMask = "((\\+\\d|10)?\\(\\d{3}\\))?\\d{3}-\\d{3}-\\d{4}";
            txtTopupPhoneNumber.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtTopupPhoneNumber.Properties.Mask.IgnoreMaskBlank = true;
            txtTopupPhoneNumber.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;

            txtValueBaht.Properties.Mask.EditMask = "d";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = false;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
        }

        private void readSetting()
        {
            try
            {
                openFileDialog1.FileName = "Files\\FileSave.txt";
                StreamReader strm = File.OpenText(openFileDialog1.FileName);

                string user = strm.ReadLine();
                string pass = strm.ReadLine();
                string server = strm.ReadLine();
                string nameDB = strm.ReadLine();
                strm.Close();
                ConnectMySql = new ConMySql(user, pass, server, nameDB);
                if (ConnectMySql.CheckConnect())
                {
                    ConnectMySql.CloseConnection();
                }
                else
                {
                    MessageBox.Show("เชื่อมต่อฐานข้อมูล ผิดพลาด");
                }
                //Debug.WriteLine(nameDB);
            }
            catch
            {
                //SaveSetDatabase();
                MessageBox.Show("กรุณาตรวจสอบ Username หรือ Password\n", ""
                   , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void main_Load(object sender, EventArgs e)
        {
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
        private List<string>[] allPhoneNumber;
        private List<string>[] saveMathPhoneNumber;
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

        private void wbsSearchNetwork(string str_phoneNumber)//other function
        {
            string str_url = str_urlCheckNetwork + str_phoneNumber + "&go=Search";
            webBrowserTopUpCheckBer.Navigate(str_url);
        }

        private void webBrowserTopUpCheckBer_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string str_html = webBrowserTopUpCheckBer.DocumentText;
            if (txtTopupPhoneNumber.Text != "")
            {
                checkStrNetwork(helper.getNetwork(str_html));
            }
            else
            {
                txtTopupPhoneNumber.Select();
            }
        }

        private void checkStrNetwork(string str_network)
        {
            switch (str_network)
            {
                case "One 2 Call":
                    cmbTopUpNetwork.SelectedIndex = 0;
                    break;
                case "DTAC":
                    cmbTopUpNetwork.SelectedIndex = 1;
                    break;
                case "TrueMove":
                    cmbTopUpNetwork.SelectedIndex = 2;
                    break;
                default:
                    cmbTopUpNetwork.SelectedIndex = -1;
                    break;
            }
            setImageNetwork(helper.getPathImages(str_network), imgTopUpNetwork);
            setImageNetwork(helper.getPathIconImages(str_network), imageTopUpIconNetwork);
            txtValueBaht.Select();
        }

        private void setImageNetwork(string str_path, PictureBox image)
        {
            try
            {
                bmImageNetwork = new Bitmap(str_path);
                image.Image = bmImageNetwork;
            }
            catch
            {
                image.Image = null;
                MessageBox.Show("กรุณาตรวจสอบไฟล์รูปภาพเครือข่าย");
            }
        }

        private void txtTopupPhoneNumber_EditValueChanged(object sender, EventArgs e)
        {
            lbTopUpPhoneNumber.Text = txtTopupPhoneNumber.Text;
            if (!checkClickTopup)
            { 
                return;
            }

            string phonenumber = txtTopupPhoneNumber.Text.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
            if (phonenumber.Length > 2)
            {
                getListPhoneNumber(false, phonenumber);
            }
            else if (phonenumber.Length == 0)
            {
                listBoxTopUpPhoneNumber.Visible = false;
                saveMathPhoneNumber = null;
            }
        }


        private void getListPhoneNumber(bool checkGetData = false, string strCheck = "")
        {
            if (checkGetData)
            {
                allPhoneNumber = ConnectMySql.getAllPhoneNumber();
            }
            if (allPhoneNumber == null)
                return;
            listBoxTopUpPhoneNumber.Items.Clear();


            if (strCheck != "" && strCheck.Length < 10)
            {
                listBoxTopUpPhoneNumber.Visible = true;
            }
            else
            {
                listBoxTopUpPhoneNumber.Visible = false;
            }
            listBoxTopUpPhoneNumber.Size = new Size(210, 200);

            bool checkMathPhoneNumber = false;
            int i = 0;
            while (i < allPhoneNumber[1].Count)
            {
                string phoneNumber = allPhoneNumber[1][i].Trim();
                bool contrain = phoneNumber.IndexOf(strCheck.Trim()) > -1;
                if (contrain)
                {
                    listBoxTopUpPhoneNumber.Items.Add(phoneNumber);
                    if (strCheck.Length == 10)
                    {
                        ConnectMySql.phoneNumberID = int.Parse(allPhoneNumber[0][i]);
                        ConnectMySql.customerID = int.Parse(allPhoneNumber[3][i]);
                        string strNetwork = allPhoneNumber[2][i].Trim();
                        checkStrNetwork(strNetwork);
                        Debug.WriteLine(allPhoneNumber[2][i] + "|true");
                        txtValueBaht.Focus();
                    }
                    checkMathPhoneNumber = true;
                }
                i++;
            }

            if (!checkMathPhoneNumber && strCheck.Length == 10)
            {
                ConnectMySql.phoneNumberID = 0;
                ConnectMySql.customerID = 0;
                wbsSearchNetwork(strCheck);
                txtValueBaht.Select();
                Debug.WriteLine(strCheck);
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

        private void cmbTopUpNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStrNetwork(cmbTopUpNetwork.Text);
        }

        private void listBoxTopUpPhoneNumber_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1)
            {
                setPhoneNumberFromList();
            }
        }

        private void txtTopupPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            checkClickTopup = true;
            if (txtTopupPhoneNumber.Text != "" && e.KeyData == Keys.Down
            && listBoxTopUpPhoneNumber.Items.Count > 0)
            {
                if (listBoxTopUpPhoneNumber.SelectedIndex < listBoxTopUpPhoneNumber.Items.Count - 1)
                {
                    listBoxTopUpPhoneNumber.SelectedIndex++;
                    txtTopupPhoneNumber.Select(0, 0);
                }
            }
            else if (txtTopupPhoneNumber.Text != "" && e.KeyData == Keys.Up
            && listBoxTopUpPhoneNumber.Items.Count > 0)
            {
                if (listBoxTopUpPhoneNumber.SelectedIndex > 0)
                {
                    listBoxTopUpPhoneNumber.SelectedIndex--;
                    txtTopupPhoneNumber.Select(0, 0);
                }
            }
            else if (e.KeyData == Keys.Return && listBoxTopUpPhoneNumber.SelectedIndex > -1)
            {
                setPhoneNumberFromList();
            }
            else txtTopupPhoneNumber.Select();
        }

        private void setPhoneNumberFromList()
        {
            try
            {
                string phoneNumber = listBoxTopUpPhoneNumber.SelectedItem.ToString();
                string newStrPhoneNumber = helper.stringConvertPhoneNumber(phoneNumber);

                txtTopupPhoneNumber.Text = newStrPhoneNumber.Trim();
                listBoxTopUpPhoneNumber.Size = new Size(210, 4);
            }
            catch
            {
                Debug.WriteLine("error");
                txtTopupPhoneNumber.Select();
            }
        }

        private void txtValueBaht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return && txtValueBaht.Text != ""
               && int.Parse(txtValueBaht.Text) > 0)
            {
                if (cmbTopUpNetwork.SelectedIndex == -1)
                {
                    cmbTopUpNetwork.Select();
                }
                else
                {
                    btnTopUpAdd_Click(null, EventArgs.Empty);
                }
            }
        }

        private void cmbTopUpNetwork_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.D1 || e.KeyData == Keys.NumPad1)
            {
                cmbTopUpNetwork.SelectedIndex = 0;
            }
            else if (e.KeyData == Keys.D2 || e.KeyData == Keys.NumPad2)
            {
                cmbTopUpNetwork.SelectedIndex = 1;
            }
            else if (e.KeyData == Keys.D3 || e.KeyData == Keys.NumPad3)
            {
                cmbTopUpNetwork.SelectedIndex = 2;
            }
            else if (cmbTopUpNetwork.SelectedIndex != -1 && e.KeyData == Keys.Return)
            {
                btnTopUpAdd_Click(null, EventArgs.Empty);
            }
            else
                cmbTopUpNetwork.SelectedIndex = -1;
        }

        private void txtValueBaht_EditValueChanged(object sender, EventArgs e)
        {
            lbTopUpValue.Text = txtValueBaht.Text;
        }

        private void btnTopUpClear_Click(object sender, EventArgs e)
        {
            txtTopupPhoneNumber.Text = "";
            txtValueBaht.Text = "";
            cmbTopUpNetwork.SelectedIndex = -1;

            txtTopupPhoneNumber.Select();
        }

        private bool checkAddTopup()
        {
            if (txtTopupPhoneNumber.Text.Length < 10)
            {
                messageError.showMessageBox("กรุณากรอกตัวเลขให้ครบ 10 หลัก");
                txtTopupPhoneNumber.Select();
                return false;
            }
            else if (txtValueBaht.Text == "0" || txtValueBaht.Text == "")
            {
                messageError.showMessageBox("กรุณากรอกจำนวนเงิน");
                txtValueBaht.Select();
                return false;
            }
            else if (txtValueBaht.Text == "0" || txtValueBaht.Text == "")
            {
                messageError.showMessageBox("กรุณากรอกจำนวนเงิน");
                txtValueBaht.Select();
                return false;
            }
            return true;
        }

        private void btnTopUpAdd_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(555);
            if (checkAddTopup())
            {
                if (!ConnectMySql.addTopup(txtTopupPhoneNumber.Text.Replace("-", ""),
                    cmbTopUpNetwork.Text, txtValueBaht.Text))
                {
                    messageError.showMessageBox("การบันทึกรายการเกินการผิดพลาด");
                }
                else
                    btnTopUpClear_Click(null, EventArgs.Empty); 

                showListWaitTopup();
            }
        }

        private void showListWaitTopup()
        {
            dataGridViewTopup.Rows.Clear();
            List<string>[] list = ConnectMySql.getListWaitTopup();
            if (list[0].Count > 0)
            {
                btnTopUpAnAll.Visible = true;
            }

            for (int i = 0; i < list[0].Count; i++)
            {
                int number = dataGridViewTopup.Rows.Add(); 
                string newStrPhoneNumber = helper.stringConvertPhoneNumber(list[1][i]);
                dataGridViewTopup.Rows[number].Cells[0].Value = list[0][i];
                dataGridViewTopup.Rows[number].Cells[1].Value = newStrPhoneNumber;
                dataGridViewTopup.Rows[number].Cells[2].Value = list[2][i];
                dataGridViewTopup.Rows[number].Cells[3].Value = 
                    Image.FromFile(helper.getPathIconImages(list[3][i]));
                dataGridViewTopup.Rows[number].Cells[4].Value = list[4][i];
                dataGridViewTopup.Rows[number].Cells[5].Value = list[5][i];
                dataGridViewTopup.Rows[number].Cells[6].Value = list[6][i];
                dataGridViewTopup.Rows[number].Cells[7].Value = list[7][i];
                dataGridViewTopup.Rows[number].Cells[8].Value = list[3][i];
            }
        }


        #endregion


        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F6:
                    tabControlMain.SelectedTab = tabPageProduct;
                    tabControlProduct.SelectedTab = tabPageBuyProduct;
                    tbxSerialAndAddPrice.Select();
                    break;
                case Keys.F7:
                    tabControlMain.SelectedTab = tabPageTopUp;
                    tabControlTopUpList.SelectedTab = tabPageAddTopup;
                    txtTopupPhoneNumber.Select();
                    break;
                case Keys.F8:
                    tabControlMain.SelectedTab = tabPageCustomerList;
                    tabControlListCustomer.SelectedTab = tabPageListCustomer;
                    tabControlModifiedCustomer.SelectedTab = tabPageSeachCustomer;
                    tbxTelSeach.Select();
                    break;
                default: break;
            }
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageProduct)
            {
                tabControlProduct.SelectedTab = tabPageBuyProduct;
                tbxSerialAndAddPrice.Select();
            }
            else if (tabControlMain.SelectedTab == tabPageTopUp)
            {
                tabControlTopUpList.SelectedTab = tabPageAddTopup;
                txtTopupPhoneNumber.Select();
            }
            else if (tabControlMain.SelectedTab == tabPageCustomerList)
            {
                tabControlListCustomer.SelectedTab = tabPageListCustomer;
                tabControlModifiedCustomer.SelectedTab = tabPageSeachCustomer;
                tbxTelSeach.Select();
            }
        }

        private void dataGridViewTopup_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewTopup.SelectedRows.Count == 1)
            {
                checkClickTopup = false;
                txtTopupPhoneNumber.Text = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
                txtValueBaht.Text = dataGridViewTopup.SelectedRows[0].Cells[2].Value.ToString();
                cmbTopUpNetwork.Text = dataGridViewTopup.SelectedRows[0].Cells[8].Value.ToString();
                txtTopupPhoneNumber.Select();
            }
            listBoxTopUpPhoneNumber.Visible = false;
        }

        private void btnTopupDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string topupID = dataGridViewTopup.SelectedRows[0].Cells[0].Value.ToString();
                string phoneNumber = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
                DialogResult result = messageError.showMessageBox("ต้องการลบเบอร์ :" + phoneNumber + " ใช่หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                Debug.WriteLine(result);
                if (result == DialogResult.Yes)
                {
                    if (!ConnectMySql.setDeletedTopup(topupID))
                    {
                        messageError.showMessageBox("การลบข้อมูลผิดพลาด");
                    }
                    showListWaitTopup();
                }
                
            }
            catch
            {
                Debug.WriteLine("ไม่มีเบอร์");
            }
        }

    }
}