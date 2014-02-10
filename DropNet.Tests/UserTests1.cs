using System;
using DropNet.Authenticators;
using DropNet.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DropNet.Tests
{
    [TestClass]
    public class UserTests1
    {
        readonly DropNetClient _client;

        public UserTests1()
        {
            _client = new DropNetClient(TestVariables.ApiKey, TestVariables.ApiSecret);
        }

        [TestMethod]
        public void Test_CanGetRequestToken()
        {
            var response = _client.GetToken();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.IsNotNull(response.Secret);
        }

        [TestMethod]
        public void Test_CanBuildAutorizeUrl()
        {
            var authorizeUrl = _client.BuildAuthorizeUrl(new Models.UserLogin
                                                        {
                                                            Secret = TestVariables.Secret,
                                                            Token = TestVariables.Token
                                                        });

            Assert.IsNotNull(authorizeUrl);
        }

        [TestMethod]
        public void Test_BuildAutorizeUrl_ThrowNullException()
        {
            try
            {
                _client.BuildAuthorizeUrl();

                Assert.Fail();
            }
            catch (ArgumentNullException ane)
            {
                Assert.IsNotNull(ane);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Can_Get_AccountInfo()
        {
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token, Secret = TestVariables.Secret };

            var accountInfo = _client.AccountInfo();

            Assert.IsNotNull(accountInfo);
            Assert.IsNotNull(accountInfo.display_name);
            Assert.IsNotNull(accountInfo.uid);
        }

        [TestMethod]
        public void BuildOAuth2AuthorizationUrl_RedirectUriIsRequired()
        {
            try
            {
                _client.BuildAuthorizeUrl(OAuth2AuthorizationFlow.Code, null);
                Assert.Fail();
            }
            catch (ArgumentNullException ane)
            {
                Assert.IsNotNull(ane);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void BuildOAuth2AuthorizationUrl_CodeFlow_NoState()
        {
            TestOAuth2AuthorizationUrl(OAuth2AuthorizationFlow.Code, "code");
        }

        [TestMethod]
        public void BuildOAuth2AuthorizationUrl_CodeFlow_WithState()
        {
            TestOAuth2AuthorizationUrl(OAuth2AuthorizationFlow.Code, "code", "foobar");
        }

        [TestMethod]
        public void BuildOAuth2AuthorizationUrl_TokenFlow_NoState()
        {
            TestOAuth2AuthorizationUrl(OAuth2AuthorizationFlow.Token, "token");
        }

        [TestMethod]
        public void BuildOAuth2AuthorizationUrl_TokenFlow_WithState()
        {
            TestOAuth2AuthorizationUrl(OAuth2AuthorizationFlow.Token, "token", "foobar");
        }

        private void TestOAuth2AuthorizationUrl(OAuth2AuthorizationFlow oAuth2AuthorizationFlow, string expectedResponseType, string state = null)
        {
            const string expectedFormat = "https://api.dropbox.com/1/oauth2/authorize?response_type={0}&client_id={1}&redirect_uri=http://example.com{2}";
            var stateQuery = string.IsNullOrWhiteSpace(state) ? string.Empty : string.Format("&state={0}", state);
            var expected = string.Format(expectedFormat, expectedResponseType, TestVariables.ApiKey, stateQuery);
            var actual = _client.BuildAuthorizeUrl(oAuth2AuthorizationFlow, "http://example.com", state);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(DropboxException))]
        public void Timeout_Exception_Raised_On_Super_Short_Timeout()
        {
            var client = new DropNetClient("", "");
            client.TimeoutMS = 100;
            
            client.GetToken();
        }
    }
}
