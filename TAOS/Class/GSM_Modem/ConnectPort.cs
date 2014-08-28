using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace TAOS
{
    public class ConnectPort
    {

        #region Open and Close Ports
        //Open Port
        public SerialPort OpenPort(
            string p_strPortName = "",
            int p_uBaudRate =9600,
            int p_uDataBits = 8,
            int p_uReadTimeout = 300,
            int p_uWriteTimeout = 300)
        {
            receiveNow = new AutoResetEvent(false);
            SerialPort port = new SerialPort();

            try
            {
                port.PortName = p_strPortName;                 //COM1
                port.BaudRate = p_uBaudRate;                   //9600
                port.DataBits = p_uDataBits;                   //8
                port.StopBits = StopBits.One;                  //1
                port.Parity = Parity.None;                     //None
                port.ReadTimeout = p_uReadTimeout;             //300
                port.WriteTimeout = p_uWriteTimeout;           //300

                port.Encoding = Encoding.GetEncoding("iso-8859-1");
                //port.Encoding = Encoding.GetEncoding("tis-620");
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                port.Open();
                port.DtrEnable = true;
                port.RtsEnable = true;
            }
            catch (Exception ex)
            {
                //throw ex;
                return null;
            } //Debug.Write("open port");
            return port;
        }

        //Close Port
        public void ClosePort(SerialPort port)
        {
            try
            {
                port.Close();
                port.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                port = null;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        #endregion

        public string responseUSSD = "";
        public USSD ussd = new USSD();

        public string[] getPort()
        {
            string[] ports = null;
            try
            {
                #region Display all available COM Ports
                ports = SerialPort.GetPortNames();

                // Add all port names to the combo box:
                foreach (string port in ports)
                {
                    //this.cboPortName.Items.Add(port);
                //Debug.WriteLine(port);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
            return ports;
        }

        public bool checkImei(string imei, SerialPort port)
        {

            try
            {
                //eheck imei "AT+CGSN"
                string strResponse = ExecCommand(port, "AT+CGSN", 300, "No phone connected");
                //Debug.WriteLine(strResponse);
                if (strResponse.EndsWith("\r\nOK\r\n"))
                {
                    if (strResponse.IndexOf(imei) != -1)
                        return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public bool checkConnectPortByName(ref string portName)
        {
            //if (portName == "" || portName == "No Port")
            //{
            //    portName = "No Port";
            //    return false;
            //}
            SerialPort portVal = new SerialPort();
            try
            {
                portVal = OpenPort(
                            portName
                        );
                Thread.Sleep(300);
                string strResponse = ExecCommand(portVal, "AT", 300, "No phone connected");
                if (strResponse.EndsWith("\r\nOK\r\n"))
                {
                    ClosePort(portVal);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            ClosePort(portVal);
            portName = "No Port";
            return false;
        }
        
        public string getPortByImei(string imei)
        {
            string checkPort = "No Port";
            string[] ports = getPort();
            SerialPort portVal = new SerialPort();
            foreach (string port in ports)
            {
                try
                {
                    //Open communication port 
                    portVal = OpenPort(
                        port
                    );
                }
                catch (Exception ex)
                {
                    //ErrorLog(ex.Message);
                    portVal = null;
                } //Debug.WriteLine(port);
                if (portVal != null)
                {
                    if (checkConnectPort(portVal))
                    {
                        if (checkImei(imei, portVal))
                        {
                            checkPort = port;
                        }
                    }
                    ClosePort(portVal);
                }
            }
            return checkPort;
        }
        
        public string setPort(ref SerialPort _port, string imei)
        {
            if (_port != null)
            {
                if (checkConnectPort(_port))
                { 
                    return _port.PortName; 
                }
            }
            string strPortName = "No Port";
            string[] ports = getPort();
            SerialPort portVal = new SerialPort();
            foreach (string port in ports)
            {
                try
                {
                    //Open communication port 
                    portVal = OpenPort(
                        port
                    );
                }
                catch (Exception ex)
                {
                    //ErrorLog(ex.Message);
                    portVal = null;
                } //Debug.WriteLine(port);
                if (portVal != null)
                {
                    if (checkConnectPort(portVal))
                    {
                        if (checkIMEI(portVal, imei))
                        {
                            _port = portVal;
                            strPortName = port;//ห้าม close port
                            return strPortName;
                        }
                    }
                    else
                    {
                        _port = null;
                    }
                    ClosePort(portVal);
                }
            }
            return strPortName;
        }

        public bool checkConnectPort(SerialPort port)
        {
            try
            {
                string strResponse = "";
                strResponse = ExecCommand(port, "AT", 300, "No phone connected");
                if (strResponse.EndsWith("\r\nOK\r\n"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public string topupUSSD(string command, string port, bool encode = false)
        {
            string message = "";
            responseUSSD = "";
            SerialPort sPort = new SerialPort();
            string response = "No Response";
            sPort = OpenPort(
                        port
                    ); Debug.WriteLine(sPort);
            Thread.Sleep(300);
            message = sendUSSD(sPort, command, encode);
            if (message == "USSD Error!")
            {
                return message;
            } else 
                try
                {
                    Debug.WriteLine(responseUSSD);
                    response = responseUSSD;
                    response = response.Substring(response.IndexOf("\"") + 1);
                    response = response.Substring(0, response.IndexOf("\""));
                    response = ussd.decodeResponseUSSDToText(response);
                }
                catch
                {
                    response = message;
                }
            //}
            ClosePort(sPort);
            return "เติมเงินสำเร็จ";
        }

        public bool checkIMEI(SerialPort port, string imei)
        {
            string result = "";
            string command = "AT+GSN";
            result = ExecCommand(port, command, 300, "error check IMEI");
            try
            {
                if (result.EndsWith("\r\nOK\r\n"))
                {
                    result = result.Replace("OK", "").Replace("AT+GSN", "").Trim();
                    result = ussd.cutStringIMEI(result);
                }
            }
            catch
            {
            }Debug.WriteLine("'"+imei +"-"+result+"'");
            if (imei == result)
                return true;
            return false;
        }

        //Execute AT Command
        public string ExecCommand(
            SerialPort port,
            string command,
            int responseTimeout,
            string errorMessage
            )
        {
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                receiveNow.Reset();
                port.Write(command + "\r");

                string input = null;
                input = ReadResponse(port, responseTimeout);
                if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.EndsWith("\r\nOK\r\n"))))
                    throw new ApplicationException("No success message was received.");

                return input;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //Receive data from port
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string ReadResponse(SerialPort port, int timeout)
        {
            string buffer = string.Empty;

            try
            {
                do
                {
                    if (receiveNow.WaitOne(timeout, true))
                    {
                        string t = port.ReadExisting();
                        buffer += t;
                        Debug.WriteLine(t);
                        responseUSSD += t;
                    }
                    else
                    {
                        if (buffer.Length > 0)
                            //throw new ApplicationException("Response received is incomplete.");
                            return buffer;
                        else
                            return buffer;
                            //throw new ApplicationException("No data received from phone.");
                    }
                }
                while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }

        #region Count SMS
        public int CountSMSmessages(SerialPort port)
        {
            int CountTotalMessages = 0;
            try
            {

                #region Execute Command

                string recievedData = ExecCommand(port, "AT", 300, "No phone connected at ");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                //Debug.WriteLine(recievedData);
                String command = "AT+CPMS?";
                recievedData = ExecCommand(port, command, 1000, "Failed to count SMS message");
                //Debug.WriteLine(recievedData);
                int uReceivedDataLength = recievedData.Length;

                #endregion

                #region If command is not executed successfully
                //if ((recievedData.Length >= 45) && (recievedData.StartsWith("AT+CPMS?")))
                if (recievedData.Contains("ERROR"))
                {

                    #region Error in Counting total number of SMS
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while counting the message" + recievedError;
                    #endregion

                }
                #endregion

                #region If command is executed successfully
                else //if ((recievedData.StartsWith("+CPMS")))
                {

                    #region Parsing SMS
                    string[] strSplit = recievedData.Split(',');
                    string strMessageStorageArea1 = strSplit[0];     //SM
                    string strMessageExist1 = strSplit[1];           //Msgs exist in SM
                    #endregion

                    #region Count Total Number of SMS In SIM
                    CountTotalMessages = Convert.ToInt32(strMessageExist1);
                    #endregion

                }
                #endregion

                return CountTotalMessages;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Read SMS

        public AutoResetEvent receiveNow;
        public ShortMessageCollection ReadSMS(SerialPort port)
        {
            //SerialPort port = new SerialPort();
            //port = OpenPort(strPort);
            //Thread.Sleep(300);

            string p_strCommand = "AT+CMGL=\"ALL\"";
            string receive = "";

            // Set up the phone and read the messages
            ShortMessageCollection messages = null;
            try
            {
                #region Execute Command
                // Check connection
                receive += ExecCommand(port, "AT", 300, "No phone connected");
                // Use message format "Text mode"
                receive += ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                // Use character set "PCCP437"
                //receive += ExecCommand(port, "AT+CSCS=\"PCCP437\"", 300, "Failed to set character set.");
                // Select SIM storage
                //receive += ExecCommand(port, "AT+CPMS=\"SM\"", 300, "Failed to select message storage.");
                receive += ExecCommand(port, "AT+CMGR=1", 300, "Failed to select message storage.");
                Debug.WriteLine(receive);
                // Read the messages
                string input = ExecCommand(port, p_strCommand, 800, "Failed to read the messages.");
                Debug.WriteLine(input);
                #endregion

                #region Parse messages
                messages = ParseMessages(input);
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

            //ClosePort(port);
            if (messages != null)
                return messages;
            else
                return null;

        }

        public ShortMessageCollection ParseMessages(string input)
        {
            ShortMessageCollection messages = new ShortMessageCollection();
            try
            {
                Regex r = new Regex(@"\+CMGL: (\d+),""(.+)"",""(.+)"",(.*),""(.+)""\r\n(.+)\r\n");
                Match m = r.Match(input);
                while (m.Success)
                {
                    ShortMessage msg = new ShortMessage();
                    //msg.Index = int.Parse(m.Groups[1].Value);
                    msg.Index = m.Groups[1].Value;
                    msg.Status = m.Groups[2].Value;
                    msg.Sender = m.Groups[3].Value;
                    msg.Alphabet = m.Groups[4].Value;
                    msg.Sent = m.Groups[5].Value;
                    msg.Message = m.Groups[6].Value;
                    messages.Add(msg);

                    m = m.NextMatch();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return messages;
        }

        #endregion

        #region Send USSD

        public string sendUSSD(SerialPort port, string ussdCode, bool encode)
        {
            string messages = null;

            try
            {
                string recievedData = "";
                string strCommand = "";
                string strResponse = "";
                recievedData = ExecCommand(port, "AT", 300, "No phone connected");

                //if (recievedData.EndsWith("\r\nOK\r\n"))
                //{
                    recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                //}
                //else
                //{
                //    return strResponse;
                //}
                Debug.WriteLine(recievedData + "|1");
                if (encode)
                {
                    ussdCode = ussd.EncodeTo7Bits(ussdCode);
                }

                strCommand = "AT+CUSD=1,\"" + ussdCode + "\",15";// +System.Environment.NewLine;

                //if (recievedData.EndsWith("\r\nOK\r\n"))
                //{
                    recievedData = ExecCommand(port, strCommand, 300, "Failed to read the messages.");
                //}
                //else
                //{
                //    return strResponse;
                //}
                Debug.WriteLine(recievedData + "|2");
                //Debug.WriteLine(strResponse);
                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    strResponse = ExecCommand(port, "", 1000, "Failed to read the messages.");
                    Debug.WriteLine(strResponse + "|3");
                }
                else
                {
                    return "USSD Error!";
                }
                #region Parse messages
                //messages = ParseMessages(input);
                return strResponse;
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Send SMS

        static AutoResetEvent readNow = new AutoResetEvent(false);

        public bool sendMsg(SerialPort port, string PhoneNo, string Message)
        {
            bool isSend = false;

            try
            {

                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                String command = "AT+CMGS=\"" + PhoneNo + "\"";
                recievedData = ExecCommand(port, command, 300, "Failed to accept phoneNo");
                command = Message + char.ConvertFromUtf32(26) + "\r";
                recievedData = ExecCommand(port, command, 3000, "Failed to send message"); //3 seconds
                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isSend = true;
                }
                else if (recievedData.Contains("ERROR"))
                {
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                    readNow.Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete SMS
        public bool DeleteMsg(SerialPort port)
        {
            string p_strCommand = "AT+CMGD=1,4";
            bool isDeleted = false;
            try
            {

                #region Execute Command
                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                String command = p_strCommand;
                recievedData = ExecCommand(port, command, 300, "Failed to delete message");
                #endregion

                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isDeleted = true;
                }
                if (recievedData.Contains("ERROR"))
                {
                    isDeleted = false;
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                //throw ex;
                return isDeleted;
            }

        }
        #endregion


        #region Error Log
        public void ErrorLog(string Message)
        {
            StreamWriter sw = null;

            try
            {
                //WriteStatusBar(Message);

                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                //string sPathName = @"E:\";
                string sPathName = @"SMSapplicationErrorLog_";

                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();

                string sErrorTime = sDay + "-" + sMonth + "-" + sYear;

                sw = new StreamWriter(sPathName + sErrorTime + ".txt", true);

                sw.Flush();

            }
            catch (Exception ex)
            {
                //ErrorLog(ex.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw.Close();
                }
            }

        }
        #endregion 
    }
}
