using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using DropNet.Extensions;

namespace DropNet.Authenticators
{
    public class OAuthAuthenticator : IAuthenticator
    {
        // Fields
        private readonly string _baseUrl;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _token;
        private readonly string _tokenSecret;
        private const string ConsumerKeyKey = "oauth_consumer_key";
        private const string NonceKey = "oauth_nonce";
        private static readonly Random Random = new Random();
        private const string SignatureKey = "oauth_signature";
        private const string SignatureMethod = "PLAINTEXT";
        private const string SignatureMethodKey = "oauth_signature_method";
        private const string TimestampKey = "oauth_timestamp";
        private const string TokenKey = "oauth_token";
        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        private const string Version = "1.0";
        private const string VersionKey = "oauth_version";

        // Methods
        public OAuthAuthenticator(string baseUrl, string consumerKey, string consumerSecret)
            : this(baseUrl, consumerKey, consumerSecret, string.Empty, string.Empty)
        {
        }

        public OAuthAuthenticator(string baseUrl, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            _baseUrl = baseUrl;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = token;
            _tokenSecret = tokenSecret;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
			if (request.Method == Method.PUT)
			{
                //Do the parameters as URL segments for PUT
                request.AddParameter("oauth_consumer_key", _consumerKey, ParameterType.UrlSegment);
                request.AddParameter("oauth_nonce", GenerateNonce(), ParameterType.UrlSegment);
                if (!string.IsNullOrEmpty(_token))
                {
                    request.AddParameter("oauth_token", _token, ParameterType.UrlSegment);
                }
				request.AddParameter("oauth_timestamp", GenerateTimeStamp(), ParameterType.UrlSegment);
                request.AddParameter("oauth_signature_method", SignatureMethod, ParameterType.UrlSegment);
                request.AddParameter("oauth_version", "1.0", ParameterType.UrlSegment);
				request.Parameters.Sort(new QueryParameterComparer());
				request.AddParameter("oauth_signature", GenerateSignature(request), ParameterType.UrlSegment);
			}
			else
			{
				request.AddParameter("oauth_version", "1.0");
				request.AddParameter("oauth_nonce", GenerateNonce());
				request.AddParameter("oauth_timestamp", GenerateTimeStamp());
				request.AddParameter("oauth_signature_method", SignatureMethod);
				request.AddParameter("oauth_consumer_key", _consumerKey);
				if (!string.IsNullOrEmpty(_token))
				{
					request.AddParameter("oauth_token", _token);
				}
				request.Parameters.Sort(new QueryParameterComparer());
				request.AddParameter("oauth_signature", GenerateSignature(request));
			}
        }

        private Uri BuildUri(IRestRequest request)
        {
            string resource = request.Resource;
            resource = request.Parameters.Where(delegate(Parameter p)
            {
                return (p.Type == ParameterType.UrlSegment);

            }).Aggregate(resource, delegate(string current, Parameter p)
            {
                return current.Replace("{" + p.Name + "}", p.Value.ToString().UrlEncode());
            });
            return new Uri(string.Format("{0}/{1}", this._baseUrl, resource));
        }

        public string GenerateNonce()
        {
            return Random.Next(0x1e208, 0x98967f).ToString();
        }

        private string GenerateSignature(IRestRequest request)
        {
            return _consumerSecret + "&" + _tokenSecret;
        }

        public string GenerateTimeStamp()
        {
            TimeSpan span = DateTime.UtcNow - new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(span.TotalSeconds).ToString();
        }

        private static string NormalizeRequestParameters(IEnumerable<Parameter> parameters)
        {
            var builder = new StringBuilder();
            List<Parameter> list = parameters.Where(p =>
            {
                //Hackity hack, don't come back...
                return (p.Type == ParameterType.GetOrPost || p.Name == "file" || p.Name.StartsWith("oauth_"));
            }).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                Parameter parameter = list[i];
                builder.AppendFormat("{0}={1}", parameter.Name, parameter.Value.ToString().UrlEncode());
                if (i < (list.Count - 1))
                {
                    builder.Append("&");
                }
            }
            return builder.ToString();
        }

        // Nested Types
        private class QueryParameterComparer : IComparer<Parameter>
        {
            // Methods
            public int Compare(Parameter x, Parameter y)
            {
                return ((x.Name == y.Name) ? string.Compare(x.Value.ToString(), y.Value.ToString()) : string.Compare(x.Name, y.Name));
            }
        }

    }

}
