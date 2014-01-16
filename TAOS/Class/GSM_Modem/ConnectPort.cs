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
            string p_strPortName,
            int p_uBaudRate,
            int p_uDataBits,
            int p_uReadTimeout,
            int p_uWriteTimeout)
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
                throw ex;
            }
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
                throw ex;
            }
        }

        #endregion

        public string responseUSSD = "";
        public SerialPort _Port;
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
                Debug.WriteLine(port);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
            return ports;
        }

        public string setPort()
        {
            string checkPort = "No Port";
            string[] ports = getPort(); 
            SerialPort portVal = new SerialPort();
            foreach (string port in ports)
            {
                Debug.WriteLine(port);
                try
                {
                    //Open communication port 
                    portVal = OpenPort(
                        port,
                        Convert.ToInt32("9600"),
                        Convert.ToInt32("8"),
                        Convert.ToInt32("300"),
                        Convert.ToInt32("300")
                    );
                }
                catch (Exception ex)
                {
                    //ErrorLog(ex.Message);
                    portVal = null;
                } Debug.WriteLine(port);
                if (portVal != null)
                {
                    if (checkConnectPort(portVal))
                    {
                        _Port = portVal;
                        return port;
                    }
                    else
                    {
                        ClosePort(portVal);
                    }
                }
            }
            return checkPort;
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


        public string topupUSSD(string command)
        {
            string message = "";
            message = sendUSSD(_Port, command);
            string response = responseUSSD;
            try
            {
                response = response.Substring(response.IndexOf("\"") + 1);
                response = response.Substring(0, response.IndexOf("\""));
                response = ussd.decodeResponseUSSDToText(response);
            }
            catch
            {
                response = "Topup Error!";
            }
            return response;
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
            responseUSSD = "";
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
                            throw new ApplicationException("Response received is incomplete.");
                        else
                            throw new ApplicationException("No data received from phone.");
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
                String command = "AT+CPMS?";
                recievedData = ExecCommand(port, command, 1000, "Failed to count SMS message");
                int uReceivedDataLength = recievedData.Length;

                #endregion

                #region If command is executed successfully
                if ((recievedData.Length >= 45) && (recievedData.StartsWith("AT+CPMS?")))
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

                #region If command is not executed successfully
                else if (recievedData.Contains("ERROR"))
                {

                    #region Error in Counting total number of SMS
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while counting the message" + recievedError;
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

        public ShortMessageCollection ReadSMS(SerialPort port, string p_strCommand)
        {

            // Set up the phone and read the messages
            ShortMessageCollection messages = null;
            try
            {

                #region Execute Command
                // Check connection
                ExecCommand(port, "AT", 300, "No phone connected");
                // Use message format "Text mode"
                ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                // Use character set "PCCP437"
                ExecCommand(port, "AT+CSCS=\"PCCP437\"", 300, "Failed to set character set.");
                // Select SIM storage
                ExecCommand(port, "AT+CPMS=\"SM\"", 300, "Failed to select message storage.");
                // Read the messages
                string input = ExecCommand(port, p_strCommand, 5000, "Failed to read the messages.");
                #endregion

                #region Parse messages
                messages = ParseMessages(input);
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        public string sendUSSD(SerialPort port, string ussdCode)
        {
            string messages = null;

            try
            {
                string recievedData = "";
                string strCommand = "";
                string strResponse = "";
                string input = "";
                recievedData = ExecCommand(port, "AT", 300, "No phone connected");

                ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                strCommand = "AT+CUSD=1,\"" + ussdCode + "\",15";// +System.Environment.NewLine;
                strResponse = ExecCommand(port, strCommand, 300, "Failed to read the messages.");
                //Debug.WriteLine(strResponse);
                if (strResponse.EndsWith("\r\nOK\r\n"))
                {
                    input = ExecCommand(port, "", 5000, "Failed to read the messages.");
                    //Debug.WriteLine(input);
                }

                messages = input;
                #region Parse messages
                //messages = ParseMessages(input);
                return messages;
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
        public bool DeleteMsg(SerialPort port, string p_strCommand)
        {
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
                throw ex;
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

                sw.WriteLine(sLogFormat + Message);
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
