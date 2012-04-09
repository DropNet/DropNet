using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace DropNet.Tests
{
    [TestClass]
    public class FileTaskTests
    {
        readonly DropNetClient _client;
        readonly Fixture _fixture;

        public FileTaskTests()
        {
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token, Secret = TestVariables.Secret };

            _fixture = new Fixture();
        }

        [TestMethod]
        public void Task_Get_MetaData()
        {
            var path = "/Test";
            var metaTask = _client.GetMetaDataTask(path);

            metaTask.Wait();

            Assert.IsNotNull(metaTask.Result);
            Assert.IsNotNull(metaTask.Result.Contents);
            Assert.AreEqual(0, string.Compare(path, metaTask.Result.Path, true));
        }

        [TestMethod]
        public void Task_Get_MetaData_With_Special_Char()
        {
            var path = "/Test/Getting'Started.rtf";
            var metaTask = _client.GetMetaDataTask(path);

            metaTask.Wait();

            Assert.IsNotNull(metaTask.Result);
            Assert.AreEqual(0, string.Compare(path, metaTask.Result.Path, true));
        }

        [TestMethod]
        public void Can_Get_Media()
        {
            var mediaTask = _client.GetMediaTask("/Test/WP_20120405_075015Z.mp4");

            mediaTask.Wait();

            Assert.IsNotNull(mediaTask.Result);
            Assert.IsNotNull(mediaTask.Result.Expires);
            Assert.IsNotNull(mediaTask.Result.Url);
        }

    }
}
