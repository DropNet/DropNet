using DropNet.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RestSharp;

namespace DropNet.Tests
{
    
    
    /// <summary>
    ///This is a test class for RequestHelperTest and is intended
    ///to contain all RequestHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RequestHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RequestHelper Constructor
        ///</summary>
        [TestMethod()]
        public void RequestHelperConstructorTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for CreateAccountInfoRequest
        ///</summary>
        [TestMethod()]
        public void CreateAccountInfoRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateAccountInfoRequest();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateCopyFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateCopyFileRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string fromPath = string.Empty; // TODO: Initialize to an appropriate value
            string toPath = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateCopyFileRequest(fromPath, toPath);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateDeleteFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateDeleteFileRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateDeleteFileRequest(path);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateGetFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateGetFileRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateGetFileRequest(path);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateLoginRequest
        ///</summary>
        [TestMethod()]
        public void CreateLoginRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string apiKey = string.Empty; // TODO: Initialize to an appropriate value
            string email = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateLoginRequest(apiKey, email, password);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateMetadataRequest
        ///</summary>
        [TestMethod()]
        public void CreateMetadataRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateMetadataRequest(path);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateMoveFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateMoveFileRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string fromPath = string.Empty; // TODO: Initialize to an appropriate value
            string toPath = string.Empty; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateMoveFileRequest(fromPath, toPath);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateUploadFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateUploadFileRequestTest()
        {
            string version = string.Empty; // TODO: Initialize to an appropriate value
            RequestHelper target = new RequestHelper(version); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            byte[] fileData = null; // TODO: Initialize to an appropriate value
            RestRequest expected = null; // TODO: Initialize to an appropriate value
            RestRequest actual;
            actual = target.CreateUploadFileRequest(path, filename, fileData);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
