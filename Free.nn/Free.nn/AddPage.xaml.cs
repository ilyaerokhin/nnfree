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

namespace Free.nn
{
    public partial class AddPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        CameraCaptureTask cameraCaptureTask;
        string photo;
        BitmapImage img;
        NNFreeAPI free;
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

                photo = e.OriginalFileName;
            }
        }
        void cameraCaptureTask_Comp(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                var scaledDownImage = AspectScale(bmp, 640, 480);
                MyPhoto.Source = scaledDownImage;

                photo = e.OriginalFileName;
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

        private void Add(object sender, EventArgs e)
        {

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
    }
}