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
using System.IO.IsolatedStorage;
using Windows.UI;
using System.Windows.Media;

namespace Free.nn
{
    public partial class MainPage : PhoneApplicationPage
    {
        NNFreeAPI free;
        public MainPage()
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
                browser.Navigate(new Uri("https://oauth.vk.com/authorize?client_id="+free.App_ID()+"&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,groups,messages,email,friends&display=wap&response_type=token"));
            }
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
                Top.Visibility = Visibility.Visible;
                browser.Visibility = Visibility.Visible;
                icon.Visibility = Visibility.Collapsed;
                //LayoutRoot.Background = new System.Windows.Media.SolidColorBrush(Color); ;
            }
        }

        private void getProfiles_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XDocument xml = XDocument.Load(e.Result);
            XElement element = xml.Root.Element("user");
            PublicData.first_name = string.Format(element.Element("first_name").Value);
            PublicData.sex = int.Parse(element.Element("sex").Value);
            PublicData.photo_path = string.Format(element.Element("photo_100").Value);

            string uri = "https://api.vkontakte.ru/method/groups.getBanned.xml?group_id=44021730&user_id="+PublicData.id+"&access_token=" + PublicData.accessToken;

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += getBanned_OpenReadCompleted;
            webClient.OpenReadAsync(new Uri(uri));
        }

        private void getBanned_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XDocument xml = XDocument.Load(e.Result);
            //MessageBox.Show(xml.Root.ToString());

            if (xml.Root.ToString().Contains("<error_code>104</error_code>"))
            {
                int a = free.CheckUser(PublicData.id);
                if (a == 0)
                {
                    free.Close();
                    NavigationService.Navigate(new Uri("/MenuPage.xaml", UriKind.Relative));
                }
                else
                {
                    if(a == 1)
                    {
                        free.Close();
                        NavigationService.Navigate(new Uri("/AgreementPage.xaml", UriKind.Relative));
                    }
                }
            }
            if (xml.Root.ToString().Contains("<response list=\"true\">"))
            {
                XElement element = xml.Root.Element("user").Element("ban_info");
                //MessageBox.Show(element.ToString());
                var comment = string.Format(element.Element("comment").Value);
                var comment_visible = int.Parse(element.Element("comment_visible").Value);
                var end_date = int.Parse(element.Element("end_date").Value);

                TimeSpan ts = TimeSpan.FromSeconds(end_date+3600*3); 
                DateTime dt = new DateTime(1970, 1, 1);
                dt += ts;


                if(comment_visible == 1)
                {
                    if(end_date == 0)
                    {
                        if (MessageBox.Show("Вы занесены в чёрный список сервиса \"Бесплатный Нижний\" навсегда\nКомментарий администратора: "+comment) == MessageBoxResult.OK)
                        {
                            IsolatedStorageSettings.ApplicationSettings.Save();
                            Application.Current.Terminate();
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Вы занесены в чёрный список сервиса \"Бесплатный Нижний\" до "+dt.ToString()+"\nКомментарий администратора: " + comment) == MessageBoxResult.OK)
                        {
                            IsolatedStorageSettings.ApplicationSettings.Save();
                            Application.Current.Terminate();
                        }
                    }
                }
                else
                {
                    if(end_date == 0)
                    {
                        if (MessageBox.Show("Вы занесены в чёрный список сервиса \"Бесплатный Нижний\" навсегда") == MessageBoxResult.OK)
                        {
                            IsolatedStorageSettings.ApplicationSettings.Save();
                            Application.Current.Terminate();
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Вы занесены в чёрный список сервиса \"Бесплатный Нижний\" до "+dt.ToString()) == MessageBoxResult.OK)
                        {
                            IsolatedStorageSettings.ApplicationSettings.Save();
                            Application.Current.Terminate();
                        }
                    }
                }
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы хотите покинуть приложение?", "Выход", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
                Application.Current.Terminate();
            }
        }
    }
}