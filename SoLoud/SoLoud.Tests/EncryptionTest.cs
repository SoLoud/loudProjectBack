using ContactHub.Helpers;
using NUnit.Framework;

namespace SoLoud.Tests
{
    class EncryptionTest
    {
        [Test]
        public void EncryptDecryptTestWithPassword()
        {
            var data = "some random data here";
            var pass = "yQa-WdxJ_6KhNf>a-1NRO+C^5>?HI4";

            var encryptedResult = AESThenHMAC.SimpleEncryptWithPassword(data, pass);
            var decryptedResult = AESThenHMAC.SimpleDecryptWithPassword(encryptedResult, pass);

            Assert.AreEqual(data, decryptedResult); 
        }
    }
}
