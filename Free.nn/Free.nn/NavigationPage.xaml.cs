using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Threading;

namespace Free.nn
{
    public partial class NavigationPage : PhoneApplicationPage
    {
        NNFreeAPI free;

        public NavigationPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            free = new NNFreeAPI(PublicData.host, PublicData.port);

            if (free.Connect() == 1)
            {
                MessageBox.Show("Мы не можем подключиться к серверу\nВозможно отсутствует подключение к интернету");
                IsolatedStorageSettings.ApplicationSettings.Save();
                Application.Current.Terminate();
            }
            else
            {
                Thread.Sleep(3000);
                free.Close();
                NavigationService.Navigate(new Uri("/MainPage.xaml?", UriKind.Relative));
            }
        }
    }
}