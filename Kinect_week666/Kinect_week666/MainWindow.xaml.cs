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
using Microsoft.Kinect;  // 키넥트 라이브러리 사용을 위한 네임스페이스 참조
using System.Windows.Threading;  // DispatcherTimer 클래스 사용을 위한 네임스페이스 참조

namespace Kinect_week666
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(); // DispatcherTimer 객체 사용을 위한 timer 변수 선언
        public MainWindow()
        {
            InitializeComponent();
            InitializeKS(); // 키넥트 초기화 함수 호출
            timer.Interval = TimeSpan.FromMilliseconds(100); // timer의 Interval 필드를 0.1초로 초기화
            timer.Tick += new EventHandler(find_head); // 0.1초마다 실행될 이벤트 핸들러 등록
            saveTime = DateTime.Now.Ticks + 2000 * 10000; // timer 동작에 도움을 주는 변수 saveTime 초기화
        }

        KinectSensor ks = null;     // 키넥트 센서 저장을 위한 변수

        void InitializeKS()
        {
            ks = KinectSensor.KinectSensors[0];

            ks.ColorStream.Enable();
            ks.ColorFrameReady += new
                EventHandler<ColorImageFrameReadyEventArgs>(ks_ColorFrameReady);

            ks.DepthStream.Enable();
            ks.SkeletonStream.Enable();
            ks.AllFramesReady += new
                EventHandler<AllFramesReadyEventArgs>(ks_AllFramesReady);

            ks.Start();
        }  // 키넥트 센서 초기화 함수

        int angle = -27;  // find_head에서 각도 저장을 위해 사용되는 변수
        void find_head(object sender, EventArgs e)
        {
            textBlock1.Text = ks.ElevationAngle.ToString();  // 현재 카메라 각도 출력

            try
            {
                angle += 2;  // 변수 angle에 저장된 값 2 증가 
                ks.ElevationAngle = angle;  // 증가된 angle 값으로 카메라 각도 변경
            }
            catch (Exception ex)
            {
            }

            if (angle >= 27)  // ElevationAngle의 최대값인 27 이상이 되면
            {
                angle = -27;  // angle값을 ElevationAngle의 최솟값인 -27로 초기화
            }
        }  // 현재 각도 출력, 머리를 찾기 위해 카메라 이동.

        void ks_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();

            if (ImageParam == null) return;

            byte[] imageBits = new byte[ImageParam.PixelDataLength];
            ImageParam.CopyPixelDataTo(imageBits);

            BitmapSource src = BitmapSource.Create(ImageParam.Width,
                                                   ImageParam.Height,
                                                   96, 96,
                                                   PixelFormats.Bgr32,
                                                   null,
                                                   imageBits,
                                                   ImageParam.Width * ImageParam.BytesPerPixel);
            image1.Source = src;
        }  
        // ColorStream처리를 위한 이벤트 핸들러

        long saveTime = 0;  // timer 동작에 도움을 주는 변수 saveTime 선언
        void ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            
            if (sf == null) return;

            Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(skeletonData);

            using (DepthImageFrame dif = e.OpenDepthImageFrame())
            {
                if (dif != null)
                {
                    foreach (Skeleton sd in skeletonData)
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint joint = sd.Joints[JointType.Head];

                            DepthImagePoint headPoint
                                = dif.MapFromSkeletonPoint(joint.Position);

                            Point point = new Point((int)(headPoint.X * image1.Width / dif.Width),
                                                    (int)(headPoint.Y * image1.Height / dif.Height));

                            textBlock1.Text = String.Format("X:{0:0.00} Y:{1:0.00}", point.X, point.Y);

                            Canvas.SetLeft(ellipse1, point.X);
                            Canvas.SetTop(ellipse1, point.Y);

                            saveTime = DateTime.Now.Ticks;
                            // 위의 코드들은 헤드조인트를 찾았을 경우에 동작한다. 
                            // 따라서 saveTime에는 헤드조인트를 추적중인 경우 실시간으로 현재시간이 업데이트된다.

                            if (timer.IsEnabled == true)
                                timer.Stop();
                            // 위의 코드들은 헤드조인트를 찾았을 경우에 동작한다.
                            // 따라서 더이상 머리를 찾기위해 카메라를 이동시킬 필요가 없기 때문에
                            // timer가 동작중이라면 중지한다.
                        }
                    }
                }
            }
            if (saveTime + 2000 < DateTime.Now.Ticks) // 추적이 종료되고 2초가 지난 경우,
            {
                if (timer.IsEnabled == false)  // timer가 동작중이지 않다면,
                    timer.Start();  // timer를 동작시킨다.
            }
        }
        // DepthStream과 SkeletonStream처리를 위한 이벤트 핸들러
    }
}
