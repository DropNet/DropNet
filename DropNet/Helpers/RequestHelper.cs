using System;
using System.IO;
using DropNet.Authenticators;
using RestSharp;
using DropNet.Models;

namespace DropNet.Helpers
{
    /// <summary>
    /// Helper class for creating DropNet RestSharp Requests
    /// </summary>
    public class RequestHelper
    {       
        private readonly string _version;

        public RequestHelper(string version)
        {
            _version = version;
        }

        public RestRequest CreateMetadataRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/metadata/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

		public RestRequest CreateVersionsRequest(string path, string root, int rev_limit)
		{
			var request = new RestRequest(Method.GET);
			request.Resource = "{version}/revisions/{root}{path}";
			request.AddParameter("version", _version, ParameterType.UrlSegment);
			request.AddParameter("path", path, ParameterType.UrlSegment);
			request.AddParameter("root", root, ParameterType.UrlSegment);
			request.AddParameter("rev_limit", rev_limit, ParameterType.UrlSegment);

			return request;
		}

        public RestRequest CreateShareRequest(string path, string root, bool shortUrl)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/shares/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("short_url", shortUrl);

            return request;
        }

        public RestRequest CreateMediaRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/media/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateCopyRefRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/copy_ref/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateGetFileRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/files/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

		public RestRequest CreateGetFileRequest(string path, string root, long startByte, long endByte, string rev)
		{
			var request = new RestRequest(Method.GET);
			request.Resource = "{version}/files/{root}{path}";
			request.AddParameter("version", _version, ParameterType.UrlSegment);
			request.AddParameter("path", path, ParameterType.UrlSegment);
			request.AddParameter("root", root, ParameterType.UrlSegment);
			request.AddParameter("rev", rev, ParameterType.UrlSegment);
			request.AddHeader("Range", "bytes=" + startByte + "-" + endByte);

			return request;
		}

		public RestRequest CreateUploadFileRequest(string path, string filename, byte[] fileData, string root)
		{
			var request = new RestRequest(Method.POST);
			request.Resource = "{version}/files/{root}{path}";
			request.AddParameter("version", _version, ParameterType.UrlSegment);
			request.AddParameter("path", path, ParameterType.UrlSegment);
			request.AddParameter("root", root, ParameterType.UrlSegment);
			//Need to add the "file" parameter with the file name
            // This isn't needed. Dropbox is particular about the ordering,
            // but the oauth sig only needs the filename, which we have in the OTHER parameter
			//request.AddParameter("file", filename);

			request.AddFile("file", fileData, filename);

			return request;
		}

		public RestRequest CreateUploadFilePutRequest(string path, string filename, byte[] fileData, string root)
		{
			var request = new RestRequest(Method.PUT);
            //Need to put the OAuth Parmeters in the Resource to get around them being put in the body
            request.Resource = "{version}/files_put/{root}{path}?file={file}&oauth_consumer_key={oauth_consumer_key}&oauth_nonce={oauth_nonce}";
            request.Resource += "&oauth_token={oauth_token}&oauth_timestamp={oauth_timestamp}";
            request.Resource += "&oauth_signature={oauth_signature}&oauth_signature_method={oauth_signature_method}&oauth_version={oauth_version}";
			request.AddParameter("version", _version, ParameterType.UrlSegment);
			request.AddParameter("path", path, ParameterType.UrlSegment);
			request.AddParameter("root", root, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            request.AddParameter("file", filename, ParameterType.UrlSegment);

            request.AddParameter("file", fileData, ParameterType.RequestBody);

			return request;
		}

        public RestRequest CreateUploadFileRequest(string path, string filename, Stream fileStream, string root)
		{
			var request = new RestRequest(Method.POST);
            //Don't want these to timeout (Maybe use something better here?)
            request.Timeout = int.MaxValue;
            request.Resource = "{version}/files/{root}{path}";
			request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
			//Need to add the "file" parameter with the file name
            // This isn't needed. Dropbox is particular about the ordering,
            // but the oauth sig only needs the filename, which we have in the OTHER parameter
			//request.AddParameter("file", filename);

			request.AddFile("file", s => StreamUtils.CopyStream(fileStream, s), filename);

			return request;
		}

        public RestRequest CreateChunkedUploadRequest(byte[] fileData)
        {
            var request = new RestRequest(Method.PUT);
            request.Resource = "{version}/chunked_upload?oauth_consumer_key={oauth_consumer_key}&oauth_nonce={oauth_nonce}";
            request.Resource += "&oauth_token={oauth_token}&oauth_timestamp={oauth_timestamp}";
            request.Resource += "&oauth_signature={oauth_signature}&oauth_signature_method={oauth_signature_method}&oauth_version={oauth_version}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("file", fileData, ParameterType.RequestBody);

            return request;
        }

        public RestRequest CreateAppendChunkedUploadRequest(ChunkedUpload upload, byte[] fileData)
        {
            var request = new RestRequest(Method.PUT);
            request.Resource = "{version}/chunked_upload?upload_id={upload_id}&offset={offset}&oauth_consumer_key={oauth_consumer_key}&oauth_nonce={oauth_nonce}";
            request.Resource += "&oauth_token={oauth_token}&oauth_timestamp={oauth_timestamp}";
            request.Resource += "&oauth_signature={oauth_signature}&oauth_signature_method={oauth_signature_method}&oauth_version={oauth_version}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("upload_id", upload.UploadId, ParameterType.UrlSegment);
            request.AddParameter("offset", upload.Offset, ParameterType.UrlSegment);

            request.AddParameter("file", fileData, ParameterType.RequestBody);

            return request;
        }

        public RestRequest CreateCommitChunkedUploadRequest(ChunkedUpload upload, string path, string root)
        {
            var request = new RestRequest(Method.POST);
            request.Resource = "{version}/commit_chunked_upload/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            request.AddParameter("overwrite", true);
            request.AddParameter("upload_id", upload.UploadId);

            return request;
        }

        public RestRequest CreateDeleteFileRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/delete";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateCopyFileRequest(string fromPath, string toPath, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/copy";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateCopyFileFromCopyRefRequest(string fromCopyRef, string toPath, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/copy";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_copy_ref", fromCopyRef);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateMoveFileRequest(string fromPath, string toPath, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/move";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateLoginRequest(string apiKey, string email, string password)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", apiKey);

            request.AddParameter("email", email);

            request.AddParameter("password", password);

            return request;
        }

        public RestRequest CreateTokenRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/request_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateAccessTokenRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/access_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateAccessToken()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/access_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            return request;
        }

        public RestRequest CreateNewAccountRequest(string apiKey, string email, string firstName, string lastName, string password)
        {
            var request = new RestRequest(Method.POST);
            request.Resource = "{version}/account";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", apiKey);

            request.AddParameter("email", email);
            request.AddParameter("first_name", firstName);
            request.AddParameter("last_name", lastName);
            request.AddParameter("password", password);

            return request;
        }

        public RestRequest BuildAuthorizeUrl(string userToken, string callback = null)
        {
            if (string.IsNullOrWhiteSpace(userToken))
            {
                throw new ArgumentNullException("userToken");
            }

            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/authorize";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("oauth_token", userToken);
            if (!string.IsNullOrWhiteSpace(callback))
            {
                request.AddParameter("oauth_callback", callback);
            }

            return request;
        }

        /// <summary>
        /// This is the first step the OAuth 2.0 authorization flow. This isn't an API call—it's the web page that lets the user sign in to Dropbox and authorize your app. The user must be redirected to the page over HTTPS and it should be presented to the user through their web browser. After the user decides whether or not to authorize your app, they will be redirected to the URL specified by redirect_uri.
        /// </summary>
        /// <param name="oAuth2AuthorizationFlow">The type of authorization flow to use.  See AuthorizationFlow documentation for descriptions of each.</param>
        /// <param name="consumerKey"></param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI registered in the app console, though localhost and 127.0.0.1 are always accepted. A redirect URI is required for a token flow, but optional for code. If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter the information in your app.</param>
        /// <param name="state">Arbitrary data that will be passed back to your redirect URI. This parameter can be used to track a user through the authorization flow.</param>
        /// <returns>A URL to which you should redirect the user.  Because /oauth2/authorize is a website, there is no direct return value. However, after the user authorizes your app, they will be sent to your redirect URI. The type of response varies based on the response_type.</returns>
        public RestRequest BuildOAuth2AuthorizeUrl(OAuth2AuthorizationFlow oAuth2AuthorizationFlow, string consumerKey, string redirectUri, string state = null)
        {
            var request = new RestRequest("{version}/oauth2/authorize", Method.GET);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("response_type", Enum.GetName(typeof(OAuth2AuthorizationFlow), oAuth2AuthorizationFlow).ToLower());
            request.AddParameter("client_id", consumerKey);
            request.AddParameter("redirect_uri", redirectUri);
            if (!string.IsNullOrWhiteSpace(state))
            {
                request.AddParameter("state", state);
            }
            return request;
        }

        /// <summary>
        /// This is the second and final step in the OAuth 2.0 'code' authorization flow.  It retrieves an OAuth2 access token using the code provided by Dropbox.
        /// </summary>
        /// <param name="code">The code provided by Dropbox when the user was redirected back to the calling site.</param>
        /// <param name="redirectUri">The calling site's host name, for verification only</param>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <returns></returns>
        public RestRequest CreateOAuth2AccessTokenRequest(string code, string redirectUri, string consumerKey, string consumerSecret)
        {
            var request = new RestRequest("{version}/oauth2/token", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", consumerKey);
            request.AddParameter("client_secret", consumerSecret);
            request.AddParameter("redirect_uri", redirectUri);
            return request;
        }

        public RestRequest CreateAccountInfoRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/account/info";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateCreateFolderRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/create_folder";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", root);

            return request;
        }

        internal RestRequest CreateDeltaRequest(string cursor)
        {
            var request = new RestRequest(Method.POST);
            request.Resource = "{version}/delta";

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("cursor", cursor);

            return request;
        }

        public RestRequest CreateThumbnailRequest(string path, ThumbnailSize size, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/thumbnails/{root}{path}";

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("size", ThumbnailSizeString(size));

            return request;
        }

        private string ThumbnailSizeString(ThumbnailSize size)
        {
            switch (size)
            {
                case ThumbnailSize.Small:
                    return "small";
                case ThumbnailSize.Medium:
                    return "medium";
                case ThumbnailSize.Large:
                    return "large";
                case ThumbnailSize.ExtraLarge:
                    return "l";
                case ThumbnailSize.ExtraLarge2:
                    return "xl";
            }
            return "s";
        }

        public RestRequest CreateSearchRequest(string searchString, string path, string root)
        {
            var request = new RestRequest(Method.GET)
                              {
                                  Resource = "{version}/search/{root}{path}"
                              };

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("query", searchString);

            return request;
        }
    }

	internal static class StreamUtils
	{
		private const int STREAM_BUFFER_SIZE = 128 * 1024; // 128KB

		public static void CopyStream (Stream source, Stream target)
		{ CopyStream (source, target, new byte[STREAM_BUFFER_SIZE]); }

		public static void CopyStream (Stream source, Stream target, byte[] buffer)
		{
			if (source == null) throw new ArgumentNullException ("source");
			if (target == null) throw new ArgumentNullException ("target");

			if (buffer == null) buffer = new byte[STREAM_BUFFER_SIZE];
			int bufferLength = buffer.Length;
			int bytesRead;
			while ((bytesRead = source.Read (buffer, 0, bufferLength)) > 0)
				target.Write (buffer, 0, bytesRead);
		}
	}
}
