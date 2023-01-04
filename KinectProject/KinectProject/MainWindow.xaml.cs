using Microsoft.Kinect;
using MySqlX.XDevAPI.Common;
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
using System.Windows.Threading;   // 타이머 사용을 위해 추가
using System.Speech.Synthesis;    // 음성 출력을 위해 추가

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

            // 타이머 초기화
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_tick);

        }

        // 키넥트 변수 선언
        KinectSensor objks = null;
        // 타이머 객체 생성
        DispatcherTimer timer = new DispatcherTimer();
        // SpeechSynthesizer 객체 생성
        SpeechSynthesizer ss = new SpeechSynthesizer();
   
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

            // df에 전달된 DepthImageFrame을 저장한다.
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    // 스켈레톤데이터 하나씩 추출해 반복
                    foreach (Skeleton sd in skeletonData)
                    {
                        // 스켈레톤이 추적중인 상태라면
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (timer.IsEnabled == false && isChecked == false)
                                timer.Start();

                            // joint 변수에 스켈레톤의 헤드조인트 저장
                            foreach (Joint joint in sd.Joints)
                            {
                                DepthImagePoint depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);

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
                            lastTrackedTime = DateTime.Now.Ticks;
                        }
                        else
                        {
                            // 추적이 끊기고 3초 유지, 새로운 사람을 다시 측정할 수 있게 변수 초기화
                            if (timer.IsEnabled == false && (lastTrackedTime + 3000 * 10000 < DateTime.Now.Ticks))
                            {
                                stable_count = 0;
                                isChecked = false;
                                heightResult.Text = "통과여부: ";
                            }
                        }
                    }
                }
            }
        }

        void CalcHeight()
        {

            // 에러 발생 : rectangle1.Height = (pLeft.Y + pRight.Y) / 2 - pHead.Y;
            if((pLeft.Y + pRight.Y)/2 - pHead.Y > 0) 
            {
                rectangle1.Height = (pLeft.Y + pRight.Y)/2 - pHead.Y;
            } 
            else 
            {
                rectangle1.Height = 0;
            }

            // 캔버스 활성화

            rectangle1.Visibility = Visibility.Visible;
            // 계산한 사람의 위치로 캔버스를 이동
            Canvas.SetLeft(rectangle1, pHead.X);
            Canvas.SetTop(rectangle1, pHead.Y);

            double dbVal = (fLeftY + fRightY) / 2 - fHeadY;
            
            if(dbVal * (fHeadZ * 100) - fHeadZ * 2 < 0)
            {
                height.Text = "입력된 키가 음수입니다";
            }
            else{
                // 키 소수점 2번째 자리에서 반올림 했습니다.
                height.Text = String.Format("키:{0}", Math.Round((dbVal * (fHeadZ * 100) - fHeadZ * 2) *100)/100);
            }
        }

        double old_height = 0;  // 이전 타이머에서 측정된 키 값을 저장할 변수
        const int HEIGHT_LIMIT = 150;  // 키 제한값 상수
        long lastTrackedTime = 0;  // 스켈레톤이 마지막으로 추적된 시간 저장
        int stable_count = 0;  // height 값이 안정적으로 유지된 횟수
        bool isChecked = false;  // 키 판별이 완료된 스켈레톤이면 true, 아니면 false.
        

        void timer_tick(object sender, EventArgs e)
        {
            double dbVal = (fLeftY + fRightY) / 2 - fHeadY;
            double calc_height = dbVal * (fHeadZ * 100) - fHeadZ * 2;

            if (old_height - 3 <= calc_height && calc_height <= old_height + 3)
                stable_count++;
            else
                stable_count = 0;

            old_height = Math.Round(calc_height);  // 반올림하여 소수점 제거
            
            if (stable_count >= 20) // height 값이 2초 이상 안정적으로 유지되면
            {
                if (calc_height >= HEIGHT_LIMIT)
                {
                    heightResult.Text = "통과여부: 통과";
                    ss.Speak("Tong Koa");
                }
                else
                {
                    heightResult.Text = "통과여부: 차단";
                    ss.Speak("Cha Dan");
                }
                
                isChecked = true;
                timer.Stop();
            }
        }
    }
}
