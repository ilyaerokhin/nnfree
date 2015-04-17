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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Free.nn
{
    public partial class MenuPage : PhoneApplicationPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            MessageBox.Show("Имя  - "+PublicData.first_name+
                            "\nID   - "+PublicData.id+
                            "\nПол  - "+PublicData.sex.ToString()+
                            "\nФото - "+PublicData.photo_path+
                            "\nToken- "+PublicData.accessToken);
            // ОТПРАВКА НА СТЕНУ
            //string uri = "https://api.vkontakte.ru/method/wall.post.xml?owner_id="+PublicData.id+"&message=" + "Привет" + "&access_token=" + PublicData.accessToken;

            // ОТПРАВКА СООБЩЕНИЯ
            //string uri = "https://api.vkontakte.ru/method/messages.send.xml?user_id=" + PublicData.id + "&message=" + "Привет" + "&access_token=" + PublicData.accessToken;

            // СОСТОИТ ЛИ В ГРУППЕ
            //string uri = "https://api.vkontakte.ru/method/groups.isMember.xml?group_id=" + "smmpub" + "&user_id=" + PublicData.id + "&message=" + "Привет";

            // ВСТУПЛЕНИЕ В ГРУППУ
            string uri = "https://api.vkontakte.ru/method/groups.join.xml?group_id=" + "43503600" + "&access_token=" + PublicData.accessToken;

            // ВЫХОД ИЗ ГРУПП
            //string uri = "https://api.vkontakte.ru/method/groups.leave.xml?group_id=" + "43503600" + "&access_token=" + PublicData.accessToken;

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += wallPost_OpenReadCompleted;
            webClient.OpenReadAsync(new Uri(uri));
        }

        private void wallPost_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XDocument xml = XDocument.Load(e.Result);
            MessageBox.Show(xml.Root.Document.ToString());
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Task.Delay(100);
            if (MessageBox.Show("Вы хотите покинуть приложение?", "Выход", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
                Application.Current.Terminate();
            }
        }

        private void Logout(object sender, EventArgs e)
        {
            new WebBrowser().ClearCookiesAsync();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}