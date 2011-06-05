using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DropNet.Tests
{
    [TestClass]
    public class WebAuthTests
    {
        [TestMethod]
        public void TestMethod1()
        {

            var client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);

            client.WebAuthUrl("test");

        }
    }
}
