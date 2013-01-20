using System;
using System.IO;
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

        public RestRequest CreateShareRequest(string path, string root)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/shares/{root}{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

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

			request.AddFile("file", fileData, filename);

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
