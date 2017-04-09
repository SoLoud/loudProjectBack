using System.Security.Cryptography;
using System.Text;

namespace ContactHub.Helpers
{
    public class Hasher
    {
        public static string CalculateMD5(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string CalculateHMACsha256(string input, string key)
        {
            string returnValue;
            var keyByte = Encoding.UTF8.GetBytes(key);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                returnValue = ByteToString(hmacsha256.Hash);
            }

            return returnValue;
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }
    }
}