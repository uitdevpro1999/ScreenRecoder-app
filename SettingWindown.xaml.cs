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
using System.Windows.Shapes;

namespace ScreenRecorder_2
{
    /// <summary>
    /// Interaction logic for WindownSetting.xaml
    /// </summary>
    public partial class SettingWindown : Window
    {
        public SettingWindown()
        {
            InitializeComponent();
            video_path.Text = MainWindow._VideoPath;
            video_type.Text = MainWindow._VideoType;
            video_framerate.Text = MainWindow._VideoFramerate.ToString();
            video_bitrate.Text = MainWindow._videoBitrate.ToString();

            audio_path.Text = MainWindow._AudioPath;
            audio_type.Text = MainWindow._AudioType;
            audio_bitrate.Text = MainWindow._AudioBitrate.ToString();

            capture_path.Text = MainWindow._CapturePath;
            capture_type.Text = MainWindow._CaptureType;


        }

        private void click_stop(object sender, RoutedEventArgs e)
        {
            // Rec.Stop();         
            this.WindowState = WindowState.Minimized;
        }

        private void click_start(object sender, RoutedEventArgs e)
        {//
            //Rec.Start();
        }

        private void video_path_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                video_path.Text = folderDlg.SelectedPath;
            }
        }

        private void audio_path_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                audio_path.Text = folderDlg.SelectedPath;
            }
        }


        private void capture_path_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                capture_path.Text = folderDlg.SelectedPath;
            }
        }

        private void save_click(object sender, RoutedEventArgs e)
        {

            MainWindow._VideoPath = video_path.Text;
            MainWindow._VideoFramerate = Convert.ToInt32(video_framerate.Text);
            MainWindow._videoBitrate = Convert.ToInt32(video_bitrate.Text);
            MainWindow._VideoType = video_type.Text;

            MainWindow._CapturePath = capture_path.Text;
            MainWindow._CaptureType = capture_type.Text;

            this.Close();
        }
    }
}
