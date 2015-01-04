using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Util.Crypto
{
    public class SHA1Provider
    {
        public static string ComputeHash(string plainText)
        {
            StringBuilder sb = new StringBuilder();

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            foreach (byte b in data)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}