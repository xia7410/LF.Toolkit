using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using System.Text;
using System.Security.Cryptography;
using LF.Toolkit.Common;

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
            foreach (var t in Enum.GetValues(typeof(HashAlgorithmType)))
            {
                Console.WriteLine(t + " --> " + HashAlgorithmProvider.ComputeHash((HashAlgorithmType)t, "123456", true));
            }

            string md5 = HashAlgorithmProvider.ComputeHash(HashAlgorithmType.MD5, "123456", true);
            Assert.IsTrue(md5.Equals("e10adc3949ba59abbe56e057f20f883e"));
        }

        [TestMethod]
        public void TestSymmetricAlgorithm()
        {
            var key = RandomStringGenerator.CreateRandomAlphanumeric(24);
            var input = "123456";

            foreach (var t in Enum.GetValues(typeof(SymmetricAlgorithmType)))
            {
                Console.WriteLine(t + " --> " + SymmetricAlgorithmProvider.EncryptToBase64((SymmetricAlgorithmType)t, input, key,
                    CipherMode.CBC, PaddingMode.ISO10126));
            }

            var aes = SymmetricAlgorithmProvider.EncryptToBase64(SymmetricAlgorithmType.Aes, input, key,
                 CipherMode.ECB, PaddingMode.PKCS7);
            var daes = SymmetricAlgorithmProvider.DecryptFromBase64(SymmetricAlgorithmType.Aes, Encoding.UTF8, aes, key
                , CipherMode.ECB, PaddingMode.PKCS7);

            Assert.IsTrue(daes.Equals("123456"));
        }

    }
}
