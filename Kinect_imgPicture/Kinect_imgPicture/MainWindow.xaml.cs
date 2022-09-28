using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;

namespace Kinect_imgPicture
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor objKS = null;    //KinectSensor 객체
        public MainWindow()
        {
            InitializeComponent();
            InitializeKinect();
        }
        void InitializeKinect()
        {
            objKS = KinectSensor.KinectSensors[0];   //Call connected Kinect

            objKS.ColorStream.Enable();
            //VideoFrameReady EventHandler
            objKS.ColorFrameReady +=
                new EventHandler<ColorImageFrameReadyEventArgs>(objKS_ColorFrameReady);
            objKS.Start();
        }

        BitmapSource imgSource = null;

        void objKS_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame imgParam = e.OpenColorImageFrame();   //Graphic Data

            if (imgParam == null) return;
            byte[] imageBits = new byte[imgParam.PixelDataLength];
            imgParam.CopyPixelDataTo(imageBits);

            //Create Bitmap Image
            imgSource = BitmapSource.Create(imgParam.Width, imgParam.Height,
                                    96, 96, PixelFormats.Bgr32,
                                    null, imageBits, imgParam.Width * imgParam.BytesPerPixel);
            image1.Source = imgSource;
        }



        private void btnImageFile_Click(object sender, RoutedEventArgs e)
        {
            if (imgSource != null)
            {
                //Save : Image -> File
                BitmapEncoder encoder = null;
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imgSource));

                File.Delete("imgPicture.png");
                FileStream fStream = new FileStream("imgPicture.png", FileMode.Create, FileAccess.Write);
                encoder.Save(fStream);
                fStream.Close();

                System.Diagnostics.Process exe = new System.Diagnostics.Process();
                exe.StartInfo.FileName = "imgPicture.png";
                exe.Start();
                
            }
        }
    }
}