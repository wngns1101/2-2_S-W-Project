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
using Microsoft.Kinect;     // Kinect 라이브러리 사용을 위한 네임스페이스 참조

namespace Kinect_week66
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeKS();  // 키넥스 센서 초기화를 위한 함수
        }

        KinectSensor ks = null;  // 키넥트 센서 변수 선언

        void InitializeKS()
        {
            ks = KinectSensor.KinectSensors[0];  // ks는 첫번째 키넥트 센서를 가리킨다.

            ks.ColorStream.Enable();  // 키넥트 센서의 컬러스트림 활성화
            ks.ColorFrameReady += new 
                EventHandler<ColorImageFrameReadyEventArgs>(ks_ColorFrameReady);
                                           // 컬러스트림 처리를 위한 이벤트 핸들러 등록
            ks.DepthStream.Enable();  // 키넥트 센서의 뎁스스트림 활성화
            ks.SkeletonStream.Enable();  // 키넥트 센서의 스텔레톤스트림 활성화
            ks.AllFramesReady += new
                EventHandler<AllFramesReadyEventArgs>(ks_AllFramesReady);
                                    // 뎁스스트림과 스켈레톤스트림 처리를 위한 이벤트 핸들러 등록
            ks.Start();  // 키넥트 동작
        }      // 키넥트 센서 초기화를 위한 함수

        void ks_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();
            
            if (ImageParam == null) return;  
            
            byte[] ImageBits = new byte[ImageParam.PixelDataLength]; 
            ImageParam.CopyPixelDataTo(ImageBits);
            
            BitmapSource src = BitmapSource.Create(ImageParam.Width,
                                                    ImageParam.Height,
                                                    96, 96,
                                                    PixelFormats.Bgr32,
                                                    null,
                                                    ImageBits,
                                                    ImageParam.Width * ImageParam.BytesPerPixel);

            image1.Source = src;
        }
        // ColorStream을 처리하기 위한 이벤트 함수

        void ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            // 변수 sf에 전달된 SkeletonFrame을 저장한다.

            if (sf == null) return; // 정상적으로 전달되지 않았을 경우 return

            Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];
            // SkeletonFrame에 포함된 Skeleton 데이터 저장을 위한 배열 선언.
            sf.CopySkeletonDataTo(skeletonData);
            // SkeletonFrame에 포함된 Skeleton 데이터를 위에서 할당한 배열에 복사한다.

            using (DepthImageFrame dif = e.OpenDepthImageFrame())
            {       // dif에 전달된 DepthImageFrame을 저장한다.
                if (dif != null) // 정상적으로 전달됬을 경우 실행
                {
                    foreach (Skeleton sd in skeletonData)  
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked) // 스켈레톤이 추적중인 상태라면,
                        {
                            Joint joint = sd.Joints[JointType.Head];  // joint 변수에 스켈레톤의 헤드조인트 저장

                            DepthImagePoint headPoint = 
                                 dif.MapFromSkeletonPoint(joint.Position); 
                            // 헤드조인트를 DepthImage 상의 좌표로 변환. 

                            Point point = new Point((int)(image1.Width * headPoint.X / dif.Width),
                                                    (int)(image1.Height * headPoint.Y / dif.Height));
                            textBlock1.Text = String.Format("X:{0:0.00} Y:{1:0.00}", point.X, point.Y);
                            // DepthImage상의 좌표를 img 컴포넌트 크기에 맞춰 변환. 출력.

                            Canvas.SetLeft(ellipse1, point.X);
                            Canvas.SetTop(ellipse1, point.Y);
                            // circle 컴포넌트를 위에서 구한 좌표값으로 이동.

                            float headPosition = (float)headPoint.Y / (float)dif.Height;
                            // DepthImageFrame 상에서 머리 위치의 비율을 저장.

                            if (headPosition < 0.2f)  // 0% ~ 20%에 있다면,
                            {
                                try // ElevationAngle이 27보다 크면 예외를 던짐.
                                {
                                    ks.ElevationAngle += 2; // 2도 증가
                                }
                                catch (Exception ex) // 예외발생 시 아무런 동작도 하지 않는다.
                                {
                                }
                            }

                            if (headPosition > 0.4f) // 40% ~ 100% 에 있다면,
                            {
                                try // ElevationAngle이 -27보다 작으면 예외를 던짐
                                {
                                    ks.ElevationAngle -= 2; // 2도 감소
                                }
                                catch (Exception ex) // 예외발생 시 아무런 동작도 하지 않는다.
                                {
                                }
                            }
                        }

                    }
                }
            }
        }
        // DepthStream과 SkeletonStream을 처리하기 위한 이벤트 함수
    }
}
