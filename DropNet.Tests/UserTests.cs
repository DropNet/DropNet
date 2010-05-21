using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DropNet.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UserTests
    {
        DropNetClient _client;

        public UserTests()
        {
            //
            // TODO: Add constructor logic here
            //
            _client = new DropNetClient(TestVarables.ApiKey, TestVarables.ApiSecret);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Can_Login()
        {
            //
            // TODO: Add test logic here
            //
            var userLogin = _client.Login(TestVarables.Email, TestVarables.Password);

            Assert.IsNotNull(userLogin);
            Assert.IsNotNull(userLogin.Token);
            Assert.IsNotNull(userLogin.Secret);
        }

        [TestMethod]
        public void Can_Get_AccountInfo()
        {
            //
            // TODO: Add test logic here
            //
            _client.Login(TestVarables.Email, TestVarables.Password);
            var accountInfo = _client.Account_Info();

            Assert.IsNotNull(accountInfo);
            Assert.IsNotNull(accountInfo.Display_Name);
            Assert.IsNotNull(accountInfo.Uid);
        }

    }
}
