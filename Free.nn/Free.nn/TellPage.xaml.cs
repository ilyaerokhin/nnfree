using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;

namespace Free.nn
{
    public partial class TellPage : PhoneApplicationPage
    {
        public TellPage()
        {
            InitializeComponent();
        }

        private void Send(object sender, EventArgs e)
        {
            string uri = "https://api.vkontakte.ru/method/messages.send.xml?user_id=" + PublicData.owner_id + "&message=" + text.Text + "&access_token=" + PublicData.accessToken;

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += messagessend_OpenReadCompleted;
            webClient.OpenReadAsync(new Uri(uri));
        }

        private void messagessend_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            MessageBox.Show("Сообщение отправлено автору объявления личным сообщением ВКонтакте");
            NavigationService.Navigate(new Uri("/MenuPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MenuPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }
    }
}