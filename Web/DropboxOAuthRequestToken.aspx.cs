using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OAuth;
using System.Text;

public partial class DropboxOAuthRequestToken : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!LinkedPapers.User.LoggedIn()) Response.Redirect("/");

        string consumerKey = "*********";
        string consumerSecret = "*************";
        Uri uri = new Uri("https://api.dropbox.com/0/oauth/request_token"); 

        string s1, s2;

        OAuthBase oAuth = new OAuthBase();
        string nonce = oAuth.GenerateNonce();
        string timeStamp = oAuth.GenerateTimeStamp();
        string sig = oAuth.GenerateSignature(
            uri, 
            consumerKey, consumerSecret,  
            string.Empty, string.Empty,
            "GET", timeStamp, nonce, 
            OAuthBase.SignatureTypes.HMACSHA1,
            out s1, out s2
            ); 

        sig = HttpUtility.UrlEncode(sig); 

        StringBuilder sb = new StringBuilder(uri.ToString());
        sb.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
        sb.AppendFormat("oauth_nonce={0}&", nonce);
        sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
        sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
        sb.AppendFormat("oauth_version={0}&", "1.0");
        sb.AppendFormat("oauth_signature={0}", sig); 

        string url = sb.ToString();

        WebRequest request = WebRequest.Create(url);


        using (WebResponse response = request.GetResponse())
        {

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string content = reader.ReadToEnd();

                Response.Redirect("https://www.dropbox.com/0/oauth/authorize?" + content + "&oauth_callback=" + Server.UrlEncode("http://dev.linkedpapers.com/Secure/User/Dropbox/DropboxOAuthCallback.aspx"));
            }
        }
    }
}