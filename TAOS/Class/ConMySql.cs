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
        public string connectionString;
        public bool checkConDB = false;

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
                    checkConDB = true;
                }
                else
                {
                    checkConDB = false;
                    MessageBox.Show("เชื่อมต่อฐานข้อมูล ผิดพลาด");
                }
            }
            catch
            {
                checkConDB = false;
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
            List<string>[] list = new List<string>[7];
            for (int i = 0; i < 7; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = @"
                    SELECT 
                      * 
                    FROM
                      product 
                    WHERE 1 
                      and `deleted` = 0  
                      and product_name LIKE '%" + strSearch.Trim() + "%'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_update"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                    list[6].Add(dataReader["date_add"] + "");
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
            List<string>[] list = new List<string>[7];
            for (int i = 0; i < 7; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = @"
                SELECT * FROM product WHERE 1 and delete=0 and barcode = '" + barcode.Trim() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader(); 
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_update"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                    list[6].Add(dataReader["date_add"] + "");
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
            List<string>[] list = new List<string>[7];
            for (int i = 0; i < 7; i++)
            {
                list[i] = new List<string>();
            }
            if (CheckConnect() == true)
            {
                string sql = @"
                    SELECT 
                      `product_id`,
                      `product_name`,
                      `price`,
                      `type_value`,
                      `barcode`,
                      `date_add`,
                      CONCAT(
                        DATE(`date_update`),
                        ' ',
                        TIME(`date_update`)
                      ) AS date_update,
                      `deleted` 
                    FROM
                      `product` 
                    WHERE 1 
                      AND `deleted` = 0 
                    ORDER BY product_id DESC 
                    LIMIT 50  
                    ";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader(); 
                while (dataReader.Read())
                {                    
                    list[0].Add(dataReader["product_id"] + "");
                    list[1].Add(dataReader["product_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_update"] + "");
                    list[4].Add(dataReader["barcode"] + "");
                    list[5].Add(dataReader["type_value"] + "");
                    list[6].Add(dataReader["date_add"] + "");
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
                string sql = @"
                    SELECT 
                      COUNT(`product_id`) AS id 
                    FROM
                      `product` 
                    where 1 
                      and `deleted` = 0
                ";
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
                //string sql = "CALL sp_get_list_phone_number('" + searchPhoneNumber + "');";
                string sql = @"
                    SELECT 
                      a.`phone_number`,
                      a.`network`,
                      a.`date_update` AS a_date_update,
                      a.`date_add`,
                      a.id AS phone_number_id,
                      IFNULL(b.id, 0) AS customer_id,
                      b.`name`,
                      b.`date_update` AS b_date_update,
                      b.`date_add`,
                      b.`address` 
                    FROM
                      phone_number a 
                      LEFT JOIN customer b 
                        ON a.id = b.phone_number_id 
                        AND b.deleted = 0 
                    WHERE 1 
                      AND a.deleted = 0 ";
                sql += searchPhoneNumber == "" ?"": " AND a.phone_number LIKE '%"+searchPhoneNumber+ "%'";
                sql += @"
                    GROUP BY a.phone_number 
                    ORDER BY a.date_update DESC 
                    LIMIT 20
                    ;";
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
                        ON c.`phone_number_id` = b.`id` 
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
        public List<string>[] getListCustomer(string customerName, string phoneNumber, string network)
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
                    (a.phone_number_id = b.id)
                    WHERE 1
                    AND a.deleted = 0
                    AND b.deleted = 0
                ";
                sql += customerName != "" ? " AND a.name LIKE '%" + customerName + "%'" : "";
                sql += phoneNumber != "" ? " AND b.phone_number ='" + phoneNumber + "'" : "";
                sql += network != "" ? " AND b.network ='" + network + "'" : "";
                sql += " LIMIT 30";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["phone_number_id"] + "");
                    list[2].Add(dataReader["name"] + "");
                    list[3].Add(dataReader["date_add"] + "");
                    list[4].Add(dataReader["date_update"] + "");
                    list[5].Add(dataReader["address"] + "");
                    list[6].Add(dataReader["phone_number"] + "");
                    list[7].Add(dataReader["network"] + "");
                }
                dataReader.Close();
                CloseConnection();
            }

            return list;
        }

        //check ว่ามีเบอร์นี้อยู่ไหม
        public int checkPhoneNumber(string phoneNumber)
        {
            if (CheckConnect())
            {
                string sql = @"
                SELECT
                  `id`,
                  `phone_number`,
                  `network`,
                  `date_update`,
                  `date_add`,
                  `deleted`
                FROM `phone_number`
                WHERE 1
                AND deleted = 0
                AND phone_number = '" + phoneNumber + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                int id = 0;
                while (dataReader.Read())
                {
                    id = int.Parse(dataReader["id"] + "");
                }
                dataReader.Close();
                CloseConnection();
                return id;
            }
            return 0;
        }

        #endregion

        #region Credit

        //get รายชื่อลูกค้าที่ค้างชำระ
        public List<string>[] getListCredit(
            string customerName = "", 
            string phoneNumber = "", 
            string price = "",
            string status = "0",
            string network = ""
            )
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
                      a.`id`,
                      a.`customer_name`,
                      a.`price`,
                      a.`date_add`,
                      a.`date_payment`,
                      a.`paid`,
                      IFNULL(c.`phone_number`, '-') AS phone_number,
                      c.`network` 
                    FROM
                      `credit` a 
                      LEFT JOIN `topup` b 
                        ON (
                          a.`topup_id` = b.`id` 
                          AND b.`deleted` = 0
                        ) 
                      LEFT JOIN `phone_number` c 
                        ON (
                          c.`id` = b.`phone_number_id` 
                          AND c.`deleted` = 0
                        ) 
                    WHERE 1 
                      AND a.`deleted` = 0
                ";
                sql += customerName != "" ? " AND a.customer_name LIKE '%" + customerName + "%'" : "";
                sql += phoneNumber != "" ? " AND c.phone_number LIKE '%" + phoneNumber + "%'" : "";
                sql += price != "" ? " AND a.price ='" + price + "'" : "";
                sql += status != "-1" ? " AND a.paid =" + status  : "";
                sql += network != "" ? " AND c.network ='" + network + "'" : "";
                sql += " LIMIT 100"; Debug.WriteLine(status);
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["id"] + "");
                    list[1].Add(dataReader["customer_name"] + "");
                    list[2].Add(dataReader["price"] + "");
                    list[3].Add(dataReader["date_add"] + "");
                    list[4].Add(dataReader["date_payment"] + "");
                    list[5].Add(dataReader["paid"] + "");
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
                cmd.ExecuteNonQuery();
                int lastID = (int)cmd.LastInsertedId;
                phoneNumberID = lastID;
                CloseConnection();
            }
            return phoneNumberID;
        }

        public bool addTopup(string phoneNumber, string network, string amount)
        {
            string sql;
            string id = "0";
            List<string>[] list = getListPhoneNumber(phoneNumber);
            for (int i = 0; i < list[0].Count; i++)
            {
                id = list[0][i];
            }
            if (id == "0")
            {
                id = addPhoeNumber(phoneNumber, network).ToString();
            }
            sql = @"
                UPDATE 
                  `phone_number` 
                SET
                  `network` = '" + network + @"' 
                WHERE `id` = '" + id + @"'
                  AND `deleted` = 0 ;";
            if (!UpDate(sql))
            {
                return false;
            }
            sql = @"                
                INSERT INTO `topup` (
                  `phone_number_id`,
                  `topup_amount`,
                  `date_add`
                ) 
                VALUES
                (
	                " + id + @",
	                " + amount + @",
	                NOW()
                );";
            if (runQuery(sql) == 0)
            {
                return false;
            }
            return true;
        }

        public bool addBehindhand(string topupID, string customerName, string customerID, string topupAmount)
        {            
            //string sql = "CALL sp_add_bhindhand(" + customerID + ", " + topupID + ", " +
            //    topupAmount + ", " + dateTimeTopup + ");";
            string sql = @"
                INSERT INTO `credit` (
                  `topup_id`,
                  `customer_name`,
                  `price`,
                  `date_add`
                )
                VALUES(" + topupID + ", '" + customerName + "'," +
                    topupAmount + ", NOW());";
            Debug.WriteLine(sql);
            if (runQuery(sql) == 0)
            {
                return false;
            }
            return true;
        }


        // add customer
        public bool addCustomer(string customerName, string phoneNumber, string address, string network)
        {
            string dateAdd = getDateTimeNow();
            int id = checkPhoneNumber(phoneNumber);
            string sql = "";
            if (id == 0)
            {
                sql = @"
                    INSERT INTO `phone_number` (
                      `phone_number`,
                      `network`,
                      `date_add`
                    ) 
                    VALUES
                      (
                        '" + phoneNumber + @"',
                        '" + network + @"',
                        NOW()
                      ) 
                      ;
                ";
                id = runQuery(sql);
            }
            if (id == 0)
            {
                return false;
            }

            sql = @"
                INSERT INTO `customer` (
                  `phone_number_id`,
                  `name`,
                  `date_add`,
                  `address`
                ) 
                VALUES
                (
                " + id + @",
                '" + customerName + @"',
                NOW(),
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

        public int updateCustomer(string customerID, string phoneNumberID,
            string name, string phoneNumber, string network, string address)
        {
            int checkMatchID = checkPhoneNumber(phoneNumber);
            if (checkMatchID != 0 && checkMatchID != int.Parse(phoneNumberID))
            {
                return -1;
            }
            string sql = @"
                UPDATE 
                  `customer` 
                SET
                  `name` = '" + name + @"',
                  `date_update` = NOW(),
                  `address` = '" + address + @"'
                WHERE `id` = " + customerID + @" ;
                UPDATE 
                  `phone_number` 
                SET
                  `phone_number` = '" + phoneNumber + @"',
                  `network` = '" + network + @"',
                  `date_update` = NOW() 
                WHERE `id` = " + phoneNumberID + @";
            "; Debug.WriteLine(sql);
            bool result = UpDate(sql);
            if (!result)
            {
                return 0;
            }
            return 1;
        }

        public bool setDeleteCustomer(string id)
        {
            string sql = @"
                UPDATE 
                  `customer` 
                SET
                  `deleted` = 1 
                WHERE `id` = " + id + @" ;
                ";
            return UpDate(sql);
        }

        public bool updateCredit(
            string creditID, 
            string customerName = "",
            string price = "",
            string paid = "",
            string deleted = "",
            string date_payment = "")
        {

            string sql = @"
                UPDATE 
                  `credit` 
                SET
                ";

            List<string> list = new List<string>();

            // You can convert it back to an array if you would like to
            //int[] terms = list.ToArray();
            //string[] arrSql;

            if (customerName != "")
            {
                list.Add(" `customer_name` = '" + customerName + "'");
            }
            if (price != "")
            {
                list.Add(" `price` = '" + price + "'");
            }
            if (paid != "")
            {
                list.Add(" `paid` = '" + paid + "'");
            }
            if (deleted != "")
            {
                list.Add(" `deleted` = '" + deleted + "'");
            }
            if (date_payment != "")
            {
                list.Add(" `date_payment` = '" + date_payment + "'");
            }

            string glue = ", ";
            string[] arrSql = list.ToArray();
            string resultingString = String.Join(glue, arrSql);

            if (resultingString == "")
            {
                return true;
            }
            sql += resultingString + @"
                WHERE `id` = '" + creditID + "' ;";
            Debug.WriteLine(sql);
            return UpDate(sql);
        }

        //run sql update
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
            string sql = "UPDATE topup SET deleted = 1 WHERE id = " + topup_id + ";";
            return UpDate(sql);
        }
        public bool setIsTopup(string topup_id)
        {
            string sql = "UPDATE topup SET is_topup = 1 WHERE id = " + topup_id + ";";
            return UpDate(sql);
        }

        public bool setIsTopupAll()
        {
            string sql = @"
                UPDATE 
	                topup 
                SET 
	                is_topup = 1, 
	                date_topup = NOW()
                WHERE 1
                AND is_topup = 0 
                AND deleted = 0;
            ";
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
