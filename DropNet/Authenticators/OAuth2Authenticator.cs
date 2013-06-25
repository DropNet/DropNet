using RestSharp;

namespace DropNet.Authenticators
{
    public class OAuth2Authenticator : RestSharp.OAuth2Authenticator
    {
        public OAuth2Authenticator(string accessToken) : base(accessToken)
        {
        }

        public override void Authenticate(IRestClient client, IRestRequest request)
        {
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                request.AddHeader("Authorization", string.Format("Bearer {0}", AccessToken));
            }
        }
    }

    /// <summary>
    ///    OAuth 2.0 supports two authorization flows.  Once the user has authorized your application 
    /// (via the URL provided by BuildAuthorizationUrl) Dropbox will redirect the user back to your 
    /// site/application with query string parameters that vary based on the authorization flow defined 
    /// here.  For a description of these parameters, see https://www.dropbox.com/developers/core/docs#oa2-authorize. 
    /// See also Section 1.3 of the OAuth 2 spec at https://tools.ietf.org/html/draft-ietf-oauth-v2-31#section-1.3
    /// </summary>
    public enum OAuth2AuthorizationFlow
    {
        /// <summary>
        ///    The 'token' (or implicit) grant flow returns the bearer token via the redirect_uri callback, 
        /// rather than requiring your app to make a second call to a server. This is useful for pure 
        /// client-side apps, such as mobile apps or JavaScript-based apps.
        /// </summary>
        Token,

        /// <summary>
        ///     The 'code' flow returns a code via the redirect_uri callback which should then be converted 
        /// into a bearer token using the GetAccessToken(code, ) call. This is the recommended flow for apps 
        /// that are running on a server.
        /// </summary>
        Code,
    }    
}