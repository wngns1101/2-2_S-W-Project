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
// 참조 추가
using Microsoft.Kinect;
using System.Windows.Threading;

namespace Kinect_Camera
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // timer 선언
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();

            // 카메라 각도에 대한 값을 텍스트 블록에 출력하기 위해 추가
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        // timer에서 지정한 0.1초가 되면 이벤트 발생
        void timer_Tick(object sender, EventArgs e)
        {
            textBlock1.Text = objKs.ElevationAngle.ToString();
        }

        // kinect 객체 objKs 선언
        KinectSensor objKs = null;

        // kinect 초기화 메소드
        private void InitializeNui()
        {
            // Kinect 인식
            objKs = KinectSensor.KinectSensors[0];

            // 컬러 사용
            objKs.ColorStream.Enable();

            // 컬러 이미지의 프레임을 받아주는 이벤트 활성화
            objKs.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(objKs_ColorFrameReady);

            objKs.Start();
        }

        void objKs_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // 컬러이미지 프레임을 받을 변수 선언
            ColorImageFrame ImageParam = e.OpenColorImageFrame();
            if(ImageParam == null)
            {
                return;
            }

            // 컬러이미지의 픽셀 값 만큼 배열 선언
            byte[] ImageBits = new byte[ImageParam.PixelDataLength];

            // 만들어진 배열에 픽셀 데이터 복사
            ImageParam.CopyPixelDataTo(ImageBits);
            
            // 폭과 너비, 픽셀 형식, 이미지 배열 생성
            BitmapSource src = null;
            src = BitmapSource.Create(
                    ImageParam.Width,
                    ImageParam.Height,
                    96, 96,
                    PixelFormats.Bgr32,
                    null,
                    ImageBits,
                    ImageParam.Width * ImageParam.BytesPerPixel);
            
            //xaml의 이미지에 저장
            image1.Source = src;
        }
        
        // 카메라 각도를 1 올려주는 메소드
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objKs.ElevationAngle++;
            } catch (Exception ex)
            {

            }
        }

        // 카메라 각도를 1 낮춰주는 메소드
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objKs.ElevationAngle--;
            }
            catch (Exception ex)
            {

            }
        }

        // 카메라 각도를 최대로 올려주는 메소드
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objKs.ElevationAngle = objKs.MaxElevationAngle;
            }
            catch (Exception ex)
            {

            }
        }

        // 카메라 각도를 최소로 내려주는 메소드
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objKs.ElevationAngle = objKs.MinElevationAngle;
            }
            catch (Exception ex)
            {

            }
        }

        // 텍스트의 숫자만큼 각도 조절하는 메소드
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objKs.ElevationAngle = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
