using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using DropNet.Exceptions;
using DropNet.Models;

namespace DropNet.Tests
{
    [TestClass]
    public class FileAsyncTests
    {
        readonly DropNetClient _client;
        readonly Fixture _fixture;

        public FileAsyncTests()
        {
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token, Secret = TestVariables.Secret };

            _fixture = new Fixture();
        }

        [TestMethod]
        public void Can_Get_MetaData_With_Special_Char_Async()
        {
            _client.GetMetaDataAsync("/Temp/test'.txt",
                Assert.IsNotNull,
                Assert.IsNull);
        }

        [TestMethod]
        public void Can_Get_List_Of_Metadata_For_Search_String()
        {
            _client.SearchAsync("Getting", s =>
                                               {
                                                   Assert.IsNotNull(s);
                                                   Assert.AreEqual(1, s.Count);
                                               }, 
                                               Assert.IsNull);
        }

        [TestMethod]
        public void Can_Upload_File_Async()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/Test", localFile.Name, content, Can_Upload_File_Async_Success, Can_Upload_File_Async_Failure);
        }

        [TestMethod]
        public void Can_Upload_File_Async_International_Char()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/Test", "testПр1.txt", content, Can_Upload_File_Async_Success, Can_Upload_File_Async_Failure);
        }

        [TestMethod]
        public void Can_Upload_File_Async_Streaming()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            var waitForUploadFinished = new ManualResetEvent(false);
            using (var fileStream = localFile.OpenRead())
            {
                _client.UploadFileAsync("/Test", localFile.Name, fileStream,
                    response =>
                    {
                        Can_Upload_File_Async_Success(response);
                        waitForUploadFinished.Set();
                    },
                    response =>
                    {
                        Can_Upload_File_Async_Failure(response);
                        waitForUploadFinished.Set();
                    });
                waitForUploadFinished.WaitOne();
            }

            //TODO - Delete
        }

        private void Can_Upload_File_Async_Success(MetaData metadata)
        {
            Assert.IsNotNull(metadata);
        }
        private void Can_Upload_File_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Can_Upload_Large_File_Async()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            var localContent = _fixture.CreateAnonymous<string>();

            for (int i = 0; i < 16; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/Test", localFile.Name, content, Can_Upload_Large_File_Async_Success, Can_Upload_Large_File_Async_Failure);

            //TODO - Delete
        }

        private void Can_Upload_Large_File_Async_Success(MetaData metadata)
        {
            Assert.IsNotNull(metadata);
        }
        private void Can_Upload_Large_File_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Can_Shares_Async()
        {
            _client.GetShareAsync("/Test/Getting Started.rtf", response =>
            {
            },
            error =>
            {
            });
        }

        [TestMethod]
        public void Can_Get_Thumbnail_Async()
        {
            _client.GetThumbnailAsync("/Temp/Test.png", Can_Get_Thumbnail_Async_Success, Can_Get_Thumbnail_Async_Failure);
        }

        public void Can_Get_Thumbnail_Async_Success(byte[] rawBytes)
        {
            Assert.IsNotNull(rawBytes);
            //Save to disk for validation
            File.WriteAllBytes(@"C:\Temp\Test.png", rawBytes);
        }
        private void Can_Get_Thumbnail_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

    }
}
