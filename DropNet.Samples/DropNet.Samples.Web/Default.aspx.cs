using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DropNet.Samples.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        ////////////////////////////////////////////////////
        // NOTE: This key is a Development only key setup for this sample and will only work with my login.
        // MAKE SURE YOU CHANGE IT OR IT WONT WORK!
        ////////////////////////////////////////////////////
        DropNetClient _client = new DropNetClient("9m6v782a7aeop0w", "dbd11uqce6hr8zg");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["dropboxcallback"] == "1")
            {
                //Its a callback from dropbox!
                if (Session["DropNetUserLogin"] != null)
                {
                    _client.UserLogin = Session["DropNetUserLogin"] as DropNet.Models.UserLogin;
                    Session["DropNetUserLogin"] = _client.GetAccessToken();

                    var accountinfo = _client.Account_Info();
                    litOutput.Text = accountinfo.quota_info.quota.ToString();
                }
                else
                {
                    litOutput.Text = "Session expired...";
                }
            }
        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            Session["DropNetUserLogin"] = _client.GetToken();

            var url = _client.BuildAuthorizeUrl(Request.Url.ToString() + "?dropboxcallback=1");

            Response.Redirect(url);
        }
    }
}
