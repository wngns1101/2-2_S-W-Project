using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectProject
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initializeobjks();

            // 키 재는 모습을 보여줄 사각형 초기화
            rectangle1.Width = 5;
            rectangle1.Opacity = 0.8;
            rectangle1.Visibility = Visibility.Collapsed;
        }
        // 키넥트 변수 선언
        KinectSensor objks = null;

        // 키넥트 초기화 메소드
        void Initializeobjks()
        {
            // 키넥트 인식
            objks = KinectSensor.KinectSensors[0];

            // 키넥트 컬러스트림 활성화
            objks.ColorStream.Enable();

            // RGB를 사용하기 위한 ColorFrameReady 이벤트
            objks.ColorFrameReady += new
                EventHandler<ColorImageFrameReadyEventArgs>(objks_ColorFrameReady);

            // 사람까지의 거리를 측정하기 위한 DepthSteram 활성화
            objks.DepthStream.Enable();

            // 사람의 관절을 측정하기 위한 SkeletonStream 활성화
            objks.SkeletonStream.Enable();

            objks.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(objks_AllFramesReady);
            objks.Start();
        }


        void objks_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // 컬러 이미지 프레임을 담을 변수 imageParam 변수 선언
            ColorImageFrame ImageParam = e.OpenColorImageFrame();

            if (ImageParam == null) return;

            // imageParam에 저장된 데이터의 길이만큼 배열 선언
            byte[] ImageBits = new byte[ImageParam.PixelDataLength];

            // 배열에 데이터 저장
            ImageParam.CopyPixelDataTo(ImageBits);

            // 이미지를 제어할 BitmapSource인 src 선언
            BitmapSource src = null;

            // 너비 높이 픽셀 형식 생성
            src = BitmapSource.Create(ImageParam.Width,
                                        ImageParam.Height,
                                        96, 96,
                                        PixelFormats.Bgr32,
                                        null,
                                        ImageBits,
                                        ImageParam.Width * ImageParam.BytesPerPixel);
            // 이미지에 저장
            image1.Source = src;
        }

        // 키를 잴 때 사용할 정보를 저장할 변수 초기화
        Point pHead = new Point();
        Point pLeft = new Point();
        Point pRight = new Point();
        float fHeadZ = 0;
        float fHeadY = 0;
        float fLeftY = 0;
        float fRightY = 0;

        private void objks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // 스켈레톤의 정보를 저장할 변수 sf 선언
            SkeletonFrame sf = e.OpenSkeletonFrame();

            if (sf == null) return;

            // 정보의 길이만큼 배열 선언
            Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];

            // 배열에 정보를 저장
            sf.CopySkeletonDataTo(skeletonData);

            // 사람까지의 거리 정보를 depthImageFrame에 저장
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    // 스켈레톤데이터 하나씩 추출해 반복
                    foreach (Skeleton sd in skeletonData)
                    {
                        // 추출한 데이터와 스켈레톤이 추적한 상태가 같을 때 실행
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // 머리와 양 발의 위치정보와 거리정보를 변수에 저장하기 위한 반복문 생성
                            // 관절들 중 하나를 추출해 반복
                            foreach (Joint joint in sd.Joints)
                            {
                                DepthImagePoint depthPoint;
                                depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);

                                switch (joint.JointType)
                                {
                                    case JointType.Head:
                                        // 이미지의 폭과 관절의 X축을 곱하고 사람을 측정한 폭을 나눈다
                                        pHead.X = (int)(image1.Width * depthPoint.X / depthImageFrame.Width);
                                        // 이미지의 높이와 관절의 Y축을 곱하고 사람을 측정한 높이를 나눈다
                                        pHead.Y = (int)(image1.Height * depthPoint.Y / depthImageFrame.Height);
                                        // 관절의 Y축을 사람을 측정한 높이로 나눈다
                                        fHeadY = (float)depthPoint.Y / depthImageFrame.Height;

                                        fHeadZ = (float)joint.Position.Z;
                                        break;
                                    case JointType.FootLeft:
                                        pLeft.X = (int)(image1.Width * depthPoint.X / depthImageFrame.Width);
                                        pLeft.Y = (int)(image1.Height * depthPoint.Y / depthImageFrame.Height);
                                        fLeftY = (float)depthPoint.Y / depthImageFrame.Height;
                                        break;
                                    case JointType.FootRight:
                                        pRight.X = (int)(image1.Width * depthPoint.X / depthImageFrame.Width);
                                        pRight.Y = (int)(image1.Height * depthPoint.Y / depthImageFrame.Height);
                                        fRightY = (float)depthPoint.Y / depthImageFrame.Height;
                                        break;
                                }
                            }
                            CalcHeight();
                        }
                    }
                }
            }
        }
        void CalcHeight()
        {

            rectangle1.Height = (pLeft.Y + pRight.Y) / 2 - pHead.Y;
            // 캔버스 활성화

            rectangle1.Visibility = Visibility.Visible;
            // 계산한 사람의 위치로 캔버스를 이동
            Canvas.SetLeft(rectangle1, pHead.X);
            Canvas.SetTop(rectangle1, pHead.Y);

            double dbVal = (fLeftY + fRightY) / 2 - fHeadY;

            textBlock3.Text = String.Format("높이: {0:0} cm", dbVal * (fHeadZ * 100) - fHeadZ * 2);

            

            if (int.Parse(restrict.Text) < int.Parse(textBlock3.Text))
            {
                // 스피커 통과
            }
            else
            {
                // 스피커 실패
            }

        }
    }
}
