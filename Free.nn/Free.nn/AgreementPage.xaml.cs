using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Free.nn
{
    public partial class AgreementPage : PhoneApplicationPage
    {
        public AgreementPage()
        {
            InitializeComponent();
        }

        private void WithAction_Click(object sender, RoutedEventArgs e)
        {
            string uri = "https://api.vkontakte.ru/method/groups.join.xml?group_id=" + "44021730" + "&access_token=" + PublicData.accessToken;

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += groupsjoin_OpenReadCompleted;
            webClient.OpenReadAsync(new Uri(uri));
        }

        private void groupsjoin_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
        }

        private void WithoutAction_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
        }
    }
}