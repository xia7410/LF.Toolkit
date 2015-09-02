using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using LF.Toolkit.Common;
using System.Text;
using System.Security.Cryptography;

namespace LF.Toolkit.UnitTests
{
    [TestClass]
    public class CommonTests
    {
        [TestMethod]
        public void TestDateTime()
        {
            DateTime weekday = new DateTime(2015, 9, 2);
            DateTime weekStart;
            DateTime weekEnd;
            DateTimeExtension.GetWeekRange(weekday, out weekStart, out weekEnd);
            Assert.IsTrue(weekStart == new DateTime(2015, 8, 31));
            Assert.IsTrue(weekEnd == new DateTime(2015, 9, 6));

            DateTime monthday = new DateTime(2012, 2, 10);
            DateTime monthStart;
            DateTime monthEnd;
            DateTimeExtension.GetMonthRange(monthday, out monthStart, out monthEnd);
            Assert.IsTrue(monthStart == new DateTime(2012, 2, 1));
            Assert.IsTrue(monthEnd == new DateTime(2012, 2, 29));
        }

        [TestMethod]
        public void TestHashAlgorithm()
        {
            string md5 = HashAlgorithmProvider.ComputeHash(HashAlgorithmType.MD5, "123456", true);

            Assert.IsTrue(md5.Equals("e10adc3949ba59abbe56e057f20f883e"));
        }

        [TestMethod]
        public void TestSymmetricAlgorithm()
        {
            var key = Encoding.UTF8.GetBytes("1234567890123456");
            var buffer = Encoding.UTF8.GetBytes("123456");
            var aes = SymmetricAlgorithmProvider.EncryptToBase64(SymmetricAlgorithmType.Aes, buffer, key,
                 CipherMode.CBC, PaddingMode.PKCS7);
            var daes = SymmetricAlgorithmProvider.DecryptFromBase64(SymmetricAlgorithmType.Aes, Convert.FromBase64String(aes), Encoding.UTF8,
                key, CipherMode.CBC, PaddingMode.PKCS7);

            Assert.IsTrue(daes.Equals("123456"));
        }

    }
}
