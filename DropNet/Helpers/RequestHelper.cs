using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestSharp;
using DropNet.Extensions;

namespace DropNet.Helpers
{
    /// <summary>
    /// Helper class for creating DropNet RestSharp Requests
    /// </summary>
    public class RequestHelper
    {       
        private string _version;

        public RequestHelper(string version)
        {
            _version = version;
        }

        public RestRequest CreateMetadataRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/metadata/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateSharesRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/shares/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateGetFileRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/files/dropbox{path}";
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);

            return request;
        }

		public RestRequest CreateUploadFileRequest (string path, string filename, byte[] fileData)
		{
			var request = new RestRequest (Method.POST);
			request.Resource = "{version}/files/dropbox{path}";
			request.AddParameter ("version", _version, ParameterType.UrlSegment);
			request.AddParameter ("path", path, ParameterType.UrlSegment);
			//Need to add the "file" parameter with the file name
			request.AddParameter ("file", filename);

			request.AddFile ("file", fileData, filename);

			return request;
		}

		public RestRequest CreateUploadFileRequest (string path, string filename, Stream fileStream)
		{
			var request = new RestRequest (Method.POST);
			request.Resource = "{version}/files/dropbox{path}";
			request.AddParameter ("version", _version, ParameterType.UrlSegment);
			request.AddParameter ("path", path, ParameterType.UrlSegment);
			//Need to add the "file" parameter with the file name
			request.AddParameter ("file", filename);

			request.AddFile ("file", s => StreamUtils.CopyStream (fileStream, s), filename);

			return request;
		}

		public RestRequest CreateDeleteFileRequest (string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/delete";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", "dropbox");

            return request;
        }

        public RestRequest CreateCopyFileRequest(string fromPath, string toPath)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/copy";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", "dropbox");

            return request;
        }

        public RestRequest CreateMoveFileRequest(string fromPath, string toPath)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/move";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", "dropbox");

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

        public RestRequest CreateWebAuthRequest()
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/oauth/request_token";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            //request.AddParameter("oauth_consumer_key", apiKey);

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

        public RestRequest CreateCreateFolderRequest(string path)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "{version}/fileops/create_folder";
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", "dropbox");

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
			int bytesRead = 0;
			while ((bytesRead = source.Read (buffer, 0, bufferLength)) > 0)
				target.Write (buffer, 0, bytesRead);
		}
	}
}
