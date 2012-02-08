using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;

namespace DropNet.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FileTests
    {
        readonly DropNetClient _client;
        readonly Fixture _fixture;

        public FileTests()
        {
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token, Secret = TestVariables.Secret };

            _fixture = new Fixture();
        }

        [TestMethod]
        public void Can_Get_MetaData_With_Special_Char()
        {
            var fileInfo = _client.GetMetaData("/Getting Started.pdf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Search()
        {
            var result = _client.Search("Getting", string.Empty);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1, "List is empty");
        }


        [TestMethod]
        public void Can_Get_File()
        {
            var fileInfo = _client.GetFile("/Getting Started.pdf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Get_File_Foreign_Language()
        {
            var rawBytes = _client.GetFile("/привет.txt");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\привет.txt", rawBytes);
        }

        [TestMethod]
        public void Can_Get_File_And_Save()
        {
            var fileInfo = _client.GetFile("/Getting Started.pdf");

            var writeStream = new FileStream("C:\\Temp\\Getting Started.pdf", FileMode.Create, FileAccess.Write);

            writeStream.Write(fileInfo, 0, fileInfo.Length);
            writeStream.Close();

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void Can_Upload_File_PUT()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>() + ".txt");
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFilePUT("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_File()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>() + ".txt");
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/Test", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_File_With_Special_Char()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", "testfile's.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_File_With_International_Char()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", "testПр.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void Can_Upload_1MB_File()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            //Make a 1MB file...
            for (int i = 0; i < 15; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
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
            _client.GetShare("/Getting Started.pdf");
        }

        [TestMethod]
        public void Can_Get_Thumbnail()
        {
            var rawBytes = _client.GetThumbnail("/Temp/Test.png");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\Test.png", rawBytes);
        }

        [TestMethod]
        public void Can_Get_Media()
        {
            var mediaLink = _client.GetMedia("/Test/WP_20120111_011610Z.mp4");

            Assert.IsNotNull(mediaLink);
            Assert.IsNotNull(mediaLink.Expires);
            Assert.IsNotNull(mediaLink.Url);
        }

    }
}
