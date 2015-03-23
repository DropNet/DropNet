using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DropNet.Authenticators;
using DropNet.Exceptions;
using DropNet.Models;
using RestSharp;

namespace DropNet
{
    public interface IDropNetClient
    {
        /// <summary>
        /// Contains the Users Token and Secret
        /// </summary>
        UserLogin UserLogin { get; set; }

        /// <summary>
        /// To use Dropbox API in sandbox mode (app folder access) set to true
        /// </summary>
        bool UseSandbox { get; set; }

#if !WINDOWS_PHONE
        IWebProxy Proxy { get; set; }
#endif

        /// <summary>
        /// Helper Method to Build up the Url to authorize a Token/Secret
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        string BuildAuthorizeUrl(string callback = null);

        /// <summary>
        /// Helper Method to Build up the Url to authorize a Token/Secret
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        string BuildAuthorizeUrl(UserLogin userLogin, string callback = null);

        /// <summary>
        /// This starts the OAuth 2.0 authorization flow. This isn't an API call—it's the web page that lets the user sign in to Dropbox and authorize your app. The user must be redirected to the page over HTTPS and it should be presented to the user through their web browser. After the user decides whether or not to authorize your app, they will be redirected to the URL specified by the 'redirectUri'.
        /// </summary>
        /// <param name="oAuth2AuthorizationFlow">The type of authorization flow to use.  See the OAuth2AuthorizationFlow enum documentation for more information.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI registered in the app console (https://www.dropbox.com/developers/apps), though localhost and 127.0.0.1 are always accepted. A redirect URI is required for a token flow, but optional for code. If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter the information in your app.</param>
        /// <param name="state">Arbitrary data that will be passed back to your redirect URI. This parameter can be used to track a user through the authorization flow in order to prevent cross-site request forgery (CRSF) attacks.</param>
        /// <returns>A URL to which your app should redirect the user for authorization.  After the user authorizes your app, they will be sent to your redirect URI. The type of response varies based on the 'oauth2AuthorizationFlow' argument.  .</returns>
        string BuildAuthorizeUrl(OAuth2AuthorizationFlow oAuth2AuthorizationFlow, string redirectUri, string state = null);

        /// <summary>
        /// Gets MetaData for the root folder.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        void GetMetaDataAsync(Action<MetaData> success, Action<DropboxException> failure, String hash = null, Boolean list = false, Boolean include_deleted = false);

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetMetaDataAsync(String path, Action<MetaData> success, Action<DropboxException> failure, String hash = null, Boolean list = false, Boolean include_deleted = false);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        void SearchAsync(string searchString, Action<List<MetaData>> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="fileLimit">The maximum and default value is 1,000. No more than <code>fileLimit</code> search results will be returned.</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        void SearchAsync(string searchString, int fileLimit, Action<List<MetaData>> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        void SearchAsync(string searchString, string path, Action<List<MetaData>> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="fileLimit">The maximum and default value is 1,000. No more than <code>fileLimit</code> search results will be returned.</param>
        /// <param name="success">Success call back</param>
        /// <param name="failure">Failure call back </param>
        void SearchAsync(string searchString, string path, int fileLimit, Action<List<MetaData>> success, Action<DropboxException> failure);

        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetFileAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Downloads a part of a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <param name="startByte">The index of the first byte to get.</param>
        /// <param name="endByte">The index of the last byte to get.</param>
        /// <param name="rev">Revision of the file</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetFileAsync(string path, long startByte, long endByte, string rev, Action<IRestResponse> success, Action<DropboxException> failure);

#if !WINDOWS_PHONE && !MONOTOUCH
        /// <summary>
        /// Uploads a File to Dropbox from the local file system to the specified folder
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="localFile">The local file to upload</param>/// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        void UploadFileAsync(string path, FileInfo localFile, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null);
#endif

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        void UploadFileAsync(string path, string filename, byte[] fileData, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileStream">The file data</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        void UploadFileAsync(string path, string filename, Stream fileStream, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Uploads a File to Dropbox in chunks that are assembled into a single file when finished.
        /// </summary>
        /// <param name="chunkNeeded">The callback function that returns a byte array given an offset</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        /// <param name="progress">The optional callback Action that receives upload progress</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <param name="fileSize">The total size of the file if available</param>
        /// <param name="maxRetries">The number of times to retry uploading if a chunk fails, unlimited if null.</param>
        void UploadChunkedFileAsync(Func<long, byte[]> chunkNeeded, string path, Action<MetaData> success, Action<DropboxException> failure, Action<ChunkedUploadProgress> progress = null, bool overwrite = true, string parentRevision = null, long? fileSize = null, long? maxRetries = null);

        /// <summary>
        /// Starts a chunked upload to Dropbox given a byte array.
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        void StartChunkedUploadAsync(byte[] fileData, Action<ChunkedUpload> success, Action<DropboxException> failure);

        /// <summary>
        /// Add data to a chunked upload given a byte array.
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="fileData">The file data</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        void  AppendChunkedUploadAsync(ChunkedUpload upload, byte[] fileData, Action<ChunkedUpload> success, Action<DropboxException> failure);

        /// <summary>
        /// Commit a completed chunked upload
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="success">The callback Action to perform on completion</param>
        /// <param name="failure">The callback Action to perform on exception</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        void CommitChunkedUploadAsync(ChunkedUpload upload, string path, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void DeleteAsync(string path, Action<IRestResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void CopyAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Copies a file or folder on Dropbox using a copy_ref as the source.
        /// </summary>
        /// <param name="fromCopyRef">Specifies a copy_ref generated from a previous /copy_ref call</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void CopyFromCopyRefAsync(string fromCopyRef, string toPath, Action<IRestResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void MoveAsync(string fromPath, string toPath, Action<IRestResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void CreateFolderAsync(string path, Action<MetaData> success, Action<DropboxException> failure);

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetShareAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="shortUrl">True to shorten the share url </param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetShareAsync(string path, bool shortUrl, Action<ShareResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetMediaAsync(string path, Action<ShareResponse> success, Action<DropboxException> failure);

        /// <summary>
        /// The beta delta function, gets updates for a given folder
        /// </summary>
        /// <param name="IKnowThisIsBetaOnly"></param>
        /// <param name="path"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        void GetDeltaAsync(bool IKnowThisIsBetaOnly, string path, Action<DeltaPage> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The MetaData</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetThumbnailAsync(MetaData file, Action<byte[]> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The metadat file</param>
        /// <param name="size">Thumbnail size</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">Failure callback</param>
        void GetThumbnailAsync(MetaData file, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">failure callback</param>
        void GetThumbnailAsync(string path, Action<byte[]> success, Action<DropboxException> failure);

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="size">Thumbnail size</param>
        /// <param name="success">success callback</param>
        /// <param name="failure">failure callback</param>
        void GetThumbnailAsync(string path, ThumbnailSize size, Action<byte[]> success, Action<DropboxException> failure);

        /// <summary>
        /// Creates and returns a copy_ref to a file.
        /// 
        /// This reference string can be used to copy that file to another user's Dropbox by passing it in as the from_copy_ref parameter on /fileops/copy.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        void GetCopyRefAsync(string path, Action<CopyRefResponse> success, Action<DropboxException> failure);

        Task<MetaData> GetMetaDataTask(String hash = null, Boolean list = false, Boolean include_deleted = false);
        Task<MetaData> GetMetaDataTask(String path, String hash = null, Boolean list = false, Boolean include_deleted = false);
        Task<List<MetaData>> SearchTask(string searchString);
        Task<List<MetaData>> SearchTask(string searchString, int fileLimit);
        Task<List<MetaData>> SearchTask(string searchString, string path, int fileLimit);
        Task<IRestResponse> GetFileTask(string path);
        Task<MetaData> UploadFileTask(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null);
        Task<MetaData> UploadFileTask(string path, string filename, Stream fileStream, bool overwrite = true, string parentRevision = null);
        Task<IRestResponse> DeleteTask(string path);
        Task<IRestResponse> CopyTask(string fromPath, string toPath);
        Task<IRestResponse> CopyFromCopyRefTask(string fromCopyRef, string toPath);
        Task<IRestResponse> MoveTask(string fromPath, string toPath);
        Task<MetaData> CreateFolderTask(string path, Action<MetaData> success, Action<DropboxException> failure);
        Task<ShareResponse> GetShareTask(string path, bool shortUrl = true);
        Task<ShareResponse> GetMediaTask(string path);
        Task<IRestResponse> GetThumbnailTask(MetaData file);
        Task<IRestResponse> GetThumbnailTask(MetaData file, ThumbnailSize size);
        Task<IRestResponse> GetThumbnailTask(string path);
        Task<IRestResponse> GetThumbnailTask(string path, ThumbnailSize size);
        Task<CopyRefResponse> GetCopyRefTask(string path);

#if !WINDOWS_PHONE
        /// <summary>
        /// Gets MetaData for the root folder.
        /// </summary>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        /// <param name="list">If true, the folder's metadata will include a contents field with a list of metadata entries for the contents of the folder. If false, the contents field will be omitted.</param>
        /// <param name="include_deleted">Only applicable when list is set. If this parameter is set to true, then contents will include the metadata of deleted children. Note that the target of the metadata call is always returned even when it has been deleted (with is_deleted set to true) regardless of this flag.</param>
        /// <returns></returns>
        MetaData GetMetaData(String hash = null, Boolean list = false, Boolean include_deleted = false);

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        /// <param name="list">If true, the folder's metadata will include a contents field with a list of metadata entries for the contents of the folder. If false, the contents field will be omitted.</param>
        /// <param name="include_deleted">Only applicable when list is set. If this parameter is set to true, then contents will include the metadata of deleted children. Note that the target of the metadata call is always returned even when it has been deleted (with is_deleted set to true) regardless of this flag.</param>
        /// <returns></returns>
        MetaData GetMetaData(String path, String hash = null, Boolean list = false, Boolean include_deleted = false);

        /// <summary>
        /// Gets List of MetaData for a File versions. Each metadata item contains info about file in certain version on Dropbox.
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="limit">Maximal number of versions to fetch.</param>
        /// <returns></returns>
        List<MetaData> GetVersions(string path, int limit);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        List<MetaData> Search(string searchString);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="fileLimit">The maximum and default value is 1,000. No more than <code>fileLimit</code> search results will be returned.</param>
        List<MetaData> Search(string searchString, int fileLimit);

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="fileLimit">The maximum and default value is 1,000. No more than <code>fileLimit</code> search results will be returned.</param>
        List<MetaData> Search(string searchString, string path, int fileLimit);

        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <returns>The files raw bytes</returns>
        byte[] GetFile(string path);

        /// <summary>
        /// Downloads a part of a File from dropbox given the path and a revision token.
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <param name="startByte">The index of the first byte to get.</param>
        /// <param name="endByte">The index of the last byte to get.</param>
        /// <param name="rev">Revision string as featured by <code>MetaData.Rev</code></param>
        /// <returns>The files raw bytes between <paramref name="startByte"/> and <paramref name="endByte"/>.</returns>
        byte[] GetFile(string path, long startByte, long endByte, string rev);

        /// <summary>
        /// Retrieve the content of a file in the local file system
        /// </summary>
        /// <param name="localFile">The local file to upload</param>
        /// <returns>True on success</returns>
        byte[] GetFileContentFromFS(FileInfo localFile);

        /// <summary>
        /// Uploads a File to Dropbox given the raw data. 
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
        MetaData UploadFilePUT(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
        MetaData UploadFile(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="stream">The file stream</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>True on success</returns>
        MetaData UploadFile(string path, string filename, Stream stream, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Starts a chunked upload to Dropbox given a byte array.
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <returns>A object representing the chunked upload on success</returns>
        ChunkedUpload StartChunkedUpload(byte[] fileData);

        /// <summary>
        /// Add data to a chunked upload given a byte array.
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="fileData">The file data</param>
        /// <returns>A object representing the chunked upload on success</returns>
        ChunkedUpload AppendChunkedUpload(ChunkedUpload upload, byte[] fileData);

        /// <summary>
        /// Commit a completed chunked upload
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <returns>A object representing the chunked upload on success</returns>
        MetaData CommitChunkedUpload(ChunkedUpload upload, string path, bool overwrite = true, string parentRevision = null);

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        /// <returns></returns>
        MetaData Delete(string path);

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        MetaData Copy(string fromPath, string toPath);

        /// <summary>
        /// Copies a file or folder on Dropbox using a copy_ref as the source.
        /// </summary>
        /// <param name="fromCopyRef">Specifies a copy_ref generated from a previous /copy_ref call</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        /// <returns>True on success</returns>
        MetaData CopyFromCopyRef(string fromCopyRef, string toPath);

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        /// <returns>True on success</returns>
        MetaData Move(string fromPath, string toPath);

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        /// <returns>MetaData of the newly created folder</returns>
        MetaData CreateFolder(string path);

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ShareResponse GetShare(string path, bool shortUrl = true);

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ShareResponse GetMedia(string path);

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        byte[] GetThumbnail(MetaData file);

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        byte[] GetThumbnail(MetaData file, ThumbnailSize size);

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        byte[] GetThumbnail(string path);

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path to the picture</param>
        /// <param name="size">The size to return the thumbnail</param>
        /// <returns></returns>
        byte[] GetThumbnail(string path, ThumbnailSize size);

        /// <summary>
        /// Creates and returns a copy_ref to a file.
        /// 
        /// This reference string can be used to copy that file to another user's Dropbox by passing it in as the from_copy_ref parameter on /fileops/copy.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        CopyRefResponse GetCopyRef(string path);

        /// <summary>
        /// Gets the deltas for a user's folders and files.
        /// </summary>
        /// <param name="cursor">The value returned from the prior call to GetDelta or an empty string</param>
        /// <returns></returns>
        DeltaPage GetDelta(string cursor);

        /// <summary>
        /// Shorthand method to get an OAuth1 token from Dropbox and build the Url to authorize it.
        /// </summary>
        /// <returns></returns>
        string GetTokenAndBuildUrl(string callback = null);

        /// <summary>
        ///     Provisions an OAuth1 token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        /// <returns></returns>
        UserLogin GetToken();

        /// <summary>
        ///     Authorizes the previously-requested OAuth1 token
        /// </summary>
        /// <returns></returns>
        UserLogin GetAccessToken();

        /// <summary>
        ///     Acquire an OAuth2 bearer token once the user has authorized the app.  This endpoint only applies to apps using the AuthorizationFlow.Code flow.
        /// </summary>
        /// <param name="code">The authorization code provided by Dropbox when the user was redirected back to your site.</param>
        /// <param name="redirectUri">The redirect Uri for your site. This is only used to validate that it matches the original /oauth2/authorize; the user will not be redirected again.</param>
        /// <returns>An OAuth2 bearer token.</returns>
        UserLogin GetAccessToken(string code, string redirectUri);

        AccountInfo AccountInfo();
#endif

        /// <summary>
        /// Gets a token from the almightly dropbox.com (Token cant be used until authorized!)
        /// </summary>
        void GetTokenAsync(Action<UserLogin> success, Action<DropboxException> failure);

        /// <summary>
        /// Converts a request token into an Access token after the user has authorized access via dropbox.com
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure);

        /// <summary>
        ///     Acquire an OAuth2 bearer token once the user has authorized the app.  This endpoint only applies to apps using the AuthorizationFlow.Code flow.
        /// </summary>
        /// <param name="success">Action to perform with the OAuth2 access token</param>
        /// <param name="failure"></param>
        /// <param name="code">The authorization code provided by Dropbox when the user was redirected back to your site.</param>
        /// <param name="redirectUri">The redirect Uri for your site. This is only used to validate that it matches the original /oauth2/authorize; the user will not be redirected again.</param>
        void GetAccessTokenAsync(Action<UserLogin> success, Action<DropboxException> failure, string code, string redirectUri);

        /// <summary>
        /// Gets AccountInfo
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        void AccountInfoAsync(Action<AccountInfo> success, Action<DropboxException> failure);

        [Obsolete("No longer supported by Dropbox")]
        void CreateAccountAsync(string email, string firstName, string lastName, string password, Action<RestResponse> success, Action<DropboxException> failure);
    }
}