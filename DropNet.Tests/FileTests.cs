using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;

namespace DropNet.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FileTests
    {
        DropNetClient _client;
        Fixture fixture;

        public FileTests()
        {
            //
            // TODO: Add constructor logic here
            //
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Can_Get_File()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var fileInfo = _client.GetFile("/Getting Started.rtf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Get_File_And_Save()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var fileInfo = _client.GetFile("/Temp/ScreenShot11.Png");

            var writeStream = new FileStream("C:\\Temp\\ScreenShot11.Png", FileMode.Create, FileAccess.Write);

            writeStream.Write(fileInfo, 0, fileInfo.Length);
            writeStream.Close();

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Upload_File()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);

            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsTrue(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_Large_File()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);

            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            for (int i = 0; i < 16; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsTrue(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_File_Async()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);

            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/", localFile.Name, content, Can_Upload_File_Async_Callback);

            //TODO - Delete
        }

        private void Can_Upload_File_Async_Callback(RestSharp.RestResponse response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void Can_Upload_Large_File_Async()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);

            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            for (int i = 0; i < 16; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/", localFile.Name, content, Can_Upload_Large_File_Async_Callback);

            //TODO - Delete
        }

        private void Can_Upload_Large_File_Async_Callback(RestSharp.RestResponse response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void Can_Delete_File()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var deleted = _client.Delete("/Test.txt");

            Assert.IsNotNull(deleted);
        }

        [TestMethod]
        public void Can_Get_MetaData()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var metaData = _client.GetMetaData("/Public");

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        public void Can_Get_MetaData_Root()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var metaData = _client.GetMetaData();

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        public void Can_Create_Folder()
        {
            _client.Login(TestVariables.Email, TestVariables.Password);
            var metaData = _client.CreateFolder("TestFolder1");

            Assert.IsNotNull(metaData);
        }

    }
}
