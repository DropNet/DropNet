using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using DropNet.Models;

namespace DropNet.Samples.WP7.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _showLogin;
        public bool ShowLogin
        {
            get { return _showLogin; }
            set
            {
                _showLogin = value;
                NotifyPropertyChanged("ShowLogin");
            }
        }

        private bool _showContents;
        public bool ShowContents
        {
            get { return _showContents; }
            set
            {
                _showContents = value;
                NotifyPropertyChanged("ShowContents");
            }
        }

        private MetaData _meta;
        public MetaData Meta
        {
            get { return _meta; }
            set
            {
                _meta = value;
                NotifyPropertyChanged("Meta");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                  PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }
    }
}
