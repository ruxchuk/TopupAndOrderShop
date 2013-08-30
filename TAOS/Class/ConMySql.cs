﻿using System;
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
        public string connectionString;

        #endregion

        public ConMySql()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Initialize();
        }

        private void Initialize()
        {
            connectDB();
            //CheckConnect();
            //CloseConnection();
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

        public void connectDB()
        {
            try
            {
                string path = "Files\\FileSave.txt";
                StreamReader strm = File.OpenText(path);

                UserId = strm.ReadLine();
                Pass = strm.ReadLine();
                Server = strm.ReadLine();
                NameDB = strm.ReadLine();
                strm.Close();
                connectionString = "SERVER=" + Server + ";" + "DATABASE=" +
                   NameDB + ";" + "UID=" + UserId + ";" + "PASSWORD=" + Pass + ";Charset=utf8;";
                connection = new MySqlConnection(connectionString);
                if (CheckConnect())
                {
                    CloseConnection();
                }
                else
                {
                    MessageBox.Show("เชื่อมต่อฐานข้อมูล ผิดพลาด");
                }
            }
            catch
            {
                MessageBox.Show("กรุณาตรวจสอบ Username หรือ Password\n", ""
                   , MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

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

        #region Get Product
        
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

        #region TopUP

        //get รายการเบอร์โทร
        public List<string>[] getListPhoneNumber(string searchPhoneNumber = "")
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

        //get รายการเติมเงิน
        public List<string>[] getListTopup(int isTopup = 0, string phoneNumber = "", 
            string date = "", string time = "", string network = "")
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
                //string sql = "CALL sp_get_list_topup(" + isTopup + ", '" + dateStart + 
                //    "', '" + dateEnd + "');";
                string sql = @"
                    SELECT 
                      a.id AS topup_id,
	                    b.phone_number,
	                    a.topup_amount,
	                    b.network,
	                    IFNULL(c.`name`, 'ไม่มีชื่อ') AS customer_name,
	                    a.date_add,
	                    IFNULL(c.id, 0) AS customer_id,
	                    b.id AS phone_number_id
                    FROM
                      `topup` a 
                      INNER JOIN `phone_number` b 
                        ON a.`phone_number_id` = b.`id` 
                      LEFT JOIN `customer` c 
                        ON c.`id` = a.`customer_id` 
                        AND c.`deleted` = 0 
                    WHERE 1 
                      AND a.`deleted` = 0 
                      AND b.`deleted` = 0 
                      AND a.`is_topup` = " + isTopup ;
                sql += date == "" ? "" : " AND Date(a.date_add) = '" + date + "'";
                sql += network == "" ? "" : " AND b.network = '" + network + "'";
                sql += time == "" || time == "0" ? "" : " AND HOUR(a.date_add) = '" + time + "'";
                sql += phoneNumber == "" ? "" : " AND b.phone_number LIKE '%" + phoneNumber.Replace("-", "") + "%'";
                Debug.WriteLine(sql);
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


        //get รายชื่อลูกค้า
        public List<string>[] getListCustomer(int customerID = 0, string customerName = "")
        {
            int countList = 8;
            List<string>[] list = new List<string>[countList];
            for (int i = 0; i < countList; i++)
            {
                list[i] = new List<string>();
            }

            if (CheckConnect())
            {
                string sql = "CALL sp_get_list_customer(" + customerID + ", '" + customerName + "');";

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["phone_number_id"] + "");
                    list[2].Add(dataReader["name"] + "");
                    list[3].Add(dataReader["date_update"] + "");
                    list[4].Add(dataReader["date_add"] + "");
                    list[5].Add(dataReader["address"] + "");
                    list[6].Add(dataReader["phone_number"] + "");
                    list[7].Add(dataReader["network"] + "");
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
        
        //search customer
        public List<string>[] searchCustomer(string customerName, string phoneNumber, string network)
        {
            int countList = 8;
            List<string>[] list = new List<string>[countList];
            for (int i = 0; i < countList; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect())
            {
                string sql = @"
                    SELECT
                      a.*,
	                    b.phone_number,
	                    b.network
                    FROM `customer` a
                    LEFT JOIN `phone_number` b ON
                    (a.phone_number_id = b.id AND b.deleted = 0)
                    WHERE 1
                ";
                sql += customerName != "" ? " AND a.name LIKE '%" + customerName + "%'" : "";
                sql += phoneNumber != "" ? " AND b.network ='" + phoneNumber + "'" : "";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["phone_number_id"] + "");
                    list[2].Add(dataReader["name"] + "");
                    list[3].Add(dataReader["date_update"] + "");
                    list[4].Add(dataReader["date_add"] + "");
                    list[5].Add(dataReader["address"] + "");
                    list[6].Add(dataReader["phone_number"] + "");
                    list[7].Add(dataReader["network"] + "");
                }
                dataReader.Close();
                CloseConnection();
            }

            return list;
        }

        #endregion

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

        public int runQuery(string sql)
        {
            int lastID = 0;
            if (CheckConnect() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    lastID = (int)cmd.LastInsertedId;
                    //MessageBox.Show(cmd.LastInsertedId.ToString());
                }
                catch
                {
                    return 0;
                }
                CloseConnection();
                return lastID;
            }
            else
            {
                return 0;
            }
        }

        public int addPhoeNumber(string phoneNumber, string network)
        {
            phoneNumberID = 0;
            //string sql = "CALL sp_add_phone_number('" + phoneNumber + "', '" + network + "');";
            string sql = @"
                INSERT INTO `phone_number`
                (
	                `phone_number`,
                  `network`,
	                `date_add`
                )
                VALUES 
                (
	                '" + phoneNumber + @"',
	                '" + network + @"',
	                NOW()
                );
                ";
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

            //sql = "CALL sp_add_topup(" + customerID + ", " + phoneNumberID + ", " + amount + ");";
            sql = @"
                INSERT INTO `topup` (
                  `customer_id`,
                  `phone_number_id`,
                  `topup_amount`,
                  `date_add`
                ) 
                VALUES
                (
	                " + customerID + @",
	                " + phoneNumberID + @",
	                " + amount + @",
	                NOW()
                );";
            if (runQuery(sql) == 0)
            {
                return false;
            }
            return true;
        }

        public bool addBehindhand(string topupID, string customerID, string topupAmount, string dateTimeTopup)
        {            
            //string sql = "CALL sp_add_bhindhand(" + customerID + ", " + topupID + ", " +
            //    topupAmount + ", " + dateTimeTopup + ");";
            string sql = @"
                INSERT INTO 
	                `topup_behindhand` 
                (
                  `topup_id`,
                  `customer_id`,
                  `price`,
                  `date_behind`,
                  `date_payment`
                )
                VALUES(" + customerID + ", " + topupID + ", " +
                    topupAmount + ", " + dateTimeTopup + ");";
            if (runQuery(sql)==0)
            {
                return false;
            }
            return true;
        }


        // add customer
        public bool addCustomer(string customerName, string phoneNumber, string address, string network)
        {
            string dateAdd = getDateTimeNow();
            string sql = @"
                INSERT INTO `phone_number` (
                  `phone_number`,
                  `network`,
                  `date_add`
                ) 
                VALUES
                  (
                    '" + phoneNumber + @",'
                    '" + network + @"',
                    '" + dateAdd + @"'
                  ) 
                  ;
            ";
            int id = runQuery(sql);
            if (id == 0)
            {
                return false;
            }

            sql = @"
                INSERT INTO `customer` (
                  `phone_number_id`,
                  `name`,
                  `date_add`,
                  `address`,
                ) 
                VALUES
                (
                " + id + @",
                '" + customerName + @"',
                '" + dateAdd + @"',
                '" + address + @"'
                ) ;
            ";
            id = runQuery(sql);
            if (id == 0)
            {
                return false;
            }
            return true;
        }


        #endregion

        #region Update

        public bool changeToNoTopup(string id)
        {
            string sql = @"
                UPDATE 
                  `topup` 
                SET
                  `is_topup` = 0,
                  `date_topup` = '0000-00-00 00:00:00' 
                WHERE `id` =
                " + id;
            return UpDate(sql);
        }

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


        public string getDateTimeNow()
        {
            string DTNow = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +
                " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            return DTNow;
        }
    }
}
