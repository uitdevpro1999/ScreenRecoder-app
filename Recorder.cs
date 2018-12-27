using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Accord.DirectSound;
using Accord.Video;
using Accord.Audio;
using Accord.Audio.Filters;
using Accord.Audio.Formats;
using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using Accord.DataSets;

namespace ScreenRecorder_2
{
    class Recorder
    {
        private Rectangle _CaptureRegion;
       // private Rectangle _CameraRegion;
        private bool _isRecording;
        private bool _isPausing;
        private bool _RecordingScreen;
        private bool _RecordingCamera;
        private bool _RecordingMicrophone;
        private bool _RecordingAudio;
        private System.Drawing.Size _VideoOutputSize;
        private int _FrameRate;
        private int _BitRate;
        private string _FileName;
        private string _FilePath;
        private int _AudioVolumme;
        private int _MicrophoneVolumme;

        private FilterInfoCollection _CameraDevices;
        private FilterInfo _CurrentCameraDevice;

        private AudioDeviceCollection _AudioDevices;
        private AudioDeviceInfo _CurrenrAudioDevice;
        private AudioCaptureDevice _AudioSource;

        private AudioDeviceCollection _SpeakerDevices;
        private AudioDeviceInfo _CurrenrSpeakerDevice;
        private AudioCaptureDevice _SpeakerSource;

        private ScreenCaptureStream _ScreenSource;
        private VideoCaptureDevice _CameraSource;

        private Accord.Video.FFMPEG.VideoFileWriter _Writer;
        private DateTime _StartTime;
        private Int64 _ScreenTotalFrames;
        private Int64 _AudioTotalFrames;
        private Bitmap _ScreenBitmap;
        private Signal _MicrophoneSignal;
        private Signal _SpeakerSignal;

        private DateTime presentTime = DateTime.MinValue;
        private DateTime oldTime = DateTime.MinValue;

        public ScreenCaptureStream ScreenSource
        {
            get
            {
                return this._ScreenSource;
            }
        }

        public VideoCaptureDevice CameraSource
        {
            get
            {
                return this._CameraSource;
            }
        }


        public FilterInfoCollection CameraDevices
        {
            get
            {
                return _CameraDevices;
            }
        }

        public FilterInfo CurrentCameraDevice
        {
            get
            {
                return _CurrentCameraDevice;
            }
            set
            {
                _CurrentCameraDevice = value;
            }
        }

        public Rectangle CaptureRegion
        {
            get
            {
                return _CaptureRegion;
            }
            set
            {
                _CaptureRegion = value;
            }
        }

        public Recorder()
        {
            _CaptureRegion = new Rectangle(0, 0, 480, 360);
            _isRecording = false;
            _isPausing = false;
            _RecordingScreen = false;
            _RecordingCamera = false;
            _RecordingMicrophone = false;
            _RecordingAudio = false;
            _FrameRate = 60;
            _BitRate = 1200*1000;
            _FileName = "Video.avi";
            _FilePath = "C:\\";

            _CameraDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _CurrentCameraDevice = GetCameraDevice(0);
            _CameraSource = new VideoCaptureDevice(_CurrentCameraDevice.MonikerString);

            _ScreenSource = new ScreenCaptureStream(_CaptureRegion);

            _AudioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            _CurrenrAudioDevice = GetAudioDevice(0);
          
            _Writer = new VideoFileWriter();
            _StartTime = DateTime.MinValue;
            _ScreenTotalFrames = 0;
            _AudioTotalFrames = 0;
        }

        public Recorder( Rectangle area, bool recordmicro, bool recordaudio, int framerate,  int bitrate, string filepath, string filename )
        {
            _CaptureRegion = new Rectangle(area.X, area.Y, area.Width, area.Height);                    
            _RecordingScreen = true;
            _RecordingMicrophone = recordmicro;
            _RecordingAudio = recordaudio;
            _FrameRate = framerate;
            _BitRate = bitrate;
            _FileName = filename;
            _FilePath = filepath;


            _CameraDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _CurrentCameraDevice = GetCameraDevice(0);
            _CameraSource = new VideoCaptureDevice(_CurrentCameraDevice.MonikerString);

            _ScreenSource = new ScreenCaptureStream(_CaptureRegion);

            _AudioDevices = new AudioDeviceCollection(AudioDeviceCategory.Capture);
            _CurrenrAudioDevice = GetAudioDevice(0);

            _Writer = new VideoFileWriter();
            _StartTime = DateTime.MinValue;
            _ScreenTotalFrames = 0;
            _AudioTotalFrames = 0;
        }
        public void Start()
        {   
            _Writer.Width = _CaptureRegion.Width;
            _Writer.Height = _CaptureRegion.Height;
            _Writer.FrameRate = _FrameRate;
            _Writer.BitRate = _BitRate;
            _Writer.VideoCodec = VideoCodec.Default;

            _Writer.Open(_FilePath + _FileName);
            
            if (_RecordingScreen)
            {
                _ScreenSource.FrameInterval = 20;
                _ScreenSource.NewFrame += Screen_NewFrame;
                _ScreenSource.Start(); 
            }         

            if (_RecordingMicrophone)
            {   
                _AudioSource = new AudioCaptureDevice(_CurrenrAudioDevice);
                _AudioSource.NewFrame += new EventHandler<Accord.Audio.NewFrameEventArgs>(Microphone_NewFrame);
                _AudioSource.Start();              
            }
            if (_RecordingAudio)
            {
                Trace.WriteLine("start recoder Speaker");
                _SpeakerSource = new AudioCaptureDevice(_CurrenrSpeakerDevice);
                _SpeakerSource.NewFrame += new EventHandler<Accord.Audio.NewFrameEventArgs>(Speaker_NewFrame);
                _SpeakerSource.Start();
            }
        }

        public void Stop()
        {
            _ScreenSource.SignalToStop();
            _ScreenSource.Stop();
            _CameraSource.SignalToStop();
            _CameraSource.Stop();            
            _Writer.Close();
            _Writer.Dispose();         
        }

        public void StartCaptureCapmera()
        {           
            _CameraSource.NewFrame += Camera_NewFrame;
            _CameraSource.Start();
        }

        public void StopCaptureCapmera()
        {
            _CameraSource.Stop();
        }

        private  void Screen_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {   
            if (_StartTime == DateTime.MinValue)
            {
                 _StartTime = DateTime.Now;
            }
            _ScreenTotalFrames++;
            _Writer.WriteVideoFrame((Bitmap)eventArgs.Frame.Clone(), DateTime.Now - _StartTime);
            presentTime = DateTime.Now;

            if(_MicrophoneSignal!=null && (presentTime - oldTime).TotalMilliseconds >= 77*2)
            {
                Trace.WriteLine("Time: " + _MicrophoneSignal.Duration.TotalMilliseconds);
                if (_RecordingMicrophone && _MicrophoneSignal != null)
                {
                    _Writer.WriteAudioFrame(_MicrophoneSignal);
                }

                if (_RecordingAudio && _SpeakerSignal != null)
                {
                    Trace.WriteLine("speaker");
                    _Writer.WriteAudioFrame(_SpeakerSignal);
                }
                oldTime = presentTime;
            }
           
        }

        private void Speaker_NewFrame(object sender, Accord.Audio.NewFrameEventArgs eventArgs)
        {
            Trace.WriteLine("Speaker + frame");
            _SpeakerSignal = eventArgs.Signal;
        }

        private void Microphone_NewFrame(object sender, Accord.Audio.NewFrameEventArgs eventArgs)
        {
            _MicrophoneSignal = eventArgs.Signal;
            float n = 1f;
            var volume = new VolumeFilter(n);
            volume.ApplyInPlace(_MicrophoneSignal);         
        }

        private void Audio_Error(object sender, AudioSourceErrorEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private FilterInfo GetCameraDevice(int index)
        {
            if (_CameraDevices != null && index >= 0 && index < _CameraDevices.Count)
            {
                return _CameraDevices[index];
            }
            return null;
        }

        private AudioDeviceInfo GetAudioDevice(int index)
        {
            if (_AudioDevices != null && index >= 0 && index < _AudioDevices.Count<AudioDeviceInfo>())
            {
                return _AudioDevices.ElementAt<AudioDeviceInfo>(index);
            }
            return null;
        }


        private void Camera_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {             
            //Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //MemoryStream ms = new MemoryStream();
            //bitmap.Save(ms, ImageFormat.Bmp);
            //ms.Seek(0, SeekOrigin.Begin);
            //bi.StreamSource = ms;
            //bi.EndInit();
            //bi.Freeze();  
        }
    }
}
