using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

namespace Free.nn
{
    public partial class AddPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        string photo;
        BitmapImage img = null;
        NNFreeAPI free;
        SocketClient photoclient;
        public AddPage()
        {
            InitializeComponent();

            // для фоток 
            //*********************************************************************************
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Comp);
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Comp);
            //**********************************************************************************
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

            photoclient = new SocketClient();
            photoclient.Connect(PublicData.photohost, PublicData.photoport);
        }

        // Функции к галерее и фотику
        //*********************************************************************************
        void photoChooserTask_Comp(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                var scaledDownImage = AspectScale(bmp, 640, 480);
                MyPhoto.Source = scaledDownImage;
                img = scaledDownImage;
            }
        }
        void cameraCaptureTask_Comp(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                var scaledDownImage = AspectScale(bmp, 320, 240);
                MyPhoto.Source = scaledDownImage;
                img = scaledDownImage;
            }
        }

        private BitmapImage AspectScale(BitmapImage img, int maxWidth, int maxHeigh)
        {
            double scaleRatio;

            if (img.PixelWidth > img.PixelHeight)
                scaleRatio = (maxWidth / (double)img.PixelWidth);
            else
                scaleRatio = (maxHeigh / (double)img.PixelHeight);

            var scaledWidth = img.PixelWidth * scaleRatio;
            var scaledHeight = img.PixelHeight * scaleRatio;

            using (var mem = new MemoryStream())
            {
                var wb = new WriteableBitmap(img);
                wb.SaveJpeg(mem, (int)scaledWidth, (int)scaledHeight, 0, 100);
                mem.Seek(0, SeekOrigin.Begin);
                var bn = new BitmapImage();
                bn.SetSource(mem);
                return bn;
            }
        }
        public EventHandler<PhotoResult> photoChooserTask_Completed { get; set; }
        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            cameraCaptureTask.Show();
        }

        private void Galery_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }
        //*********************************************************************************

        private void Add(object sender, EventArgs e)
        {
            if(low_text.Text.CompareTo(string.Empty) == 0)
            {
                MessageBox.Show("Введите заголовок объявления!");
                return;
            }
            if (big_text.Text.CompareTo(string.Empty) == 0)
            {
                MessageBox.Show("Введите текст объявления!");
                return;
            }
            if (img == null)
            {
                MessageBox.Show("Выберите фотографию!");
                return;
            }

            int value = free.AddAdvert(PublicData.id, low_text.Text, big_text.Text);
            photoclient.SendFile(img, value.ToString() + ".jpg");

            string uri = "https://api.vkontakte.ru/method/board.addComment.xml?group_id=44021730&topic_id=31675032&text=" + big_text.Text + "&access_token=" + PublicData.accessToken;

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += boardaddComment_OpenReadCompleted;
            webClient.OpenReadAsync(new Uri(uri));
        }

        private void boardaddComment_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //XDocument xml = XDocument.Load(e.Result);
            //MessageBox.Show(xml.Root.ToString());
            MessageBox.Show("Объявление успешно размещено");
            NavigationService.Navigate(new Uri("/MenuPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MenuPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        }
    }
}