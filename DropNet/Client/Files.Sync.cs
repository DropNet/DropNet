#if !WINDOWS_PHONE

using System;
using System.Collections.Generic;
using System.IO;
using DropNet.Models;
using DropNet.Authenticators;
using System.Net;
using DropNet.Exceptions;
using RestSharp;
using RestSharp.Deserializers;

namespace DropNet
{
    public partial class DropNetClient
    {
        public MetaData GetMetaData()
        {
            return GetMetaData(string.Empty);
        }

        public MetaData GetMetaData(string path)
        {
			if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

		public List<MetaData> GetVersions(string path, int limit)
		{
			var request = _requestHelper.CreateVersionsRequest(path, Root, limit);
			
			return Execute<List<MetaData>>(ApiType.Base, request);
		}

        public List<MetaData> Search(string searchString)
        {
            return Search(searchString, string.Empty);
        }

        public List<MetaData> Search(string searchString, string path)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, Root);

            return Execute<List<MetaData>>(ApiType.Base, request);
        }

        //TODO - Make class for this to return (instead of just a byte[])
        public byte[] GetFile(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateGetFileRequest(path, Root);
            var response = Execute(ApiType.Content, request);

            return response.RawBytes;
        }

		//TODO - Make class for this to return (instead of just a byte[])
		public byte[] GetFile(string path, long startByte, long endByte, string rev)
		{
			if (!path.StartsWith("/"))
			{
				path = "/" + path;
			}

			var request = _requestHelper.CreateGetFileRequest(path, Root, startByte, endByte, rev);
			var response = Execute(ApiType.Content, request);

			return response.RawBytes;
		}

        public byte[] GetFileContentFromFS(FileInfo localFile)
        {
            //Get the file stream
            byte[] bytes;

            using (var fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    long numBytes = localFile.Length;
                    bytes = br.ReadBytes((int)numBytes);
                }

                fs.Close();
            }

            return bytes;
        }

        public MetaData UploadFilePUT(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateUploadFilePutRequest(path, filename, fileData, Root, overwrite, parentRevision);
            var response = _restClientContent.Execute<MetaData>(request);

            //TODO - Return something better here?
            return response.Data;
        }

        public MetaData UploadFile(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData, Root, overwrite, parentRevision);
            var response = _restClientContent.Execute<MetaData>(request);

            //TODO - Return something better here?
            return response.Data;
        }

        public MetaData UploadFile(string path, string filename, Stream stream, bool overwrite = true, string parentRevision = null)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateUploadFileRequest(path, filename, stream, Root, overwrite, parentRevision);
            var response = _restClientContent.Execute<MetaData>(request);

            //TODO - Return something better here?
            return response.Data;
        }

        public ChunkedUpload StartChunkedUpload(byte[] fileData)
        {
            var request = _requestHelper.CreateChunkedUploadRequest(fileData);
            var response = _restClientContent.Execute<ChunkedUpload>(request);
            return response.Data;
        }

        public ChunkedUpload AppendChunkedUpload(ChunkedUpload upload, byte[] fileData)
        {
            var request = _requestHelper.CreateAppendChunkedUploadRequest(upload, fileData);
            var response = _restClientContent.Execute<ChunkedUpload>(request);
            return response.Data;
        }

        public MetaData CommitChunkedUpload(ChunkedUpload upload, string path, bool overwrite = true, string parentRevision = null)
        {
            var request = _requestHelper.CreateCommitChunkedUploadRequest(upload, path, Root, overwrite, parentRevision);
            var response = _restClientContent.Execute<MetaData>(request);
            return response.Data;
        }

        public MetaData Delete(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateDeleteFileRequest(path, Root);
            return Execute<MetaData>(ApiType.Base, request);
        }

        public MetaData Copy(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/"))
            {
                fromPath = "/" + fromPath;
            }

            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        public MetaData CopyFromCopyRef(string fromCopyRef, string toPath)
        {
            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateCopyFileFromCopyRefRequest(fromCopyRef, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        public MetaData Move(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/"))
            {
                fromPath = "/" + fromPath;
            }

            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        public MetaData CreateFolder(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateCreateFolderRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        public ShareResponse GetShare(string path, bool shortUrl = true)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateShareRequest(path, Root, shortUrl);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        public ShareResponse GetMedia(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateMediaRequest(path, Root);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        public byte[] GetThumbnail(MetaData file)
        {
            return GetThumbnail(file.Path, ThumbnailSize.Small);
        }

        public byte[] GetThumbnail(MetaData file, ThumbnailSize size)
        {
            return GetThumbnail(file.Path, size);
        }

        public byte[] GetThumbnail(string path)
        {
            return GetThumbnail(path, ThumbnailSize.Small);
        }

        public byte[] GetThumbnail(string path, ThumbnailSize size)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateThumbnailRequest(path, size, Root);

            var response = Execute(ApiType.Content, request);

            return response.RawBytes;
        }

        public CopyRefResponse GetCopyRef(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateCopyRefRequest(path, Root);

            return Execute<CopyRefResponse>(ApiType.Base, request);
        }

        public DeltaPage GetDelta(string cursor)
        {
            var request = _requestHelper.CreateDeltaRequest(cursor);

            var deltaResponse =  Execute<DeltaPageInternal>(ApiType.Base, request);

            var deltaPage = new DeltaPage
                                {
                                    Cursor = deltaResponse.Cursor,
                                    Has_More = deltaResponse.Has_More,
                                    Reset = deltaResponse.Reset,
                                    Entries = new List<DeltaEntry>()
                                };

            foreach (var stringList in deltaResponse.Entries)
            {
                deltaPage.Entries.Add(StringListToDeltaEntry(stringList));
            }

            return deltaPage;
        }

        private DeltaEntry StringListToDeltaEntry(List<string> stringList)
        {
            var deltaEntry = new DeltaEntry
            {
                Path = stringList[0]
            };
            if (!String.IsNullOrEmpty(stringList[1]))
            {
                var jsonDeserializer = new JsonDeserializer();
                var fakeresponse = new RestResponse
                {
                    Content = stringList[1]
                };
                deltaEntry.MetaData = jsonDeserializer.Deserialize<MetaData>(fakeresponse);
            }
            return deltaEntry;
        } 

        private class DeltaPageInternal
        {
            public string Cursor { get; set; }
            public bool Has_More { get; set; }
            public bool Reset { get; set; }
            public List<List<string>> Entries { get; set; }
        }
    }
}
#endif
