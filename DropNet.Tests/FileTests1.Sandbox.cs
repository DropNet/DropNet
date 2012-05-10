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
    public class FileTests_Sandbox
    {
        DropNetClient _client;
        Fixture fixture;

        public FileTests_Sandbox()
        {
            _client = new DropNetClient(TestVariables.ApiKey_Sandbox, TestVariables.ApiSecret_Sandbox);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token_Sandbox, Secret = TestVariables.Secret_Sandbox };
            _client.UseSandbox = true;

            fixture = new Fixture();
        }

        [TestMethod]
        public void SANDBOX_Can_Get_MetaData_With_Special_Char()
        {
            var fileInfo = _client.GetMetaData("/test'.txt");
            
            Assert.IsNotNull(fileInfo);
        }


        [TestMethod]
        public void SANDBOX_Can_Get_File()
        {
            var fileInfo = _client.GetFile("/Sandbox.rtf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_File_Foreign_Language()
        {
            var rawBytes = _client.GetFile("/привет1.txt");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\привет1.txt", rawBytes);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_File_And_Save()
        {
            var fileInfo = _client.GetFile("/Sandbox.rtf");

            var writeStream = new FileStream("C:\\Temp\\Sandbox.rtf", FileMode.Create, FileAccess.Write);

            writeStream.Write(fileInfo, 0, fileInfo.Length);
            writeStream.Close();

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_PUT()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>() + ".txt");
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFilePUT("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>() + ".txt");
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_With_Special_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", "testfile's.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_With_International_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var uploaded = _client.UploadFile("/", "testПр.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_1MB_File()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

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
        public void SANDBOX_Can_Delete_File()
        {
            var deleted = _client.Delete("/Test.txt");

            Assert.IsNotNull(deleted);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_MetaData()
        {
            var metaData = _client.GetMetaData("/");

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
            Assert.AreEqual("app_folder", metaData.Root);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_MetaData_Root()
        {
            var metaData = _client.GetMetaData();

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        public void SANDBOX_Can_Create_Folder()
        {
            var metaData = _client.CreateFolder(string.Format("TestFolder1{0:yyyyMMddhhmmss}", DateTime.Now));

            Assert.IsNotNull(metaData);
        }

        [TestMethod]
        public void SANDBOX_Can_Shares()
        {
            _client.GetShare("/Sandbox.rtf");
        }

        [TestMethod]
        public void SANDBOX_Can_Get_Thumbnail()
        {
            var rawBytes = _client.GetThumbnail("/Test.png");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\TestSandbox.png", rawBytes);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_Delta()
        {
            var deltaPage = _client.GetDelta("");

            Assert.IsNotNull(deltaPage);

        }

    }
}
