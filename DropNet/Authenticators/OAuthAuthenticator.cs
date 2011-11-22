using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Security.Cryptography;
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
        private const string SignatureMethod = "HMAC-SHA1";
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
            this._baseUrl = baseUrl;
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._token = token;
            this._tokenSecret = tokenSecret;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
			if (request.Method == Method.PUT)
			{
                //Do the parameters as URL segments for PUT
                request.AddParameter("oauth_consumer_key", this._consumerKey, ParameterType.UrlSegment);
                request.AddParameter("oauth_nonce", this.GenerateNonce(), ParameterType.UrlSegment);
                if (!string.IsNullOrEmpty(this._token))
                {
                    request.AddParameter("oauth_token", this._token, ParameterType.UrlSegment);
                }
				request.AddParameter("oauth_timestamp", this.GenerateTimeStamp(), ParameterType.UrlSegment);
                request.AddParameter("oauth_signature_method", "HMAC-SHA1", ParameterType.UrlSegment);
                request.AddParameter("oauth_version", "1.0", ParameterType.UrlSegment);
				request.Parameters.Sort(new QueryParameterComparer());
				request.AddParameter("oauth_signature", this.GenerateSignature(request), ParameterType.UrlSegment);
			}
			else
			{
				request.AddParameter("oauth_version", "1.0");
				request.AddParameter("oauth_nonce", this.GenerateNonce());
				request.AddParameter("oauth_timestamp", this.GenerateTimeStamp());
				request.AddParameter("oauth_signature_method", "HMAC-SHA1");
				request.AddParameter("oauth_consumer_key", this._consumerKey);
				if (!string.IsNullOrEmpty(this._token))
				{
					request.AddParameter("oauth_token", this._token);
				}
				request.Parameters.Sort(new QueryParameterComparer());
				request.AddParameter("oauth_signature", this.GenerateSignature(request));
			}
        }

        private Uri BuildUri(IRestRequest request)
        {
            string resource = request.Resource;
            resource = request.Parameters.Where<Parameter>(delegate(Parameter p)
            {
                return (p.Type == ParameterType.UrlSegment);
            }).Aggregate<Parameter, string>(resource, delegate(string current, Parameter p)
            {
                return current.Replace("{" + p.Name + "}", p.Value.ToString().UrlEncode());
            });
            return new Uri(string.Format("{0}/{1}", this._baseUrl, resource));
        }

        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
        }

        public string GenerateNonce()
        {
            return Random.Next(0x1e208, 0x98967f).ToString();
        }

        private string GenerateSignature(IRestRequest request)
        {
            Uri uri = this.BuildUri(request);
            string str = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if (((uri.Scheme != "http") || (uri.Port != 80)) && ((uri.Scheme != "https") || (uri.Port != 0x1bb)))
            {
                str = str + ":" + uri.Port;
            }
            str = str + uri.AbsolutePath;
            string str2 = NormalizeRequestParameters(request.Parameters);
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}&", request.Method.ToString().ToUpper());
            builder.AppendFormat("{0}&", str.UrlEncode());
            builder.AppendFormat("{0}", str2.UrlEncode());
            string data = builder.ToString();
            HMACSHA1 hashAlgorithm = new HMACSHA1();
            hashAlgorithm.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", this._consumerSecret.UrlEncode(), string.IsNullOrEmpty(this._tokenSecret) ? string.Empty : this._tokenSecret.UrlEncode()));
            return ComputeHash(hashAlgorithm, data);
        }

        public string GenerateTimeStamp()
        {
            TimeSpan span = (TimeSpan)(DateTime.UtcNow - new DateTime(0x7b2, 1, 1, 0, 0, 0, 0));
            return Convert.ToInt64(span.TotalSeconds).ToString();
        }

        private static string NormalizeRequestParameters(IList<Parameter> parameters)
        {
            StringBuilder builder = new StringBuilder();
            List<Parameter> list = parameters.Where(p =>
            {
                //Hackity hack, don't come back...
                return (p.Type == ParameterType.GetOrPost || p.Name == "file" || p.Name.StartsWith("oauth_"));
            }).ToList();

            Parameter parameter = null;
            
            for (int i = 0; i < list.Count; i++)
            {
                parameter = list[i];
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
