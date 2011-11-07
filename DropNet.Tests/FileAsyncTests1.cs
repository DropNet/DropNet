using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using DropNet.Exceptions;

namespace DropNet.Tests
{
    [TestClass]
    public class FileAsyncTests1
    {
        DropNetClient _client;
        Fixture fixture;

        public FileAsyncTests1()
        {
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token, Secret = TestVariables.Secret };

            fixture = new Fixture();
        }

        [TestMethod]
        public void Can_Get_MetaData_With_Special_Char_Async()
        {
            _client.GetMetaDataAsync("/Temp/test'.txt",
                (metaData) =>
                {
                    Assert.IsNotNull(metaData);
                },
                (error) =>
                {
                    Assert.IsNull(error);
                });
        }


        [TestMethod]
        public void Can_Upload_File_Async()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/", localFile.Name, content, Can_Upload_File_Async_Success, Can_Upload_File_Async_Failure);
        }

        [TestMethod]
        public void Can_Upload_File_Async_International_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/", "testПр.txt", content, Can_Upload_File_Async_Success, Can_Upload_File_Async_Failure);
        }

		[TestMethod]
		public void Can_Upload_File_Async_Streaming()
		{
			var localFile = new FileInfo (fixture.CreateAnonymous<string> ());
			var localContent = fixture.CreateAnonymous<string> ();

			File.WriteAllText (localFile.FullName, localContent, System.Text.Encoding.UTF8);
			Assert.IsTrue (File.Exists (localFile.FullName));
			byte[] content = _client.GetFileContentFromFS (localFile);

			var waitForUploadFinished = new ManualResetEvent (false);
			using (var fileStream = localFile.OpenRead ())
			{
				_client.UploadFileAsync ("/", localFile.Name, fileStream, 
					response => {
						Can_Upload_File_Async_Success (response);
						waitForUploadFinished.Set ();
					}, 
					response => {
						Can_Upload_File_Async_Failure (response);
						waitForUploadFinished.Set ();
					});
				waitForUploadFinished.WaitOne ();
			}

			//TODO - Delete
		}

		private void Can_Upload_File_Async_Success (RestSharp.RestResponse response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }
        private void Can_Upload_File_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Can_Upload_Large_File_Async()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            for (int i = 0; i < 16; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = _client.GetFileContentFromFS(localFile);

            _client.UploadFileAsync("/", localFile.Name, content, Can_Upload_Large_File_Async_Success, Can_Upload_Large_File_Async_Failure);

            //TODO - Delete
        }

        private void Can_Upload_Large_File_Async_Success(RestSharp.RestResponse response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
        }
        private void Can_Upload_Large_File_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void Can_Shares_Async()
        {
            _client.SharesAsync("/Android intro.pdf", (response) =>
            {
            },
            (error) =>
            {
            });
        }

        [TestMethod]
        public void Can_Get_Thumbnail_Async()
        {
            _client.ThumbnailsAsync("/Temp/Test.png", Can_Get_Thumbnail_Async_Success, Can_Get_Thumbnail_Async_Failure);
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
