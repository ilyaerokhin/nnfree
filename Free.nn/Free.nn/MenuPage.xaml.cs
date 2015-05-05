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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;

namespace Free.nn
{
    public partial class MenuPage : PhoneApplicationPage
    {
        static NNFreeAPI free;
        static int advert_id;
        ScrollViewer scrollViewer;
        public readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register("ListVerticalOffset",
typeof(double), typeof(MenuPage), new PropertyMetadata(new PropertyChangedCallback(OnListVerticalOffsetChanged)));

        private static void OnListVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuPage page = d as MenuPage;
            ScrollViewer viewer = page.scrollViewer;
            //Checks if the Scroll has reached the last item based on the ScrollableHeight 
            bool atBottom = viewer.VerticalOffset >= viewer.ScrollableHeight/30;
            if (atBottom)
            {
                //MessageBox.Show("here");
                if(advert_id<0)
                {
                    return;
                }

                string value = free.ListAds(advert_id.ToString());
                string[] ads = value.Split(new Char[] { '/' });
                int n = 0;

                //MessageBox.Show(value);
                foreach (string s in ads)
                {
                    // если строка не пустая
                    if (s.Trim() != "")
                    {
                        string[] text = s.Split(new Char[] { '|' });
                        string[] big_text = free.GetAdvert(text[0]).Split(new Char[] { '/' });
                        page.list_ads.Items.Add(new Adverts() { owner = big_text[0], low_text = text[1], name = big_text[3], big_text = big_text[2], advert_id = "# " + text[0], ImagePath = "http://109.120.164.212/photos/" + text[0] + ".jpg" + "?" + Guid.NewGuid().ToString() });
                        n = Int32.Parse(text[0]);  
                    }
                }
                advert_id = n - 1;
            }
        }

        public double ListVerticalOffset
        {
            get { return (double)this.GetValue(ListVerticalOffsetProperty); }
            set { this.SetValue(ListVerticalOffsetProperty, value); }
        }

        public MenuPage()
        {
            InitializeComponent();
            list_ads.Loaded += new RoutedEventHandler(list_ads_Loaded); 
        }

        private void list_ads_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Loaded -= list_ads_Loaded;
            scrollViewer = FindChildOfType<ScrollViewer>(element);
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("ScrollViewer not found.");
            }

            Binding binding = new Binding();
            binding.Source = scrollViewer;
            binding.Path = new PropertyPath("VerticalOffset");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(ListVerticalOffsetProperty, binding);
        }

        static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
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

                advert_id = free.NumberOfAds();
                string value = free.ListAds(advert_id.ToString());
                string[] ads = value.Split(new Char[] { '/' });
                int n = 0;

                foreach (string s in ads)
                {
                    // если строка не пустая
                    if (s.Trim() != "")
                    {
                        string[] text = s.Split(new Char[] { '|' });
                        string[] big_text = free.GetAdvert(text[0]).Split(new Char[] { '/' });
                        list_ads.Items.Add(new Adverts() { owner = big_text[0], low_text = text[1], name = big_text[3], big_text = big_text[2], advert_id = "# " + text[0], ImagePath = "http://109.120.164.212/photos/" + text[0] + ".jpg" + "?" + Guid.NewGuid().ToString() });
                        n = Int32.Parse(text[0]);                    
                    }
                }
                advert_id = n - 1;
            }
        }

        private void Refresh()
        {
            free.Close();
            NavigationService.Navigate(new Uri("/MenuPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы хотите покинуть приложение?", "Выход", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
                Application.Current.Terminate();
            }
        }

        private void Logout(object sender, EventArgs e)
        {
            new WebBrowser().ClearCookiesAsync();
            NavigationService.Navigate(new Uri("/MainPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }

        private void list_ads_Scroll(object sender, ScrollEventArgs e)
        {
            ScrollBar sb = e.OriginalSource as ScrollBar;

            MessageBox.Show(sb.Value.ToString());
            if (sb.Value == sb.Maximum)
            {

                MessageBox.Show("here");

            }

        }

        private void Add(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddPage.xaml", UriKind.Relative));
        }

        private void RefreshPage(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Tell(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TellPage.xaml", UriKind.Relative));
        }

        private void Profile_panel_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            object owner = ((StackPanel)sender).FindName("owner");
            if (owner is TextBlock)
            {
                TextBlock wantedChild = owner as TextBlock;
                PublicData.owner_id = wantedChild.Text;
            }

            object advert = ((StackPanel)sender).FindName("advert_id");
            if (advert is TextBlock)
            {
                TextBlock wantedChild = advert as TextBlock;
                PublicData.advert_id = wantedChild.Text.Split(new Char[] { ' ' })[1];
            }
        }

        private void Bad_Click(object sender, RoutedEventArgs e)
        {
            free.BadAdvert(PublicData.owner_id, PublicData.advert_id);
        }
    }
}