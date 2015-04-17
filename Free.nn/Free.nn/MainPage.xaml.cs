using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Free.nn.Resources;
using System.Xml.Linq;

namespace Free.nn
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            browser.Navigate(new Uri("https://oauth.vk.com/authorize?client_id=4871619&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,groups,messages,email,friends&display=wap&response_type=token"));
        }

        void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            string responceData = e.Uri.OriginalString;
            if (responceData.Contains("access_token") && responceData.Contains("id"))
            {
                browser.Visibility = Visibility.Collapsed;
                string[] parameters = responceData.Split('#')[1].Split('&');
                PublicData.accessToken = parameters[0].Substring(parameters[0].IndexOf("=", StringComparison.Ordinal)).Remove(0, 1);
                PublicData.id = parameters[2].Remove(0, 8);

                string uri = "https://api.vkontakte.ru/method/getProfiles.xml?uids=" + PublicData.id + "&fields=first_name,sex,photo_100&access_token=" + PublicData.accessToken;

                WebClient webClient = new WebClient();
                webClient.OpenReadCompleted += getProfiles_OpenReadCompleted;
                webClient.OpenReadAsync(new Uri(uri));
            }
            else
            {
                NAME.Visibility = Visibility.Visible;
            }
        }

        private void getProfiles_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XDocument xml = XDocument.Load(e.Result);
            XElement element = xml.Root.Element("user");
            PublicData.first_name = string.Format(element.Element("first_name").Value);
            PublicData.sex = int.Parse(element.Element("sex").Value);
            PublicData.photo_path = string.Format(element.Element("photo_100").Value);
            NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
        }
    }
}