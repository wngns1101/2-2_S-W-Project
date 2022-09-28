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

namespace Kinect_ColorStream
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor objKS = null;     //KinectSensors 객체
        public MainWindow()
        {
            InitializeComponent();
            InitializeKinect();
        }
            void InitializeKinect()
            {
                objKS = KinectSensor.KinectSensors[0];  //Call connected Kinect

                objKS.ColorStream.Enable();
                //VideoFrameReady EventHandler
                objKS.ColorFrameReady +=
                    new EventHandler<ColorImageFrameReadyEventArgs>(objKS_ColorFrameReady);
                objKS.Start();
            }

            void objKS_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
            {
                ColorImageFrame imgParam = e.OpenColorImageFrame();   //Graphic Data

                if (imgParam == null) return;
                byte[] imageBits = new byte[imgParam.PixelDataLength];
                imgParam.CopyPixelDataTo(imageBits);

                BitmapSource imgSource = null;
                //Create Bitmap Image
                imgSource = BitmapSource.Create(imgParam.Width, imgParam.Height,
                                         96, 96, PixelFormats.Bgr32,
                                         null, imageBits, imgParam.Width * imgParam.BytesPerPixel);
                image1.Source = imgSource;

                FramePerSecond(imgParam);   //Output Frame Information
            }

        long m_orgFrame = -1;
        long m_orgTime = -1;
        long m_saveTime = 0;
        long m_saveFrame = 0;

        void FramePerSecond(ColorImageFrame imgFrame)
        {
            //Frame Per Second
            long IgFrame = (long)imgFrame.FrameNumber;   //The Number of Frame
            long IgTime = (long)imgFrame.Timestamp;        //Elapsed Time

            if (m_orgFrame <= 0) m_orgFrame = IgFrame;
            if (m_orgTime <= 0) m_orgTime = IgTime;
            IgTime = IgTime - m_orgTime;
            IgFrame = IgFrame - m_orgFrame;

            //Total number of Frame & Time
            TextBlock1.Text = "Total Frame : " + IgFrame.ToString();
            TextBlock2.Text = "Elapsed Time : " + IgTime.ToString();

            long IgAvgFrame = 0;
            if (IgFrame > 0 && IgTime > 0)
            {
                IgAvgFrame = (IgFrame * 1000) / IgTime;
            }
            TextBlock3.Text = "FPS:" + IgAvgFrame.ToString();

            //The Number of Frame(the nearest 1 second)
            if (IgTime - m_saveTime > 1000)
            {
                long IgTemp = IgFrame - m_saveFrame;
                IgTemp = IgTemp * 1000 / (IgTime - m_saveTime);

                TextBlock4.Text = "Frame(the nearest 1 second) :" + IgTemp.ToString();
                m_saveTime = IgTime;
                m_saveFrame = IgFrame;
            }
        }

    }
    }
