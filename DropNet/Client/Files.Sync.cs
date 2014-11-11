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

        /// <summary>
        /// Gets MetaData for the root folder.
        /// </summary>
        /// <returns></returns>
        public MetaData GetMetaData()
        {
            return GetMetaData(string.Empty);
        }

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <returns></returns>
        public MetaData GetMetaData(string path)
        {
			if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMetadataRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

		/// <summary>
		/// Gets List of MetaData for a File versions. Each metadata item contains info about file in certain version on Dropbox.
		/// </summary>
		/// <param name="path">The path of the file</param>
		/// <param name="limit">Maximal number of versions to fetch.</param>
		/// <returns></returns>
		public List<MetaData> GetVersions(string path, int limit)
		{
			var request = _requestHelper.CreateVersionsRequest(path, Root, limit);
			
			return Execute<List<MetaData>>(ApiType.Base, request);
		}

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        public List<MetaData> Search(string searchString)
        {
            return Search(searchString, string.Empty);
        }

        /// <summary>
        /// Restores a file path to a previous revision.
        /// </summary>
        /// <param name="rev">The revision of the file to restore.</param>
        /// <param name="path">The path to the file.</param>
        /// <returns>The metadata of the restored file.</returns>
        public MetaData Restore(string rev, string path)
        {
            var request = _requestHelper.CreateRestoreRequest(rev, path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        public List<MetaData> Search(string searchString, string path)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, Root);

            return Execute<List<MetaData>>(ApiType.Base, request);
        }

        //TODO - Make class for this to return (instead of just a byte[])
        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <returns>The files raw bytes</returns>
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
		/// <summary>
		/// Downloads a part of a File from dropbox given the path and a revision token.
		/// </summary>
		/// <param name="path">The path of the file to download</param>
		/// <param name="startByte">The index of the first byte to get.</param>
		/// <param name="endByte">The index of the last byte to get.</param>
		/// <param name="rev">Revision string as featured by <code>MetaData.Rev</code></param>
		/// <returns>The files raw bytes between <paramref name="startByte"/> and <paramref name="endByte"/>.</returns>
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

        /// <summary>
        /// Retrieve the content of a file in the local file system
        /// </summary>
        /// <param name="localFile">The local file to upload</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Uploads a File to Dropbox given the raw data. 
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="stream">The file stream</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Starts a chunked upload to Dropbox given a byte array.
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <returns>A object representing the chunked upload on success</returns>
        public ChunkedUpload StartChunkedUpload(byte[] fileData)
        {
            var request = _requestHelper.CreateChunkedUploadRequest(fileData);
            var response = _restClientContent.Execute<ChunkedUpload>(request);
            return response.Data;
        }

        /// <summary>
        /// Add data to a chunked upload given a byte array.
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="fileData">The file data</param>
        /// <returns>A object representing the chunked upload on success</returns>
        public ChunkedUpload AppendChunkedUpload(ChunkedUpload upload, byte[] fileData)
        {
            var request = _requestHelper.CreateAppendChunkedUploadRequest(upload, fileData);
            var response = _restClientContent.Execute<ChunkedUpload>(request);
            return response.Data;
        }

        /// <summary>
        /// Commit a completed chunked upload
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>A object representing the chunked upload on success</returns>
        public MetaData CommitChunkedUpload(ChunkedUpload upload, string path, bool overwrite = true, string parentRevision = null)
        {
            var request = _requestHelper.CreateCommitChunkedUploadRequest(upload, path, Root, overwrite, parentRevision);
            var response = _restClientContent.Execute<MetaData>(request);
            return response.Data;
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <returns></returns>
        public MetaData Delete(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateDeleteFileRequest(path, Root);
            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Copies a file or folder on Dropbox using a copy_ref as the source.
        /// </summary>
        /// <param name="fromCopyRef">Specifies a copy_ref generated from a previous /copy_ref call</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        public MetaData CopyFromCopyRef(string fromCopyRef, string toPath)
        {
            if (!toPath.StartsWith("/"))
            {
                toPath = "/" + toPath;
            }
            var request = _requestHelper.CreateCopyFileFromCopyRefRequest(fromCopyRef, toPath, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <returns>MetaData of the newly created folder</returns>
        public MetaData CreateFolder(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateCreateFolderRequest(path, Root);

            return Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShareResponse GetShare(string path, bool shortUrl = true)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateShareRequest(path, Root, shortUrl);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShareResponse GetMedia(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateMediaRequest(path, Root);

            return Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(MetaData file)
        {
            return GetThumbnail(file.Path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(MetaData file, ThumbnailSize size)
        {
            return GetThumbnail(file.Path, size);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(string path)
        {
            return GetThumbnail(path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path to the picture</param>
        /// <param name="size">The size to return the thumbnail</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates and returns a copy_ref to a file.
        /// 
        /// This reference string can be used to copy that file to another user's Dropbox by passing it in as the from_copy_ref parameter on /fileops/copy.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public CopyRefResponse GetCopyRef(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var request = _requestHelper.CreateCopyRefRequest(path, Root);

            return Execute<CopyRefResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// A long-poll endpoint to wait for changes on an account. In conjunction with /delta, this call gives you a low-latency way to monitor an account for file changes.
        /// </summary>
        /// <param name="cursor">The value returned from the prior call to GetDelta.</param>
        /// <param name="timeout">An optional integer indicating a timeout, in seconds.
        ///  The default value is 30 seconds, which is also the minimum allowed value. The maximum is 480 seconds.</param>
        /// <returns></returns>
        public LongpollDeltaResult GetLongpollDelta(string cursor, int timeout = 30)
        {
            var request = _requestHelper.CreateLongpollDeltaRequest(cursor, timeout);

            return Execute<LongpollDeltaResult>(ApiType.Notify, request);
        }

        /// <summary>
        /// Gets the deltas for a user's folders and files.
        /// </summary>
        /// <param name="cursor">The value returned from the prior call to GetDelta or an empty string</param>
        /// <returns></returns>
        public DeltaPage GetDelta(string cursor)
        {
            var request = _requestHelper.CreateDeltaRequest(cursor, null, null, false);

            var deltaResponse = Execute<DeltaPageInternal>(ApiType.Base, request);

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

        /// <summary>
        /// Gets the deltas for a user's folders and files.
        /// </summary>
        /// <param name="cursor">The value returned from the prior call to GetDelta or an empty string</param>
        /// <param name="pathPrefix">If present, this parameter filters the response to only include entries at or under the specified path</param>
        /// <param name="locale">If present the metadata returned will have its size field translated based on the given locale</param>
        /// <param name="includeMediaInfo">If true, each file will include a photo_info dictionary for photos and a video_info dictionary for videos with additional media info. When include_media_info is specified, files will only appear in delta responses when the media info is ready. If you use the include_media_info parameter, you must continue to pass the same value on subsequent calls using the returned cursor.</param>
        /// <returns></returns>
        public DeltaPage GetDelta(string cursor, string pathPrefix, string locale, bool includeMediaInfo)
        {
            if (!pathPrefix.StartsWith("/")) pathPrefix = "/" + pathPrefix;

            var request = _requestHelper.CreateDeltaRequest(cursor, pathPrefix, locale, includeMediaInfo);

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

        /// <summary>
        /// Helper function to convert a stringlist to a DeltaEntry object
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Private class used to deal with the DropBox API returning two different types in a given list
        /// </summary>
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
