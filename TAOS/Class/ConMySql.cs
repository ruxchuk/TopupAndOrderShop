using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Threading;
using System.Globalization;

namespace TAOS
{
    class ConMySql
    {
        #region ประกาศค่า global

        private MySqlConnection connection;
        private string Server, NameDB, UserId, Pass;
        public int CountAdd = 0;
        public int customerID = 0;
        public int phoneNumberID = 0;
        public DateTime dateStart;
        public DateTime dateEnd;

        #endregion

        public ConMySql(string user, string pass, string server, string dbName)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            UserId = user;
            Pass = pass;
            Server = server;
            NameDB = dbName;
            Initialize();
        }

        private void Initialize()
        {
            string connectionString = "SERVER=" + Server + ";" + "DATABASE=" +
                   NameDB + ";" + "UID=" + UserId + ";" + "PASSWORD=" + Pass + ";Charset=utf8;";
            connection = new MySqlConnection(connectionString);
            CheckConnect();
            CloseConnection();
            //throw new NotImplementedException();
        }


        public DateTime getDateStartOfDay()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        public DateTime getDateEndOfDay()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
        }
        

        #region Check Connect/Close Connect
        
        public Boolean CheckConnect()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("กรุณาตรวจสอบ Username หรือ Password\nของฐานข้อมูล อีกครั้ง", "แจ้งเตือน"
                   , MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                        break;
                }
                return false;
            }
        }

        public  bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
       
        #endregion

        #region Select

        public bool checkAddProduct(string barcode)
        {
            if (CheckConnect() == true)
            {
                string sql = "SELECT * FROM product WHERE barcode = '" + barcode + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                int countData = 0;
                while (dataReader.Read())
                {
                    countData++;
                }
                CloseConnection();
                dataReader.Close();
                if (countData > 0)
                {                    
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public List<string>[] getSearchProduct(string strSearch)
        {
            List<string>[] list = new List<string>[6];
            for (int i = 0; i < 6; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = "SELECT * FROM product WHERE product_name LIKE '%" + strSearch.Trim() + "%'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_modified"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string>[] getProduct(string barcode)
        {
            List<string>[] list = new List<string>[6];
            for (int i = 0; i < 6; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = "SELECT * FROM product WHERE barcode = '" + barcode.Trim() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader(); 
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_modified"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string>[] getAllProduct()
        {
            List<string>[] list = new List<string>[6];
            for (int i = 0; i < 6; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = "SELECT * FROM product ORDER BY product_id DESC LIMIT 50";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader(); 
                while (dataReader.Read())
                {                    
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_modified"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string>[] getAllPhoneNumber(string searchPhoneNumber = "")
        {
            List<string>[] list = new List<string>[4];
            for (int i = 0; i < 4; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect())
            {
                string sql = "CALL sp_get_list_phone_number('" + searchPhoneNumber + "');";

                Debug.WriteLine(sql);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["phone_number_id"] + "");
                    list[1].Add(dataReader["phone_number"] + "");
                    list[2].Add(dataReader["network"] + "");
                    list[3].Add(dataReader["customer_id"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }

        public List<string>[] getListTopup(int isTopup = 0)
        {
            dateStart = getDateStartOfDay();
            dateEnd = getDateEndOfDay();

            int countList = 8;
            List<string>[] list = new List<string>[countList];
            for (int i = 0; i < countList; i++)
            {
                list[i] = new List<string>();
            }

            if (CheckConnect())
            {
                string sql = "CALL sp_get_list_topup(" + isTopup + ", '" + dateStart + 
                    "', '" + dateEnd + "');";

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["topup_id"] + "");
                    list[1].Add(dataReader["phone_number"] + "");
                    list[2].Add(dataReader["topup_amount"] + "");
                    list[3].Add(dataReader["network"] + "");
                    list[4].Add(dataReader["customer_name"] + "");
                    list[5].Add(dataReader["date_add"] + "");
                    list[6].Add(dataReader["customer_id"] + "");
                    list[7].Add(dataReader["phone_number_id"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return list;
            }
            else
            {
                return list;
            }
        }

        public string countProduct()
        {
            string strCount = "";
            if (CheckConnect() == true)
            {
                string sql = "SELECT COUNT(`product_id`) AS id FROM `product`";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    strCount = dataReader["id"] + "";
                }
                dataReader.Close();
                CloseConnection();
                return strCount;
            }
            else
            {
                return strCount;
            }
        }

        #endregion

        #region INSERT

        public string insertProduct(string sql) 
        {
            //SQL1=SelectTop(SQL1,"TID");
            //SQL2 += SQL1 + ")";
            //InsertOnTop(SQL2);
            //SQL2 = SelectTop(SQL3, "ID");
            //SQL4 += SQL2 + "')";
            //InsertOnTop(SQL4);
            //return SQL1; .
            return "";
        }

        public bool runQuery(string sql)
        {
            if (CheckConnect() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }
                CloseConnection();
                return true;
            }
            else
            {
                return false;
            }
        }

        public int addPhoeNumber(string phoneNumber, string network)
        {
            phoneNumberID = 0;
            string sql = "CALL sp_add_phone_number('" + phoneNumber + "', '" + network + "');";
            if (CheckConnect())
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    phoneNumberID = int.Parse(dataReader["id"] + "");
                }
                dataReader.Close();
                CloseConnection();
            }
            return phoneNumberID;
        }

        public bool addTopup(string phoneNumber, string network, string amount)
        {
            string sql;
            if (phoneNumberID == 0)
            {
                addPhoeNumber(phoneNumber, network);
            }

            sql = "CALL sp_add_topup(" + customerID + ", " + phoneNumberID + ", " + amount + ");";
            if (!runQuery(sql))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Update

        public bool UpDate(string sql)
        {
            if (CheckConnect() == true)
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch {
                    return false;
                }
                CloseConnection();
            }
            return true;
        }

        public bool setDeletedTopup(string topup_id)
        {
            string sql = "CALL sp_set_deleted_topup(" + topup_id + ");";
            return UpDate(sql);
        }

        public bool setIsTopupAll()
        {
            string sql = "CALL sp_set_is_topup_all();";
            return UpDate(sql);
        }
        
        #endregion

        #region Delete
        
        public string Delete(string sql)
        {
            if (CheckConnect() == true)
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                //try
                //{
                    cmd.ExecuteNonQuery();
                //}
                //catch { }
                CloseConnection();
            }
            return sql;
        }
        
        #endregion
       
    }
}
