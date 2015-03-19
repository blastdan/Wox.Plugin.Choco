using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace Wox.Plugin.Choco.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            if (!Directory.Exists(Parameters.TempImageFilePath))
            {
                Directory.CreateDirectory(Parameters.TempImageFilePath);
            }
        }

        [DeploymentItem("", "Images")]
        [DeploymentItem("", "Temp")]
        [TestMethod]
        public void QueryTest()
        {
            var main = new Main();
            var query = new Query("choco AIDA64");

            main.Query(query);
        }

        [TestMethod]
        public void WebTest()
        {
            var results = Web.Query("java");
        }

        [TestMethod]
        public void FileNameCleaning()
        {
            var result = FileUtilities.CleanFileName("helo / man * ( ) %@!");

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
