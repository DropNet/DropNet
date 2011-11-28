using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using DropNet.Samples.WP7.ViewModels;

namespace DropNet.Samples.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel _model;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Basic MVVM stuff
            _model = new MainViewModel();
            _model.ShowLogin = true;
            this.DataContext = _model;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            App.DropNetClient.GetTokenAsync(userToken =>
                {
                    //Dont really need to do anything with the usertoken yet as its stored inside the client for the session

                    //Now we want to use the new Request token we have to generate an Authorize Url
                    var tokenUrl = App.DropNetClient.BuildAuthorizeUrl("http://dkdevelopment.net/BoxShotLogin.htm"); //Spelt correctly in v1.8.1 
                    //Capture the LoadCompleted event from the browser so we can check when the user has logged in
                    loginBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(loginBrowser_LoadCompleted);
                    //Open a browser with the URL
                    loginBrowser.Navigate(new Uri(tokenUrl));
                },
                (error) =>
                {
                    //OH DEAR GOD WHAT HAPPENED?!
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(error.Message);
                        });
                });
        }

        void loginBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //Check for the callback path here (or just check it against "/1/oauth/authorize")
            if (e.Uri.AbsolutePath == "/BoxShotLogin.htm")
            {
                //The User has logged in!
                //Now to convert the Request Token into an Access Token
                App.DropNetClient.GetAccessTokenAsync(response =>
                    {
                        //GREAT SUCCESS!
                        //Now we should save the Token and Secret so the user doesnt have to login next time
                        //response.Token;
                        //response.Secret;

                        //Now lets load the root contents and hide the login on the page
                        _model.ShowContents = true;
                        _model.ShowLogin = false;

                        LoadContents();
                    },
                    (error) =>
                    {
                        //OH DEAR GOD WHAT HAPPENED?!
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(error.Message);
                        });
                    });
            }
            else
            {
                //Probably the login page loading, ignore
            }
        }

        private void LoadContents()
        {
            App.DropNetClient.GetMetaDataAsync("/", (response) =>
                {
                    _model.Meta = response;
                },
                (error) =>
                {
                    //OH DEAR GOD WHAT HAPPENED?!
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(error.Message);
                    });
                });
        }
    }
}