using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using DropNet.Exceptions;
using System;

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
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
            _client.Login(TestVariables.Email, TestVariables.Password);

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
        public void Can_Get_MetaData_With_Special_Char()
        {
            var fileInfo = _client.GetMetaData("/Temp/test'.txt");
            
            Assert.IsNotNull(fileInfo);
        }


        [TestMethod]
        public void Can_Get_File()
        {
            var fileInfo = _client.GetFile("/Getting Started.rtf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Get_File_And_Save()
        {
            var fileInfo = _client.GetFile("/Getting Started.rtf");

            var writeStream = new FileStream("C:\\Temp\\Getting Started.rtf", FileMode.Create, FileAccess.Write);

            writeStream.Write(fileInfo, 0, fileInfo.Length);
            writeStream.Close();

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Upload_File()
        {
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
        public void Can_Upload_File_With_Special_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", "testfile's.txt", content);

            Assert.IsTrue(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_Large_File()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            for (int i = 0; i < 17; i++)
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
        public void Can_Delete_File()
        {
            var deleted = _client.Delete("/Test.txt");

            Assert.IsNotNull(deleted);
        }

        [TestMethod]
        public void Can_Get_MetaData()
        {
            var metaData = _client.GetMetaData("/Public");

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        public void Can_Get_MetaData_Root()
        {
            var metaData = _client.GetMetaData();

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        public void Can_Create_Folder()
        {
            var metaData = _client.CreateFolder(string.Format("TestFolder1{0:yyyyMMddhhmmss}", DateTime.Now));

            Assert.IsNotNull(metaData);
        }

        [TestMethod]
        public void Can_Shares()
        {
            _client.Shares("/Android intro.pdf");
        }

        [TestMethod]
        public void Can_Get_Thumbnail()
        {
            var rawBytes = _client.Thumbnails("/Temp/Test.png");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\Test.png", rawBytes);
        }

    }
}
