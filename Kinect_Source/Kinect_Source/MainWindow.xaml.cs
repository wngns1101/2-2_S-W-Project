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

namespace Kinect_Source
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor objKS;
        public MainWindow()
        {
            InitializeComponent();
            objKS = KinectSensor.KinectSensors[0];
            KinectSensor.KinectSensors.StatusChanged +=
                new EventHandler<StatusChangedEventArgs>(Kinects_StatusChanged);
        }

        void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            TextBlock1.Text = objKS.Status.ToString();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            int iNumKinect = KinectSensor.KinectSensors.Count;


            String strKinectInfo = null;
            strKinectInfo += iNumKinect.ToString("키넥트 수: 0개");
            strKinectInfo += "\n";
            strKinectInfo += objKS.UniqueKinectId;
            strKinectInfo += "\n";
            strKinectInfo += objKS.DeviceConnectionId;
            strKinectInfo += "\n";
            strKinectInfo += objKS.Status.ToString();
            strKinectInfo += "\n";
            strKinectInfo += "Kinect 동작 여부: " + objKS.IsRunning;

            TextBlock1.Text = strKinectInfo;
        }

    }
}
