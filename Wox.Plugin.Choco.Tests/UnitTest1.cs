using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using Wox.Plugin;

namespace Wox.Plugin.Choco.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
        }

        [DeploymentItem("", "Images")]
        [DeploymentItem("", "Temp")]
        [TestMethod]
        public void QueryTest()
        {
            var main = new Main();
            var qurey = new Wox.Plugin.Query();

            //query.ActionKeyword = "choco";
            //query.Terms = new string[] { "Git" };

            //var results = main.Query(query);
            //var resultCount = results.Count;
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
