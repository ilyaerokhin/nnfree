using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace nnfree
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Go_to_women(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "women");
        }

        private void Go_to_men(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "men");
        }

        private void Go_to_child(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "child");
        }

        private void Go_to_tech(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "tech");
        }

        private void Go_to_makeup(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "makeup");
        }

        private void Go_to_animals(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "animals");
        }

        private void Go_to_services(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "services");
        }

        private void Go_to_books(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "books");
        }

        private void Go_to_sport(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "sport");
        }

        private void Go_to_others(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListPage), "others");
        }
    }
}
