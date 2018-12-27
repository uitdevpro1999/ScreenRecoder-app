using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.DirectSound;
using Accord.Video;
using Accord.Audio;
using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System.Drawing;
using Microsoft.Win32;

namespace ScreenRecorder_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public static string _VideoPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string _videoName = "Vide0";
        public static string _VideoType = ".avi";
        public static int _VideoFramerate = 60;
        public static int _videoBitrate = 5000000;
        public static string _AudioPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string _AudioName = "Audio";
        public static string _AudioType = ".mp3";
        public static int _AudioBitrate = 4096;
        public static string _CapturePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string _CaptureName = "ScreenShot";
        public static string _CaptureType = ".png";
        public static SettingWindown setWindown;
        public static RecorderWindown RecWindown;
     
        public MainWindow()
        {    
            InitializeComponent();
           
           

        }

        private void RecWindown_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void btsetting_click(object sender, RoutedEventArgs e)
        {
            setWindown = new SettingWindown();
            setWindown.Show();
        }

        private void fullscreen_click(object sender, RoutedEventArgs e)
        {;
            _videoName = @"\Video_" + Convert.ToDateTime(DateTime.Now).ToString("MMM_dd_yyyy_HH_mm_ss");
            RecWindown = new RecorderWindown(CaptureMode.Fullscreen, _VideoPath, _videoName, _VideoType, _VideoFramerate, _videoBitrate, _CapturePath, _CaptureType, this);
            this.WindowState = WindowState.Minimized;
            RecWindown.Show();
        }
        private void custtom_click(object sender, RoutedEventArgs e)
        {         
            _videoName = @"\Video_" + Convert.ToDateTime(DateTime.Now).ToString("MMM_dd_yyyy_HH_mm_ss");
            RecWindown = new RecorderWindown(CaptureMode.SelectRegion, _VideoPath, _videoName, _VideoType, _VideoFramerate, _videoBitrate, _CapturePath, _CaptureType, this);
            this.WindowState = WindowState.Minimized;
            RecWindown.Show();
         
        }     
    }
}
