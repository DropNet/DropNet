using DropNet.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RestSharp;
using Ploeh.AutoFixture;

namespace DropNet.Tests
{
    
    
    /// <summary>
    ///This is a test class for RequestHelperTest and is intended
    ///to contain all RequestHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RequestHelperTest
    {
        private RequestHelper _target;
        private string _version;
        private Fixture fixture;

        public RequestHelperTest()
        {
            _version = "0";
            _target = new RequestHelper(_version);
            fixture = new Fixture();
        }

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
            string version = "0";
            RequestHelper _target = new RequestHelper(version);
            Assert.IsNotNull(_target);
        }

        /// <summary>
        ///A test for CreateAccountInfoRequest
        ///</summary>
        [TestMethod()]
        public void CreateAccountInfoRequestTest()
        {
            RestRequest actual = _target.CreateAccountInfoRequest();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 1);
            Assert.IsTrue(actual.Parameters[0].Name == "version");
            Assert.IsTrue(String.Equals(actual.Parameters[0].Value, _version));
            Assert.IsTrue(actual.Parameters[0].Type == ParameterType.UrlSegment);
        }

        /// <summary>
        ///A test for CreateCopyFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateCopyFileRequestTest()
        {
            string fromPath = fixture.CreateAnonymous<string>();
            string toPath = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateCopyFileRequest(fromPath, toPath, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 4);
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "version"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "from_path"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "to_path"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "root"));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "from_path").Value, fromPath));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "to_path").Value, toPath));
        }

        /// <summary>
        ///A test for CreateDeleteFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateDeleteFileRequestTest()
        {
            string path = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateDeleteFileRequest(path, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 3);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "path").Value, path));
        }

        /// <summary>
        ///A test for CreateGetFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateGetFileRequestTest()
        {
            string path = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateGetFileRequest(path, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 3);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "path").Value, path));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "path").Type == ParameterType.UrlSegment);
        }

        /// <summary>
        ///A test for CreateLoginRequest
        ///</summary>
        [TestMethod()]
        public void CreateLoginRequestTest()
        {
            string apiKey = fixture.CreateAnonymous<string>();
            string email = fixture.CreateAnonymous<string>();
            string password = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateLoginRequest(apiKey, email, password);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 4);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "oauth_consumer_key").Value, apiKey));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "email").Value, email));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "password").Value, password));
        }

        /// <summary>
        ///A test for CreateMetadataRequest
        ///</summary>
        [TestMethod()]
        public void CreateMetadataRequestTest()
        {
            string path = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateMetadataRequest(path, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 2);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "path").Value, path));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "path").Type == ParameterType.UrlSegment);
        }

        /// <summary>
        ///A test for CreateMoveFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateMoveFileRequestTest()
        {
            string fromPath = fixture.CreateAnonymous<string>();
            string toPath = fixture.CreateAnonymous<string>();
            RestRequest actual = _target.CreateMoveFileRequest(fromPath, toPath, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.GET);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 4);
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "version"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "from_path"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "to_path"));
            Assert.IsTrue(actual.Parameters.Exists(x => x.Name == "root"));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "from_path").Value, fromPath));
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "to_path").Value, toPath));
        }

        /// <summary>
        ///A test for CreateUploadFileRequest
        ///</summary>
        [TestMethod()]
        public void CreateUploadFileRequestTest()
        {
            string path = fixture.CreateAnonymous<string>();
            string filename = fixture.CreateAnonymous<string>();
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes(fixture.CreateAnonymous<string>());
            
            RestRequest actual = _target.CreateUploadFileRequest(path, filename, fileData, "dropbox");

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Method == Method.POST);
            Assert.IsNotNull(actual.Resource);
            Assert.IsNotNull(actual.Parameters);
            Assert.IsTrue(actual.Parameters.Count == 4);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "version").Value, _version));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "version").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "path").Value, path));
            Assert.IsTrue(actual.Parameters.Find(x => x.Name == "path").Type == ParameterType.UrlSegment);
            Assert.IsTrue(String.Equals(actual.Parameters.Find(x => x.Name == "file").Value, filename));
            Assert.IsTrue(actual.Files.Count == 1);
        }
    }
}
