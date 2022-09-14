using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Kinect_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            KinectSensor objKS;
            objKS = KinectSensor.KinectSensors[0];

            while (true)
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

                Console.WriteLine(strKinectInfo);

                if (Console.ReadKey().Key == ConsoleKey.Escape) break;
            }
        }
    }
}
