using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAOS
{
    public class USSD
    {
        #region Interface

        /*
         * แปลง response เป็น text
         * */
        public string decodeResponseUSSDToText(string res)
        {
            String _response = res;

            Byte[] _bytes = new Byte[_response.Length / 2];

            for (int i = 0; i < _response.Length / 2; i++)
                _bytes[i] = Convert.ToByte(_response.Substring(i * 2, 2), 16);

            _response = Encoding.BigEndianUnicode.GetString(_bytes);
            return _response;
        }

        public string EncodeTo7Bits(string data)
        {
            int sourceLength = data.Length;
            string reversedStr = "";

            for (int i = 0; i < sourceLength; i++)
            {
                byte value = Convert.ToByte(data[i]);
                string binary = Convert.ToString(value, 2);
                binary = binary.PadLeft(7, '0');

                reversedStr += GetInvertString(binary);
            }

            int totalLength = (reversedStr.Length / 8 + 1) * 8;
            reversedStr = reversedStr.PadRight(totalLength, '0');

            string output = "";

            while (reversedStr.Length > 0)
            {
                string symbolBinary = reversedStr.Substring(0, 8);
                symbolBinary = GetInvertString(symbolBinary);

                reversedStr = reversedStr.Substring(8);

                byte symbolByte = Convert.ToByte(symbolBinary, 2);
                string symbolHex = Convert.ToString(symbolByte, 16);
                symbolHex = symbolHex.PadLeft(2, '0');

                output += symbolHex.ToUpper();
            }

            return output;
        }

        public string Decode7bit(string source, int length = 0)
        {
            byte[] bytes = GetInvertBytes(source);

            string binary = string.Empty;

            foreach (byte b in bytes)
                binary += Convert.ToString(b, 2).PadLeft(8, '0');

            string result = string.Empty;

            binary = GetInvertString(binary);

            binary = binary.PadRight((binary.Length / 7 + 1) * 7, '0');

            while (binary.Length > 0)
            {
                string symbolByte = binary.Substring(0, binary.Length >= 7 ? 7 : binary.Length);
                symbolByte = GetInvertString(symbolByte);

                byte byteResult = Convert.ToByte(symbolByte, 2);

                result += Convert.ToChar(byteResult);

                binary = binary.Substring(7);
            }

            return RemoveLB(result);
        }

        public string endcodeStringToHex(string ussdCode)
        {
            char[] values = ussdCode.ToCharArray();
            ussdCode = "";
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                //Console.WriteLine("Hexadecimal value of {0} is {1}", letter, hexOutput);
                ussdCode += hexOutput;
            }
            return ussdCode;
        }
        #endregion

        #region Private routines

        #region Bytes

        private  byte[] GetInvertBytes(string source)
        {
            byte[] bytes = GetBytes(source);

            Array.Reverse(bytes);

            return bytes;
        }

        private  byte[] GetBytes(string source)
        {
            return GetBytes(source, 16);
        }

        private  byte[] GetBytes(string source, int fromBase)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < source.Length / 2; i++)
                bytes.Add(Convert.ToByte(source.Substring(i * 2, 2), fromBase));

            return bytes.ToArray();
        }

        #endregion

        #region Strings

        private  string RemoveLB(string source)
        {
            int index = 0;
            while (index >= 0)
            {
                index = source.IndexOf('\0');

                if (index < 0)
                    break;

                source = source.Remove(index, 1);
            }
            return source;
        }

        private  string GetInvertString(string source)
        {
            string output = "";
            int length = source.Length;

            for (int i = 0; i < length; i++)
            {
                output += source[length - 1 - i];
            }
            return output;
        }

        public string cutStringIMEI(string msg)
        {
            //int i = msg.IndexOf('\r');
            msg = msg.Substring(0, 15);
            return msg;
        }
        #endregion

        #endregion
    }

}
