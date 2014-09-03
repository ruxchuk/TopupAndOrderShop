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
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Media;

namespace TAOS
{
    public partial class MainForm : XtraForm
    {

        private string str_formName = "ร้านแดงตาก้อง";
        private Helper helper;
        private string saveTextBarcode = "";
        private ConMySql ConnectMySql;
        private bool checkConDB = false;

        private string idSelect = "";
        private double SumPriceAll = 0;
        private string str_urlCheckNetwork = "http://www.checkber.com/default.asp?q=";
        private Bitmap bmImageNetwork;
        private MessageError messageError;
        private bool checkClickTopup = false;

        private WaitForLoading waitingForm = new WaitForLoading();

        #region GSM Modem

        private string strNoPort = "No Port";
        private ConnectPort connectPort = new ConnectPort();
        #endregion

        #region One 2 Call
        public SerialPort _Port1 = null;
        public string one2CallPortName = "";
        public string imeiOne2Call = "357083030372135";
        public string topupCodeOne2Call1 = "*123*0796*";
        public string topupCodeOne2Call2 = "*";
        public string topupCodeOne2Call3 = "#";

        public string returnCodeOne2Call1 = "*321*0796*";
        public string returnCodeOne2Call2 = "*";
        public string returnCodeOne2Call3 = "#";
        #endregion

        #region DTAC
        public SerialPort _Port2 = null;//DTAC
        public string dtacPortName = "";
        public string imeiDTAC = "860941002610669";//DTAC
        public string topupCodeDTAC1 = "*211*1*";// *211*1*เบอร์*8899*เงิน*1#
        public string topupCodeDTAC2 = "*8899*";
        public string topupCodeDTAC3 = "*1#";

        public string returnCodeDTAC1 = "*211*8*";// *211*8*เบอร์*8899*เงิน*1#
        public string returnCodeDTAC2 = "*8899*";
        public string returnCodeDTAC3 = "*1#";
        #endregion

        #region TRUE MOVE
        public SerialPort _Port3 = null;//TRUE MOVE
        public string trueMovePortName = "";
        public string imeiTrueMove = "";//"860941002609570";//TRUE MOVE
        public string topupCodeTrueMove1 = "*666*";// *666*เบอร์*เงิน*1590#
        public string topupCodeTrueMove2 = "*";
        public string topupCodeTrueMove3 = "*1590#";

        public string returnCodeTrueMove1 = "*321*0796*";// 
        public string returnCodeTrueMove2 = "*";
        public string returnCodeTrueMove3 = "#";
        #endregion

        public MainForm()
        {
            InitializeComponent();
            waitingForm.showLoading("กำลังเปิดโปรแกรม");

            helper = new Helper();
            loadSetting();

            ConnectMySql = new ConMySql();
            checkConDB = ConnectMySql.checkConDB;

            if (checkConDB)
            {
                getListPhoneNumber(true, "");
                getListTopup();
                selectProduct();
                getListCredit();
                getListCustomer();
                getListSMS();
            }

            KeyboardHook.CreateHook(KeyReader);
            imeiOne2Call = lbImeiOne2Call.Text;
            imeiDTAC = lbImeiDTAC.Text;
            imeiTrueMove = lbImeiTrueMove.Text;
            checkPort();
        }

        private void checkPort()
        {
            one2CallPortName = connectPort.getPortByImei(imeiOne2Call);
            textEditPortOne2Call.Text = one2CallPortName;
            Thread.Sleep(1000);

            dtacPortName = connectPort.getPortByImei(imeiDTAC);
            textEditPortDTAC.Text = dtacPortName;
            Thread.Sleep(1000);

            //trueMovePortName = connectPort.getPortByImei(imeiTrueMove);
            //textEditPortTrueMove.Text = trueMovePortName;
            //Thread.Sleep(1000);
        }

        private void loadSetting()
        {
            this.Text = str_formName;
            this.KeyPreview = true;

            messageError = new MessageError();

            setPropertiesTextbox();

            dtHistory.Value = DateTime.Now;

            //ImageList iconsList = new ImageList();
            //iconsList.TransparentColor = Color.Blue;
            //iconsList.ColorDepth = ColorDepth.Depth32Bit;
            //iconsList.ImageSize = new Size(25, 25);
            //iconsList.Images.Add(Image.FromFile(@"Files\Images\icoOne2Call.png"));
            //iconsList.Images.Add(Image.FromFile(@"Files\Images\icoDTAC.png"));
            //iconsList.Images.Add(Image.FromFile(@"Files\Images\icoTrueMove.png"));
            //tabControlSMS.ImageList = iconsList;
        }

        private void setPropertiesTextbox()
        {
            //textbox phone number
            txtTopupPhoneNumber = helper.setTextboxPhoneNumber(txtTopupPhoneNumber);
            textboxCustomerSearchPhone = helper.setTextboxPhoneNumber(textboxCustomerSearchPhone);
            textboxAddCustomerPhone = helper.setTextboxPhoneNumber(textboxAddCustomerPhone);
            tbxHistoryPhoneNumber = helper.setTextboxPhoneNumber(tbxHistoryPhoneNumber);

            //textbox number
            txtValueBaht.Properties.Mask.EditMask = "d";
            txtValueBaht.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            txtValueBaht.Properties.Mask.IgnoreMaskBlank = false;
            txtValueBaht.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtValueBaht.Properties.MaxLength = 4;

            tbxHistoryTime.Properties.Mask.EditMask = "d";
            tbxHistoryTime.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            tbxHistoryTime.Properties.Mask.IgnoreMaskBlank = false;
            tbxHistoryTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            tbxHistoryTime.Properties.MaxLength = 2;

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

        #region customer
        private void getListCustomer(string customerName = "", string phoneNumber = "", string network = "")
        {
            dataGridViewListCustomer.Rows.Clear();
            List<string>[] listCustomer = ConnectMySql.getListCustomer(customerName, phoneNumber, network);

            for (int i = 0; i < listCustomer[0].Count; i++)
            {
                int number = dataGridViewListCustomer.Rows.Add();
                string newStrPhoneNumber = helper.stringConvertPhoneNumber(listCustomer[6][i]);
                dataGridViewListCustomer.Rows[number].Cells[0].Value = i + 1;
                dataGridViewListCustomer.Rows[number].Cells[1].Value = listCustomer[2][i];
                dataGridViewListCustomer.Rows[number].Cells[2].Value = newStrPhoneNumber;
                dataGridViewListCustomer.Rows[number].Cells[3].Value =
                     Image.FromFile(helper.getPathIconImages(listCustomer[7][i]));
                dataGridViewListCustomer.Rows[number].Cells[4].Value = listCustomer[5][i];
                dataGridViewListCustomer.Rows[number].Cells[5].Value = listCustomer[0][i];//customer id
                dataGridViewListCustomer.Rows[number].Cells[6].Value = listCustomer[1][i];//phone number id
                dataGridViewListCustomer.Rows[number].Cells[7].Value = listCustomer[7][i];

                dataGridViewListCustomer.Rows[number].Cells[8].Value = listCustomer[3][i];
                dataGridViewListCustomer.Rows[number].Cells[9].Value = listCustomer[4][i];

            }
        }

        #endregion


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
                checkStrNetwork(helper.getNetwork(str_html), cmbTopUpNetwork, imgTopUpNetwork);
            setImageNetwork(helper.getPathIconImages(str_html), imageTopUpIconNetwork);
            txtValueBaht.Select();
            }
            else
            {
                txtTopupPhoneNumber.Select();
            }
        }

        private void checkStrNetwork(string str_network, System.Windows.Forms.ComboBox cmb, PictureBox pic)
        {
            switch (str_network)
            {
                case "One 2 Call":
                    cmb.SelectedIndex = 0;
                    break;
                case "DTAC":
                    cmb.SelectedIndex = 1;
                    break;
                case "TrueMove":
                    cmb.SelectedIndex = 2;
                    break;
                default:
                    cmb.SelectedIndex = -1;
                    break;
            }
            setImageNetwork(helper.getPathImages(str_network), pic);
            //setImageNetwork(helper.getPathImages(str_network), imgTopUpNetwork);
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
                MessageBox.Show("กรุณาตรวจสอบไฟล์รูปภาพเครือข่าย", "ตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void getListPhoneNumber(bool checkGetData = false, string strCheck = "")
        {
            //if (checkGetData)
            //{
                allPhoneNumber = ConnectMySql.getListPhoneNumber(strCheck);
            //}
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
                    phoneNumber = helper.convertStrToPhoneNumberFormat(phoneNumber);
                    listBoxTopUpPhoneNumber.Items.Add(phoneNumber);
                    if (strCheck.Length == 10)
                    {
                        ConnectMySql.phoneNumberID = int.Parse(allPhoneNumber[0][i]);
                        ConnectMySql.customerID = int.Parse(allPhoneNumber[3][i]);
                        string strNetwork = allPhoneNumber[2][i].Trim();
                        checkStrNetwork(strNetwork, cmbTopUpNetwork, imgTopUpNetwork);
                        setImageNetwork(helper.getPathIconImages(strNetwork), imageTopUpIconNetwork);
                      
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
            checkStrNetwork(cmbTopUpNetwork.Text, cmbTopUpNetwork, imgTopUpNetwork);
            setImageNetwork(helper.getPathIconImages(cmbTopUpNetwork.Text), imageTopUpIconNetwork);
            txtValueBaht.Select();
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
            
            if (txtTopupPhoneNumber.Text != "" && e.KeyData == Keys.Down
            && listBoxTopUpPhoneNumber.Items.Count > 0)
            {
                if (listBoxTopUpPhoneNumber.SelectedIndex < listBoxTopUpPhoneNumber.Items.Count - 1)
                {
                    listBoxTopUpPhoneNumber.SelectedIndex++;
                    txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);
                }
            }
            else if (txtTopupPhoneNumber.Text != "" && e.KeyData == Keys.Up
            && listBoxTopUpPhoneNumber.Items.Count > 0)
            {
                if (listBoxTopUpPhoneNumber.SelectedIndex > 0)
                {
                    listBoxTopUpPhoneNumber.SelectedIndex--;
                    txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);
                }
            }
            else if (e.KeyData == Keys.Return && listBoxTopUpPhoneNumber.SelectedIndex > -1)
            {
                setPhoneNumberFromList();
                calMod(txtTopupPhoneNumber.Text);
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

        public void btnTopUpClear_Click(object sender, EventArgs e)
        {
            listBoxTopUpPhoneNumber.Visible = false;
            lbSelectTopupID.Text = "";
            lbTopUpPhoneNumber.Text = "";
            txtTopupPhoneNumber.Text = "";
            txtValueBaht.Text = "";
            tbxTopupCustomerName.Text = "";
            cmbTopUpNetwork.SelectedIndex = -1;

            lbMod.Text = "-";

            txtTopupPhoneNumber.Select();
            //getListPhoneNumber(true, "");
            getListTopup();

            tbxTopupCustomerName.Visible = false;
            btnSaveCustomerName.Visible = false;
            btnTopupUSSD1.Visible = false;
            checkClickTopup = false;
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

        public void btnTopUpAdd_Click(object sender, EventArgs e)
        {
            if (checkAddTopup())
            {
                if (!ConnectMySql.addTopup(
                    txtTopupPhoneNumber.Text.Replace("-", ""),
                    cmbTopUpNetwork.Text, txtValueBaht.Text
                    ))
                {
                    messageError.showMessageBox("การบันทึกรายการเกินการผิดพลาด");
                }
                else
                    btnTopUpClear_Click(null, EventArgs.Empty); 

                getListTopup();
            }
        }

        /*แสดงรายการการเติมเงิน
         * isTopup = 0 ยังไม่เติมแล้ว
         * isTopup = 1 เติมแล้ว
        */
        private void getListTopup(int isTopup = 0, string phoneNumber = "",
            string date = "", string time = "", string network = "", string order = "")
        {
            dataGridViewTopup.Rows.Clear();
            List<string>[] list = ConnectMySql.getListTopup(isTopup, phoneNumber, date, time, network, order);


            Debug.WriteLine(list[0].Count);


            if (list[0].Count > 0)
            {
                btnTopUpAnAll.Visible = true;
            }
            else
            {
                btnTopUpAnAll.Visible = false;
            }

            for (int i = 0; i < list[0].Count; i++)
            {
                string newStrPhoneNumber = helper.stringConvertPhoneNumber(list[1][i]);
                int number = dataGridViewTopup.Rows.Add();
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
            //Debug.WriteLine(e.KeyData);
            checkClickTopup = true;
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
                    tabControlModifiedCustomer.SelectedTab = tabPageSeachCustomer;
                    Thread.Sleep(100);
                    tabControlListCustomer.SelectedTab = tabPageListCustomer;
                    Thread.Sleep(100);
                    tabControlMain.SelectedTab = tabPageCustomerList;
                    Thread.Sleep(100);
                    textboxCustomerSearchName.Select();
                    break;

                case Keys.OemMinus:
                    Debug.WriteLine(323);
                    break;
                case Keys.Subtract:
                    if (tabControlMain.SelectedTab == tabPageTopUp)
                    {
                        if (tabControlTopUpList.SelectedTab == tabPageAddTopup &&
                            dataGridViewTopup.RowCount > 0)
                        {
                            btnTopUpAnAll_Click(null, EventArgs.Empty);
                        }
                    }
                    break;
                case Keys.Multiply:
                    if (tabControlMain.SelectedTab == tabPageTopUp)
                    {
                        if (tabControlTopUpList.SelectedTab == tabPageAddTopup)
                        {
                            btnTopUpClear_Click(null, EventArgs.Empty);
                        }
                    }
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
                getListTopup();
                tabControlTopUpList.SelectedTab = tabPageAddTopup;
                txtTopupPhoneNumber.Select();
            }
            else if (tabControlMain.SelectedTab == tabPageCustomerList)
            {
                if (tabControlListCustomer.SelectedTab == tabPageListCustomer)
                {
                    if (tabControlModifiedCustomer.SelectedTab == tabPageSeachCustomer)
                    {
                        textboxCustomerSearchName.Select();
                        textboxCustomerSearchName.Select();
                    }
                    else if (tabControlModifiedCustomer.SelectedTab == tabPageAddCustomer)
                    {
                        textBoxAddCustomerName.Select();
                    }
                }
                else if (tabControlListCustomer.SelectedTab == tabPageCredit)
                {
                    if (tabControlCredit.SelectedTab == tabPageSearchCradit)
                    {
                        txtCustomerNameSearchCredit.Select();
                    }
                    else if (tabControlCredit.SelectedTab == tabPageAddCredit)
                    {
                        tbxAddCustomerNameCredit.Select();
                    }
                }
            }
        }

        private void dataGridViewListCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewListCustomer.SelectedRows.Count == 1)
            {
                string phoneNumber = dataGridViewListCustomer.SelectedRows[0].Cells[2].Value.ToString();
                if (phoneNumber != "")
                {
                    txtTopupPhoneNumber.Text = phoneNumber;
                    phoneNumber = phoneNumber.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
                    if (phoneNumber.Length > 2)
                    {
                        getListPhoneNumber(false, phoneNumber);
                    }
                    FRMCustomerTopup cutTopup = new FRMCustomerTopup(this);
                    cutTopup.ShowDialog();
                }

            }
        }

        private void dataGridViewListCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewListCustomer.SelectedRows.Count == 1)
            {
                textBoxAddCustomerName.Text = dataGridViewListCustomer.SelectedRows[0].Cells[1].Value.ToString();
                textboxAddCustomerPhone.Text = dataGridViewListCustomer.SelectedRows[0].Cells[2].Value.ToString();
                comboBoxAddCustomerNetwork.Text = dataGridViewListCustomer.SelectedRows[0].Cells[7].Value.ToString();
                richTextBoxAddCustomer.Text = dataGridViewListCustomer.SelectedRows[0].Cells[4].Value.ToString();

                lbOldPhoneNumber.Text = textboxAddCustomerPhone.Text;
                lbCustomerID.Text = dataGridViewListCustomer.SelectedRows[0].Cells[5].Value.ToString();
                lbPhoneNumberID.Text = dataGridViewListCustomer.SelectedRows[0].Cells[6].Value.ToString();
                try
                {
                    //labelAddCustomerID.Text = dataGridViewListCustomer.SelectedRows[0].Cells[5].Value.ToString();

                }
                catch
                {
                    //labelAddCustomerID.Text = "0";
                }
                //buttonAddCusotmerAdd.Visible = false;
                buttonCustomerEdit.Visible = true;
                buttonCustomerDelete.Visible = true;

                if (tabControlModifiedCustomer.SelectedTab == tabPageAddCustomer)
                {
                    textBoxAddCustomerName.Select();
                }
                else
                {
                    textboxCustomerSearchName.Select();
                }
            }
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
                    getListTopup();
                }
                txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);
            }
            catch
            {
                Debug.WriteLine("ไม่มีเบอร์");
            }
        }

        private void btnTopUpAnAll_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridViewTopup.RowCount;
            if (rowCount > 0)
            {
                Debug.WriteLine(rowCount);
                DialogResult result = messageError.showMessageBox("คุณได้ทำการเติมเงินจำนวน " +
                    rowCount + " รายการหมดแล้ว ใช่หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (!ConnectMySql.setIsTopupAll())
                    {
                        messageError.showMessageBox("การลบข้อมูลผิดพลาด");
                    }
                    else
                    {
                        getListTopup();
                        btnTopUpClear_Click(null, EventArgs.Empty);
                    }
                }
                txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);                
            }
        }

        private void dataGridViewTopup_Click(object sender, EventArgs e)
        {
            listBoxTopUpPhoneNumber.Visible = false;
            lbTopupMassage.Text = "";
            if (dataGridViewTopup.SelectedRows.Count == 1)
            {
                string network = dataGridViewTopup.SelectedRows[0].Cells[8].Value.ToString(); 
                if (tabControlTopUpList.SelectedTab == tabPageListTopup)
                {
                    //return topup 
                    btnTopupUSSD1.Visible = false;Debug.WriteLine(network);
                    if (network == "One 2 Call" && one2CallPortName != strNoPort)
                    {
                        bool result = connectPort.checkConnectPortByName(ref one2CallPortName);
                        //Thread.Sleep(300);
                        if (result)
                        {
                            Debug.WriteLine("one 2 call click");
                            btnReTopup.Visible = true;
                            lbRefTopupNo.Visible = true;
                            tbxRefReTopup.Visible = true;
                            tbxRefReTopup.Select();
                            checkClickTopup = true;
                        }
                    }
                    else if (network == "DTAC" && textEditPortDTAC.Text != strNoPort)
                    {
                        bool result = connectPort.checkConnectPortByName(ref dtacPortName);
                        //Thread.Sleep(300);
                        if (result)
                        {
                            btnReTopup.Visible = true;
                            tbxRefReTopup.Visible = false;
                            lbRefTopupNo.Visible = false;
                            checkClickTopup = true;
                        }
                    }
                    else if (network == "TrueMove" && textEditPortTrueMove.Text != strNoPort)
                    {
                        bool result = connectPort.checkConnectPortByName(ref trueMovePortName);
                        //Thread.Sleep(300);
                        if (result)
                        {
                            //tbxRefReTopup.Visible = false;
                            //lbRefTopupNo.Visible = false;
                            lbRefTopupNo.Visible = false;
                            tbxRefReTopup.Visible = false;
                            btnReTopup.Visible = false;
                            checkClickTopup = true;
                        }
                    }
                    else
                    {
                        lbRefTopupNo.Visible = false;
                        tbxRefReTopup.Visible = false;
                        btnReTopup.Visible = false;
                        checkClickTopup = false;
                    }
                    //tbxHistoryPhoneNumber.Select();
                    return;
                }
                else if (tabControlTopUpList.SelectedTab == tabPageAddTopup)
                {
                    tbxTopupCustomerName.Visible = true;
                    btnSaveCustomerName.Visible = true;
                    lbSelectTopupID.Text = dataGridViewTopup.SelectedRows[0].Cells[0].Value.ToString();
                    lbTopUpPhoneNumber.Text = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
                    txtValueBaht.Text = dataGridViewTopup.SelectedRows[0].Cells[2].Value.ToString();
                    cmbTopUpNetwork.Text = dataGridViewTopup.SelectedRows[0].Cells[8].Value.ToString();
                    txtTopupPhoneNumber.Text = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();

                    tbxTopupCustomerName.Text = dataGridViewTopup.SelectedRows[0].Cells[4].Value.ToString() == "ไม่มีชื่อ" ? 
                        "" : dataGridViewTopup.SelectedRows[0].Cells[4].Value.ToString();
                    txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);

                    calMod(txtTopupPhoneNumber.Text);
                    //txtTopupPhoneNumber.Select();

                    //topup
                    if (network == "One 2 Call" && textEditPortOne2Call.Text != strNoPort)
                    {
                        bool result = connectPort.checkConnectPortByName(ref one2CallPortName);

                        Debug.WriteLine(result); 
                        if (result)
                        {
                            btnTopupUSSD1.Visible = true;
                            checkClickTopup = true;
                        }
                            
                    }
                    else if (network == "DTAC" && textEditPortDTAC.Text != strNoPort)
                    {
                        bool result = connectPort.checkConnectPortByName(ref dtacPortName);
                        if (result)
                        {
                            btnTopupUSSD1.Visible = true;
                            checkClickTopup = true;
                        }

                    }
                    else if (network == "TrueMove" && textEditPortTrueMove.Text != strNoPort)
                    {

                        bool result = connectPort.checkConnectPortByName(ref trueMovePortName);
                        if (result)
                        {
                            btnTopupUSSD1.Visible = true;
                            checkClickTopup = true;
                        }
                    }
                    else
                    {
                        lbRefTopupNo.Visible = false;
                        tbxRefReTopup.Visible = false;
                        btnTopupUSSD1.Visible = false;
                        btnReTopup.Visible = false;
                        checkClickTopup = false;
                    }
                    //if (textEditPortOne2Call.Text != strNoPort
                    //    || textEditPortDTAC.Text != strNoPort
                    //    || textEditPortTrueMove.Text != strNoPort)
                    //{
                    //    btnTopupUSSD1.Visible = true;
                    //}
                }
                else
                {
                    lbRefTopupNo.Visible = false;
                    tbxRefReTopup.Visible = false;
                    btnTopupUSSD1.Visible = false;
                    btnReTopup.Visible = false;
                    checkClickTopup = false;
                }
            }

            txtTopupPhoneNumber.Select();
            
        }

        private void tabControlTopUpList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbRefTopupNo.Visible = false;
            tbxRefReTopup.Visible = false;
            btnTopupUSSD1.Visible = false;
            btnReTopup.Visible = false;
            lbTopupMassage.Text = "";
            if (tabControlTopUpList.SelectedTab == tabPageAddTopup)
            {
                getListTopup();
                txtTopupPhoneNumber.Select();
                tabControlSMS.Visible = false;
                dataGridViewTopup.Visible = true;
            }
            else if (tabControlTopUpList.SelectedTab == tabPageListTopup)
            {
                btnHistorySearch_Click(EventArgs.Empty, null);
                tabControlSMS.Visible = false;
                dataGridViewTopup.Visible = true;
            }
            else
            {
                tabControlSMS.Visible = true;
                dataGridViewTopup.Visible = false;
            }
        }

        private void btnTopUpAddPayment_Click(object sender, EventArgs e)
        {
            try
            {
                string topupID = dataGridViewTopup.SelectedRows[0].Cells[0].Value.ToString();
                string customerID = dataGridViewTopup.SelectedRows[0].Cells[6].Value.ToString();
                string customerName = dataGridViewTopup.SelectedRows[0].Cells[4].Value.ToString();
                string amount = dataGridViewTopup.SelectedRows[0].Cells[2].Value.ToString();
                string dateTopup = dataGridViewTopup.SelectedRows[0].Cells[5].Value.ToString();

                FRMAddCredit addBhindhand = new FRMAddCredit(topupID, customerID, amount, dateTopup, customerName);
                addBhindhand.ShowDialog();

            }
            catch
            {
                Debug.WriteLine("ไม่มีเบอร์");
            }
        }

        private void buttonSearchClear_Click(object sender, EventArgs e)
        {
            textboxCustomerSearchName.Text = "";
            textboxCustomerSearchPhone.Text = "";
            comboBoxSearchCustomerNetwork.SelectedIndex = -1;
            textboxCustomerSearchName.Select();
            getListCustomer();
        }

        private void tabControlModifiedCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlModifiedCustomer.SelectedTab == tabPageAddCustomer)
            {
                textBoxAddCustomerName.Select();
            }
            else
            {
                textboxCustomerSearchName.Select();
            }
        }

        private void buttonAddCustomerClear_Click(object sender, EventArgs e)
        {
            textBoxAddCustomerName.Text = "";
            textboxAddCustomerPhone.Text = "";
            comboBoxAddCustomerNetwork.SelectedIndex = -1;
            richTextBoxAddCustomer.Clear();
            textBoxAddCustomerName.Select();
            //labelAddCustomerID.Text = "";

            buttonAddCusotmerAdd.Visible = true;
            buttonCustomerEdit.Visible = false;
            buttonCustomerDelete.Visible = false;
        }

        private void buttonAddCusotmerAdd_Click(object sender, EventArgs e)
        {
            if (!validateAddCustomer())
            {
                return;
            }
            string phoneNumber = textboxAddCustomerPhone.Text.Replace("-", "").Trim();
            bool result = ConnectMySql.addCustomer(textBoxAddCustomerName.Text, phoneNumber,
                richTextBoxAddCustomer.Text, comboBoxAddCustomerNetwork.Text);
            if (!result)
            {
                MessageBox.Show("การเพิ่มลูกค้าเกิดข้อผิดพลาด", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                getListCustomer(textboxCustomerSearchName.Text, textboxCustomerSearchPhone.Text, comboBoxSearchCustomerNetwork.Text);
            }
        }

        private bool validateAddCustomer(bool checkUpdate = false)
        {
            if (textBoxAddCustomerName.Text == "")
            {
                MessageBox.Show("กรุณากรอกชื่อลูกค้า", "ตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAddCustomerName.Select();
                return false;
            }else if (textboxAddCustomerPhone.Text == "")
            {
                MessageBox.Show("กรุณากรอกเบอร์โทร", "ตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textboxAddCustomerPhone.Select();
                return false;
            }
            else if (ConnectMySql.getListCustomer("", textboxAddCustomerPhone.Text.Replace("-", ""), "")[0].Count > 0 && !checkUpdate)
            {
                MessageBox.Show("มีเบอร์ " + textboxAddCustomerPhone.Text + " นี้อยู่แล้ว\nกรุณาตรวจสอบ", "กรุณาตรวจสอบเบอร์",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textboxAddCustomerPhone.Select();
                return false;
            }
            else if (comboBoxAddCustomerNetwork.SelectedIndex == -1)
            {
                MessageBox.Show("กรุณาเลือกเครือข่าย", "ตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxAddCustomerNetwork.Select();
                return false;
            }
            //else if (richTextBoxAddCustomer.Text == "")
            //{
            //    MessageBox.Show("กรุณากรอกที่อยู่");
            //    richTextBoxAddCustomer.Select();
            //    return false;
            //}

            return true;
        }

        private void btnHistorySearch_Click(object sender, EventArgs e)
        {
            lbRefTopupNo.Visible = false;
            tbxRefReTopup.Visible = false;
            btnTopupUSSD1.Visible = false;
            btnReTopup.Visible = false;
            string date = dtHistory.Value.Year + "-" + dtHistory.Value.Month + "-" + dtHistory.Value.Day;
            Debug.WriteLine(date);
            getListTopup(1, tbxHistoryPhoneNumber.Text, date, tbxHistoryTime.Text, cmbHistoryNetwork.Text, " ORDER BY a.id DESC");
            tbxHistoryPhoneNumber.Select();
            //if (dataGridViewTopup.SelectedRows.Count == 1){
            //dataGridViewTopup.FirstDisplayedScrollingRowIndex = dataGridViewTopup.RowCount - 1;}
        }

        private void btnHistoryClear_Click(object sender, EventArgs e)
        {
            tbxHistoryPhoneNumber.Text = "";
            cmbHistoryNetwork.SelectedIndex = -1;
            tbxHistoryTime.Text = "";
            dtHistory.Value = DateTime.Now;
            string date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            getListTopup(1, "", date, "", "", " ORDER BY a.id DESC");
            tbxHistoryPhoneNumber.Select();

            lbRefTopupNo.Visible = false;
            tbxRefReTopup.Visible = false;
            btnTopupUSSD1.Visible = false;
            btnReTopup.Visible = false;
            tbxRefReTopup.Clear();
        }

        private void btnHistoryCancelTopup_Click(object sender, EventArgs e)
        {
            lbRefTopupNo.Visible = false;
            tbxRefReTopup.Visible = false;
            btnTopupUSSD1.Visible = false;
            btnReTopup.Visible = false;
            try
            {
                string idSelect = dataGridViewTopup.SelectedRows[0].Cells[0].Value.ToString();
                string phoneNumber = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
                DialogResult result = MessageBox.Show("คุณต้องเปลี่ยนสถานะเบอร์ " + phoneNumber + " \nเป็นยังไม่ได้เติมเงิน ใช่ หรือไม่", "เปลี่ยนสถานะ", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Debug.WriteLine(idSelect);
                    bool updateResult = ConnectMySql.changeToNoTopup(idSelect);
                    if (!updateResult)
                    {
                        MessageBox.Show("การแก้ไขผิดพลาด", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        tabControlTopUpList.SelectedTab = tabPageAddTopup;
                    }
                }
                tbxHistoryPhoneNumber.Select(tbxHistoryPhoneNumber.Text.Length, 0);
                
            }
            catch
            {
                Debug.WriteLine("No Select");
            }
        }

        private void textboxCustomerSearchName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (dataGridViewListCustomer.SelectedRows.Count == 1)
                {
                    string phoneNumber = dataGridViewListCustomer.SelectedRows[0].Cells[2].Value.ToString();
                    if (phoneNumber != "")
                    {
                        txtTopupPhoneNumber.Text = phoneNumber;
                        phoneNumber = phoneNumber.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
                        if (phoneNumber.Length > 2)
                        {
                            getListPhoneNumber(false, phoneNumber);
                        }
                        FRMCustomerTopup cutTopup = new FRMCustomerTopup(this);
                        cutTopup.ShowDialog();
                    }

                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                textboxCustomerSearchName.Text = "";
            }
        }

        private void comboBoxSearchCustomerNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStrNetwork(comboBoxSearchCustomerNetwork.Text, comboBoxSearchCustomerNetwork, pictureBoxCustomerSearch);
        }

        private void comboBoxAddCustomerNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStrNetwork(comboBoxAddCustomerNetwork.Text, comboBoxAddCustomerNetwork, pictureBoxCustomerAdd);
        }

        private void buttonCustomerEdit_Click(object sender, EventArgs e)
        {
            if (!validateAddCustomer(true))
            {
                return;
            }
            string phoneNumber = textboxAddCustomerPhone.Text.Replace("-", "").Trim();
            int result = ConnectMySql.updateCustomer(lbCustomerID.Text, lbPhoneNumberID.Text, textBoxAddCustomerName.Text, phoneNumber
                , comboBoxAddCustomerNetwork.Text, richTextBoxAddCustomer.Text);
            if (result == 0)
            {
                MessageBox.Show("การแก้ไขกค้าเกิดข้อผิดพลาด", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == -1)
            {
                MessageBox.Show("เบอร์ " + textboxAddCustomerPhone.Text + " มีอยู่ในระบบแล้วกรุณาตรวจสอบ",
                    "ตรวจสอบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textboxAddCustomerPhone.Select();
            }
            else
            {
                MessageBox.Show("ทำการบันทึกข้อมูลเรียบร้อยแล้ว", "บันทึกสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                getListCustomer(textboxCustomerSearchName.Text, textboxCustomerSearchPhone.Text, comboBoxSearchCustomerNetwork.Text);
            }
            textBoxAddCustomerName.Select();
        }

        private void buttonCustomerDelete_Click(object sender, EventArgs e)
        {
            bool result = ConnectMySql.setDeleteCustomer(lbCustomerID.Text);
            if (!result)
            {
                MessageBox.Show("การลบเกิดข้อผิดพลาด", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                buttonAddCustomerClear_Click(EventArgs.Empty, null); 
                getListCustomer(textboxCustomerSearchName.Text, textboxCustomerSearchPhone.Text, comboBoxSearchCustomerNetwork.Text);
                MessageBox.Show("ทำการลบข้อมูลเรียบร้อยแล้ว", "ลบสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxAddCustomerName.Select();
            }
        }

        private void getListCredit(
            string customerName = "",
            string phoneNumber = "",
            string price = "",
            string status = "0",
            string network = ""
            )
        {
            dataGridViewListCredit.Rows.Clear();
            List<string>[] listCredit = ConnectMySql.getListCredit(customerName, phoneNumber, price, status, network);

            for (int i = 0; i < listCredit[0].Count; i++)
            {
                int number = dataGridViewListCredit.Rows.Add();
                string newStrPhoneNumber = helper.stringConvertPhoneNumber(listCredit[6][i]);
                dataGridViewListCredit.Rows[number].Cells[0].Value = listCredit[0][i];//id
                dataGridViewListCredit.Rows[number].Cells[1].Value = listCredit[1][i];//customer name
                dataGridViewListCredit.Rows[number].Cells[2].Value = listCredit[6][i];//phonenumber
                dataGridViewListCredit.Rows[number].Cells[3].Value = listCredit[2][i];//price
                dataGridViewListCredit.Rows[number].Cells[4].Value =
                     Image.FromFile(helper.getPathIconImages(listCredit[7][i]));//network
                dataGridViewListCredit.Rows[number].Cells[5].Value = listCredit[3][i];//price
                dataGridViewListCredit.Rows[number].Cells[6].Value = listCredit[4][i];//
                dataGridViewListCredit.Rows[number].Cells[9].Value = listCredit[7][i];//

            }
            txtCustomerNameSearchCredit.Select();
        }

        private void btnSearchCredit_Click(object sender, EventArgs e)
        {
            getListCredit(txtCustomerNameSearchCredit.Text, 
                tbxPhoneSearchCredit.Text, 
                tbxSearchPriceCredit.Text,
                cmbSearchStatusCredit.SelectedIndex.ToString(), 
                cmbSearchNetworkCredit.Text);
            txtCustomerNameSearchCredit.Select();
        }

        private void btnClearSearchCredit_Click(object sender, EventArgs e)
        {
            tbxPhoneSearchCredit.Text = "";
            tbxSearchPriceCredit.Text = "";
            cmbSearchStatusCredit.SelectedIndex = 0;
            cmbSearchNetworkCredit.SelectedIndex = -1;
            getListCredit();
            txtCustomerNameSearchCredit.Select();
        }

        /*private void btnAddCredit_Click(object sender, EventArgs e)
        {
            if (tbxAddCustomerNameCredit.Text == "")
            {
                messageError.showMessageBox("กรุณาระบุชื่อลูกค้า");
                tbxAddCustomerNameCredit.Focus();
                return;
            }
            //else if (tbxAddPhoneNumberCredit.Text == "")
            //{
            //    messageError.showMessageBox("กรุณาระบุเบอร์โทร");
            //    tbxAddPhoneNumberCredit.Focus();
            //    return;
            //}
            else if (tbxAddCredit.Text == "")
            {
                messageError.showMessageBox("กรุณาระบุจำนวนเงิน");
                tbxAddCredit.Focus();
                return;
            }
            else if (cmdNetworkAddCredit.SelectedIndex < 0)
            {
                messageError.showMessageBox("กรุณาระบุเครือข่าย");
                cmdNetworkAddCredit.Focus();
                return;
            }
            if (!ConnectMySql.addBehindhand("0", "", "0", tbxAddCredit.Text))
            {
                messageError.showMessageBox("การเพิ่มข้อมูลผิดพลาด");
            }
            else
            {
                getListCredit();
            }
        }*/

        private void cmbSearchNetworkCredit_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStrNetwork(cmbSearchNetworkCredit.Text, cmbSearchNetworkCredit, pictureBoxNetworkSearchCredit);
        }

        private void cmdNetworkAddCredit_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStrNetwork(cmdNetworkAddCredit.Text, cmdNetworkAddCredit, pictureBoxNetworkAddCredit);
        }

        private void btnClearCredit_Click(object sender, EventArgs e)
        {
            tbxAddCustomerNameCredit.Text = "";
            tbxAddPhoneNumberCredit.Text = "";
            tbxAddCredit.Text = "";
            cmdNetworkAddCredit.SelectedIndex = -1;

            btnEditCredit.Visible = false;
            btnDeleteCredit.Visible = false;
            getListCredit();
            tbxAddCustomerNameCredit.Select();
        }

        private void dataGridViewListCredit_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewListCredit.SelectedRows.Count == 1)
            {
                tbxAddCustomerNameCredit.Text = dataGridViewListCredit.SelectedRows[0].Cells[1].Value.ToString();
                tbxAddPhoneNumberCredit.Text = dataGridViewListCredit.SelectedRows[0].Cells[2].Value.ToString();
                tbxAddCredit.Text = dataGridViewListCredit.SelectedRows[0].Cells[3].Value.ToString();
                cmdNetworkAddCredit.Text = dataGridViewListCredit.SelectedRows[0].Cells[9].Value.ToString();

                lbSelectCreditID.Text = dataGridViewListCredit.SelectedRows[0].Cells[0].Value.ToString();

                //lbCustomerID.Text = dataGridViewListCustomer.SelectedRows[0].Cells[5].Value.ToString();
                //lbPhoneNumberID.Text = dataGridViewListCustomer.SelectedRows[0].Cells[6].Value.ToString();

                tbxAddCredit.Select();
                btnEditCredit.Visible = true;
                btnDeleteCredit.Visible = true;
            }
        }

        private void btnEditCredit_Click(object sender, EventArgs e)
        {
            if (tbxAddCustomerNameCredit.Text == "")
            {
                messageError.showMessageBox("กรุณาระบุชื่อลูกค้า");
                tbxAddCustomerNameCredit.Focus();
                return;
            }
            else if (tbxAddCredit.Text == "")
            {
                messageError.showMessageBox("กรุณาระบุจำนวนเงิน");
                tbxAddCredit.Focus();
                return;
            }
            bool result = ConnectMySql.updateCredit(
                lbSelectCreditID.Text,
                tbxAddCustomerNameCredit.Text,
                tbxAddCredit.Text);
            if (!result)
            {
                messageError.showMessageBox("การเพิ่มข้อมูลผิดพลาด");
            }
            else
            {
                btnClearCredit_Click(null, EventArgs.Empty);
            }

        }

        private void btnDeleteCredit_Click(object sender, EventArgs e)
        {
            DialogResult dResult = messageError.showMessageBox("ต้องการลบยอดค้าง :" +
                tbxAddCredit.Text +
                " บาท\nเบอร์ :" + tbxAddPhoneNumberCredit.Text + 
                "\nใช่หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dResult == DialogResult.No)
            {
                return;
            }
            bool result = ConnectMySql.updateCredit(lbSelectCreditID.Text, "", "", "", "1");
            if (!result)
            {
                messageError.showMessageBox("การลบผิดพลาด");
            }
            else
            {
                btnClearCredit_Click(null, EventArgs.Empty);
            }
        }

        private void dataGridViewListCredit_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewListCredit.SelectedRows.Count == 1)
            {
                DialogResult dResult = messageError.showMessageBox("ลูกค้า :\"" +
                tbxAddCustomerNameCredit.Text +
                "\" ได้จ่ายเงินแล้ว ใช่หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dResult == DialogResult.No)
                {
                    return;
                }
                bool result = ConnectMySql.updateCredit(lbSelectCreditID.Text, "", "", "1");
                if (!result)
                {
                    messageError.showMessageBox("การบันทึกผิดพลาด");
                }
                else
                {
                    btnClearCredit_Click(null, EventArgs.Empty);
                }
            }
        }

        private void tabControlListCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlListCustomer.SelectedTab == tabPageListCustomer)
            {
                if (tabControlModifiedCustomer.SelectedTab == tabPageSeachCustomer) 
                {
                    textboxCustomerSearchName.Select();
                }
                else if (tabControlModifiedCustomer.SelectedTab == tabPageAddCustomer) 
                {
                    textBoxAddCustomerName.Select();
                }
            }
            else if (tabControlListCustomer.SelectedTab == tabPageCredit)
            {
                if (tabControlCredit.SelectedTab == tabPageSearchCradit)
                {
                    txtCustomerNameSearchCredit.Select();
                }
                else if (tabControlCredit.SelectedTab == tabPageAddCredit)
                {
                    tbxAddCustomerNameCredit.Select();
                }
            }
        }

        private void tabControlCredit_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControlCredit.SelectedTab == tabPageSearchCradit)
            {
                txtCustomerNameSearchCredit.Select();
            }
            else if (tabControlCredit.SelectedTab == tabPageAddCredit)
            {
                tbxAddCustomerNameCredit.Select();
            }
        }

        private void txtTopupPhoneNumber_KeyUp(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(2);
            //lbTopUpPhoneNumber.Text = txtTopupPhoneNumber.Text;
            //string phonenumber = txtTopupPhoneNumber.Text.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
            //if (phonenumber.Length > 2)
            //{
            //    getListPhoneNumber(false, phonenumber);
            //}
            //else if (phonenumber.Length == 0)
            //{
            //    listBoxTopUpPhoneNumber.Visible = false;
            //    saveMathPhoneNumber = null;
            //}

        }

        private void dataGridViewTopup_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewTopup.SelectedRows.Count == 1 &&
                tabControlTopUpList.SelectedTab == tabPageAddTopup)
            {
                DialogResult result = messageError.showMessageBox("ทำการเติมเงินเบอร์ \"" +
                    txtTopupPhoneNumber.Text + "\" แล้ว \nใช่ หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //lbSelectTopupID.Text
                if (result == DialogResult.Yes)
                {
                    if (!ConnectMySql.setIsTopup(lbSelectTopupID.Text))
                    {
                        messageError.showMessageBox("การบันทึกผิดพลาด");
                    }
                    else
                    {
                        btnTopUpClear_Click(null, EventArgs.Empty);
                    }
                }
                txtTopupPhoneNumber.Select(txtTopupPhoneNumber.Text.Length, 0);
            }
        }

        private void calMod(string number)
        {

            string phonenumber = number.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
            int sum = 0;
            sum += int.Parse(phonenumber[0].ToString());
            sum += int.Parse(phonenumber[1].ToString());
            sum += int.Parse(phonenumber[2].ToString());
            sum += int.Parse(phonenumber[3].ToString());
            sum += int.Parse(phonenumber[4].ToString());
            sum += int.Parse(phonenumber[5].ToString());
            sum += int.Parse(phonenumber[6].ToString());
            sum += int.Parse(phonenumber[7].ToString());
            sum += int.Parse(phonenumber[8].ToString());
            sum += int.Parse(phonenumber[9].ToString());
            sum = sum % 9;
            sum = sum == 0 ? 9 : sum;
            lbMod.Text = sum.ToString();
        }

        private void txtTopupPhoneNumber_EditValueChanged(object sender, EventArgs e)
        {

            lbTopUpPhoneNumber.Text = txtTopupPhoneNumber.Text;
            string phonenumber = txtTopupPhoneNumber.Text.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
            if (phonenumber.Length > 2)
            {
                getListPhoneNumber(false, phonenumber);
                if (phonenumber.Length == 10)
                {
                    calMod(phonenumber);
                } 
            }
            else if (phonenumber.Length == 0)
            {
                listBoxTopUpPhoneNumber.Visible = false;
                saveMathPhoneNumber = null;
            }

            Debug.WriteLine(phonenumber.Length);
        }


        
        private int countKeyDown = 0;
        public void KeyReader(IntPtr wParam, IntPtr lParam)
        {
            int key = Marshal.ReadInt32(lParam);
            string temp = KeyboardHook.checkMatchKey(key);
            if (temp == ".")
            {
                countKeyDown++;
                Debug.WriteLine(countKeyDown);
                if (countKeyDown > 2)
                {
                    countKeyDown = 0;
                    //this.WindowState = FormWindowState.Normal;
                    //this.WindowState = FormWindowState.Maximized;
                    //this.Activate();
                    //this.WindowState = FormWindowState.Normal;
                    //this.Show();

                    KeyboardHook.RestoreWindows(this);
                }
            }
            else countKeyDown = 0;
        }

        private void txtValueBaht_TextChanged(object sender, EventArgs e)
        {
            if (txtValueBaht.Text != "")
            {
                if (double.Parse(txtValueBaht.Text) < 0)
                {
                    //Debug.WriteLine(double.Parse(txtValueBaht.Text));
                    txtValueBaht.Text = (double.Parse(txtValueBaht.Text) * -1).ToString();
                }

            }
        }

        private void textboxCustomerSearchName_TextChanged(object sender, EventArgs e)
        {
            getListCustomer(textboxCustomerSearchName.Text, textboxCustomerSearchPhone.Text, comboBoxSearchCustomerNetwork.Text);
        }

        private void btnTopupUSSD1_Click(object sender, EventArgs e)
        {
            checkClickTopup = true;
            string phone = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
            string network = dataGridViewTopup.SelectedRows[0].Cells[8].Value.ToString();
            string valueBath = dataGridViewTopup.SelectedRows[0].Cells[2].Value.ToString();
            DialogResult result = messageError.showMessageBox("ต้องการเติมเงิน " + network + " เบอร์ \n" +
                phone + " ใช่ หรือไม่",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                waitingForm.showLoading("กำลังเติมเงิน กรุณารอสักครู่");
                phone = phone.Trim();
                phone = phone.Replace("-", "");
                string ussdCode = "";
                string response = ""; 
                switch (network)
                {
                    case "One 2 Call": ussdCode = 
                        topupCodeOne2Call1 + phone +
                        topupCodeOne2Call2 + valueBath +
                        topupCodeOne2Call3;
                        //if (!connectPort.checkConnectPort(_Port1))
                        //{
                        //    lbTopupMassage.Text = "กรุณาเช็ค Port: One 2 Call";
                        //    waitingForm.cloadLoading();
                        //    return;
                        //}
                        //Debug.Write("Send ussd start");
                        response = connectPort.topupUSSD(ussdCode, one2CallPortName, true);
                    break;
                    case "DTAC": ussdCode =
                        //"*101#";
                    topupCodeDTAC1 + phone +
                    topupCodeDTAC2 + valueBath +
                    topupCodeDTAC3; 
                    //if (!connectPort.checkConnectPort(_Port2))
                    //{
                    //    lbTopupMassage.Text = "กรุณาเช็ค Port: DTAC";
                    //    waitingForm.cloadLoading();
                    //    return;
                    //}
                    response = connectPort.topupUSSD(ussdCode, dtacPortName, true);
                    break;
                    case "TrueMove": ussdCode =
                        //"*103#";
                    topupCodeTrueMove1 + phone +
                    topupCodeTrueMove2 + valueBath +
                    topupCodeTrueMove3; 
                    //if (!connectPort.checkConnectPort(_Port3))
                    //{
                    //    lbTopupMassage.Text = "กรุณาเช็ค Port: TRUE MOVE";
                    //    waitingForm.cloadLoading();
                    //    return;
                    //}
                    response = connectPort.topupUSSD(ussdCode, trueMovePortName, true);
                    break;
                }

                if (!ConnectMySql.setIsTopup(lbSelectTopupID.Text))
                {
                    waitingForm.cloadLoading();
                    messageError.showMessageBox("การบันทึกผิดพลาด");
                }
                else
                {
                    btnTopUpClear_Click(null, EventArgs.Empty);
                }
                lbTopupMassage.Text = response;
                waitingForm.cloadLoading();
            }
            else {
                btnTopupUSSD1.Visible = false;
            }
            checkClickTopup = false;
        }

        private void btnReTopup_Click(object sender, EventArgs e)
        {
            checkClickTopup = true;
            string network = dataGridViewTopup.SelectedRows[0].Cells[8].Value.ToString();
            if (tbxRefReTopup.Text == "" && network == "One 2 Call")
            {
                tbxRefReTopup.Select();
                return;
            }
            string phone = dataGridViewTopup.SelectedRows[0].Cells[1].Value.ToString();
            DialogResult result = messageError.showMessageBox("ต้องการดึงเงินคืน เบอร์ \n" +
                    phone + " ใช่ หรือไม่",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                waitingForm.showLoading("กำลังดึงเงินคืน กรุณารอสักครู่");
                phone = phone.Trim();
                 phone = phone.Replace("-", "");
                 string subPhone = phone.Substring(6);

                string valueBath = dataGridViewTopup.SelectedRows[0].Cells[2].Value.ToString();
                string refNo = tbxRefReTopup.Text;
                string ussdCode = "";
                string response = "";
                switch (network)
                {
                    case "One 2 Call": ussdCode =
                    returnCodeOne2Call1 + subPhone +
                    returnCodeOne2Call2 + refNo +
                    returnCodeOne2Call3; 
                    response = connectPort.topupUSSD(ussdCode, one2CallPortName, true);
                    break;
                    case "DTAC": ussdCode =
                    returnCodeDTAC1 + phone +
                    returnCodeDTAC2 + valueBath +
                    returnCodeDTAC3;
                    //Debug.WriteLine(ussdCode);return;
                    response = connectPort.topupUSSD(ussdCode, dtacPortName, true);
                    break;
                    //case "TrueMove": ussdCode =
                    //topupCodeOne2Call1 + phone +
                    //topupCodeOne2Call2 + refNo +
                    //topupCodeOne2Call3; break;
                    //response = connectPort.topupUSSD(ussdCode, dtacPortName, true);
                }

                
                //MessageBox.Show(response);
                lbTopupMassage.Text = response;
                btnHistoryClear_Click(null, EventArgs.Empty);
                waitingForm.cloadLoading();
            }
            else
            {
                lbRefTopupNo.Visible = false;
                tbxRefReTopup.Visible = false;
                btnTopupUSSD1.Visible = false;
                btnReTopup.Visible = false;
            }
            checkClickTopup = false;
        }

        private void tbxRefReTopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                btnReTopup_Click(null, EventArgs.Empty);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            waitingForm.cloadLoading();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //waitingForm.showLoading("กำลังส่ง");
            //string ussdCode = "AT+GSN";
            ////string ussdCode = "AT+CGSN";
            ////string response = connectPort.topupUSSD(ussdCode, _Port1);
            //string response = connectPort.checkIMEI(_Port3, imeiTrueMove);
            //Debug.WriteLine(response);
            //MessageBox.Show(response);
            //lbTopupMassage.Text = response;
            //btnHistoryClear_Click(null, EventArgs.Empty);
            //waitingForm.cloadLoading();
        }

        private void btnConnectPort3_Click(object sender, EventArgs e)
        {
            waitingForm.showLoading("กำลังเช็ค Port ทั้งหมดกรุณารอสักครู่");
            checkClickTopup = true;
            checkPort();
            waitingForm.cloadLoading();
            checkClickTopup = false;
        }

        private void btnSaveCustomerName_Click(object sender, EventArgs e)
        {
            string phoneNumber = txtTopupPhoneNumber.Text.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
            string network = cmbTopUpNetwork.Text;
            string customerID = dataGridViewTopup.SelectedRows[0].Cells[6].Value.ToString();
            string phoneNumberID = dataGridViewTopup.SelectedRows[0].Cells[7].Value.ToString();

            textBoxAddCustomerName.Text = tbxTopupCustomerName.Text;
            textboxAddCustomerPhone.Text = phoneNumber;
            comboBoxAddCustomerNetwork.Text = network;
            //richTextBoxAddCustomer.Text = dataGridViewListCustomer.SelectedRows[0].Cells[4].Value.ToString();
            lbOldPhoneNumber.Text = phoneNumber;
            lbCustomerID.Text = customerID;
            lbPhoneNumberID.Text = phoneNumberID;
            if (customerID == "0")
            {
                buttonAddCusotmerAdd_Click(null, EventArgs.Empty);
            }
            else
            {
                buttonCustomerEdit_Click(null, EventArgs.Empty);
            }

            getListTopup();

            btnTopupUSSD1.Visible = false;
            tbxTopupCustomerName.Visible = false;
            btnSaveCustomerName.Visible = false;
        }

        private void textboxCustomerSearchPhone_TextChanged(object sender, EventArgs e)
        {
            getListCustomer(textboxCustomerSearchName.Text, textboxCustomerSearchPhone.Text, comboBoxSearchCustomerNetwork.Text);
        }

        private void textboxCustomerSearchPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (dataGridViewListCustomer.SelectedRows.Count == 1)
                {
                    string phoneNumber = dataGridViewListCustomer.SelectedRows[0].Cells[2].Value.ToString();
                    if (phoneNumber != "")
                    {
                        txtTopupPhoneNumber.Text = phoneNumber;
                        phoneNumber = phoneNumber.Replace("_", "").Replace("-", "").Replace("_", "").Trim();
                        if (phoneNumber.Length > 2)
                        {
                            getListPhoneNumber(false, phoneNumber);
                        }
                        FRMCustomerTopup cutTopup = new FRMCustomerTopup(this);
                        cutTopup.ShowDialog();
                    }

                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                textboxCustomerSearchPhone.Text = "";
            }
        }

        private void timerGetSMS_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("get sms:");
            if (!checkClickTopup)
            {
                receiveSMS();
            }
        }

        private void receiveSMS()
        {
            waitingForm.showLoading("กำลังโหลด SMS กรุณารอสักครู่");
            bool checkReceive = false;
            SerialPort port1 = null;
            SerialPort port2 = null;
            SerialPort port3 = null;
            if (one2CallPortName != strNoPort)
            {
                port1 = connectPort.OpenPort(one2CallPortName);
            }
            if (dtacPortName != strNoPort)
            {
                port2 = connectPort.OpenPort(dtacPortName);
            }
            if (trueMovePortName != strNoPort)
            {
                port3 = connectPort.OpenPort(trueMovePortName);
            }
            if (port1 != null)
            {
                ShortMessageCollection objShortMessageCollection = connectPort.ReadSMS(port1);
                if (objShortMessageCollection.Count > 0)
                {
                    checkReceive = true;
                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        string massage = connectPort.ussd.decodeResponseUSSDToText(msg.Message);
                        ConnectMySql.addReceiveSMS(
                            msg.Sender,
                            massage,
                            msg.Sent.Replace("+28", ""),
                            1);
                        if (massage.Length > 20)
                            lbTopupMassage.Text = massage;

                    }
                    connectPort.DeleteMsg(port1);
                }
                connectPort.ClosePort(port1);
            }

            if (port2 != null)
            {
                ShortMessageCollection objShortMessageCollection = connectPort.ReadSMS(port2);
                if (objShortMessageCollection.Count > 0)
                {
                    checkReceive = true;
                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        string massage = connectPort.ussd.decodeResponseUSSDToText(msg.Message);
                        ConnectMySql.addReceiveSMS(
                            msg.Sender,
                            massage,
                            msg.Sent.Replace("+28", ""),
                            2);
                        if (massage.Length > 20)
                            lbTopupMassage.Text = massage;

                    }
                    connectPort.DeleteMsg(port2);
                }
                connectPort.ClosePort(port2);
            }

            if (port3 != null)
            {
                ShortMessageCollection objShortMessageCollection = connectPort.ReadSMS(port3);
                if (objShortMessageCollection.Count > 0)
                {
                    checkReceive = true;
                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        string massage = connectPort.ussd.decodeResponseUSSDToText(msg.Message);
                        ConnectMySql.addReceiveSMS(
                            msg.Sender,
                            massage,
                            msg.Sent.Replace("+28", ""),
                            3);
                        if (massage.Length > 20)
                            lbTopupMassage.Text = massage;

                    }
                    connectPort.DeleteMsg(port3);
                }
                connectPort.ClosePort(port3);
            }
            
            if (checkReceive)
            {
                (new SoundPlayer(@"Files\Sound\sms.wav")).Play();
                getListSMS();
            }
            waitingForm.cloadLoading();
        }

        private void getListSMS()
        {

            dataGridViewSMS1.Rows.Clear();
            List<string>[] list = ConnectMySql.getListSMS(1);
            for (int i = 0; i < list[0].Count; i++)
            {
                int number = dataGridViewSMS1.Rows.Add();
                dataGridViewSMS1.Rows[number].Cells[0].Value = list[0][i];
                dataGridViewSMS1.Rows[number].Cells[1].Value = list[1][i];
                dataGridViewSMS1.Rows[number].Cells[2].Value = list[2][i];
                dataGridViewSMS1.Rows[number].Cells[3].Value = list[3][i];
            }

            dataGridViewSMS2.Rows.Clear();
            list = ConnectMySql.getListSMS(2); 
            for (int i = 0; i < list[0].Count; i++)
            {
                int number = dataGridViewSMS2.Rows.Add();
                dataGridViewSMS2.Rows[number].Cells[0].Value = list[0][i];
                dataGridViewSMS2.Rows[number].Cells[1].Value = list[1][i];
                dataGridViewSMS2.Rows[number].Cells[2].Value = list[2][i];
                dataGridViewSMS2.Rows[number].Cells[3].Value = list[3][i];
            }

            dataGridViewSMS3.Rows.Clear();
            list = ConnectMySql.getListSMS(3);
            for (int i = 0; i < list[0].Count; i++)
            {
                int number = dataGridViewSMS3.Rows.Add();
                dataGridViewSMS3.Rows[number].Cells[0].Value = list[0][i];
                dataGridViewSMS3.Rows[number].Cells[1].Value = list[1][i];
                dataGridViewSMS3.Rows[number].Cells[2].Value = list[2][i];
                dataGridViewSMS3.Rows[number].Cells[3].Value = list[3][i];
            }

        }

        private void btnReceiveSMS_Click(object sender, EventArgs e)
        {
            receiveSMS();
        }
    }
}