using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wox.Plugin.Choco.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [DeploymentItem("","Images")]
        [TestMethod]
        public void QueryTest()
        {
            var main = new Main();
            var query = new Query("choco java");

            main.Query(query);
        }

        [TestMethod]
        public void WebTest()
        {
            var results = Web.Query("java");
        }
    }
}
