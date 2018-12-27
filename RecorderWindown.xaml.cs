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
using System.Diagnostics;
using System.IO;
using Accord.Video.FFMPEG;


namespace ScreenRecorder_2
{
    /// <summary>
    /// Interaction logic for CaptureRegion.xaml
    /// </summary>
    /// 
   
    public enum CaptureMode
    {
        Fullscreen,
        SelectRegion,
        AroundMouse
    }

    public enum ResizeType
    {
        None,
        LT,
        T,
        RT,
        R,
        RB,
        B,
        LB,
        L,
        All,
        Symmetry
    }

    public enum DrawMode
    {
        None,
        Rectangle,
        Ellipse,
        Line,
        FreeDraw10,
        FreeDraw5,
        FreeDraw2
    }

    public partial class RecorderWindown : Window
    {    

        private int stepDoubleClick = 0;
        private DateTime startDoubleClick;
        private TextBlock infor = new TextBlock();

        private CaptureMode Mode;
        private ResizeType resizeType;
        private double SizeButtom;
        private Point MouseDownPostion;
        private Point MouseUpPostion;
        private Point CurrentPostion;
        private bool Resizing = false;
        private bool ResizeCompleted = false;
        private bool Selecting = false;
        private bool SelectCompleted = false;
        private bool toolbar_Removing = false;
        private double MAXWIDTH;
        private double MAXHEIGHT;
        private double MINWIDTH;
        private double MINHEIGHT;
        private Canvas EventCanvas;

        private bool Pausing = false;
        private bool Recording = false;
        private bool Timing = false;
        private bool isRecordWebcam = false;
        private bool isRecordMicrophone = false;
        private bool isRecordAudio = false;
        private bool MovingCamerawiewer = false;
        private DrawMode Drawmode = DrawMode.None;
        private bool Drawing = false;

        private string video_path;
        private string video_name;
        private string video_type;
        private int video_framerate;
        private int video_bitrate;
        private string capture_path;
        private string capture_type;

        private Image camerawiewer;
        private Rectangle shape_rect;
        private Ellipse shape_ellip;
        private Line shape_line;
        private List<Line> shape_free;
        private Point CurPos;
        private Window mainwindown;
        private Brush DrawColor = Brushes.Red;

        private DateTime mousedownTime = DateTime.MaxValue;
        private System.Windows.Point mousedownPos;
        private Recorder REC, CAMERA_REC;
       

        public RecorderWindown(CaptureMode mode, string video_path, string video_name, string video_type, int video_framerate, int video_bitrate, string capture_path, string capture_type, Window mainwindown )
        {
            InitializeComponent();          
            this.video_path = video_path;
            this.video_name = video_name;
            this.video_type = video_type;
            this.video_framerate = video_framerate;
            this.video_bitrate = video_bitrate;
            this.capture_path = capture_path;
            this.capture_type = capture_type;
            this.mainwindown = mainwindown;
            SizeButtom = 20;
            MINWIDTH = MINHEIGHT = 100;
            region.MinHeight = MINHEIGHT;
            region.MinWidth = MINWIDTH;
            border.StrokeDashArray = new DoubleCollection() { 4,3};
            MAXWIDTH = System.Windows.SystemParameters.PrimaryScreenWidth;
            MAXHEIGHT = System.Windows.SystemParameters.PrimaryScreenHeight;
            EventCanvas = new Canvas();          
            EventCanvas.MouseDown += eventcanvas_down;
            EventCanvas.MouseUp += eventcanvas_up;
            EventCanvas.MouseMove += eventcanvas_move;          
            fullregion.Children.Add(EventCanvas);       
            Canvas.SetLeft(toolbar, MAXWIDTH - 40);
            switch (mode)
            {
                case CaptureMode.Fullscreen:
                    FullScreen();
                    break;

                case CaptureMode.SelectRegion:
                    SelectRegion();
                    break;

                case CaptureMode.AroundMouse:
                    break;
            }
        }

       

        private void SelectRegion()
        {
            region.Opacity = 0;
            EventCanvas.Background = Brushes.White;
            EventCanvas.Opacity = 0.01;
            toolbar.Opacity = 0;
            Cursor = Cursors.Cross;
        }


        private void FullScreen()
        {
            region.Margin = new Thickness(0);
        }

        private void LT_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeNWSE;
        }

        private void LT_down(object sender, MouseButtonEventArgs e)
        {          
            if (!ResizeCompleted && !Resizing)
            {   
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.LT;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);            
            }            
        }
        
        private void T_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {   
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.T;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void T_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeNS;
        }

        private void RT_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.RT;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }           
        }

        private void RT_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeNESW;
        }

        private void R_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.R;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void R_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeWE;
        }

        private void RB_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.RB;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void RB_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeNWSE;
        }

        private void B_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.B;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void B_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeNS;
        }

        private void LB_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.LB;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void LB_enter(object sender, MouseEventArgs e)
        {
            if(!ResizeCompleted)
                 Cursor = Cursors.SizeNESW;
        }

        private void L_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.L;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void L_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
                Cursor = Cursors.SizeWE;
        }

        private void All_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted && !Resizing)
            {
                Resizing = true;
                CurrentPostion = e.GetPosition(this);
                resizeType = ResizeType.All;
                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);

                if (stepDoubleClick == 0)
                {
                    startDoubleClick = DateTime.Now;
                }
                stepDoubleClick++;             
            }
        }

        private void All_enter(object sender, MouseEventArgs e)
        {
            if (!ResizeCompleted)
            {
                Cursor = Cursors.SizeAll;
                infor.Opacity = 1;
                infor.Margin = new Thickness(All.TranslatePoint(new Point(0, 0), this).X - 160, All.TranslatePoint(new Point(0, 0), this).Y + 70, 0, 0);
                infor.Text = "Hold mouse to move, right mouse to complete";
                infor.FontSize = 18;
                infor.Foreground = Brushes.White;
                fullregion.Children.Add(infor);
            }       
        }

        private void All_leave(object sender, MouseEventArgs e)
        {
            fullregion.Children.Remove(infor);
        }

        private void All_right_down(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted)
            {
                stepDoubleClick = 0;
                ResizeCompleted = true;
                Cursor = Cursors.Arrow;
                toolbar.Opacity = 1;         
                foreach (Rectangle r in grid.Children)
                {
                    size.Opacity = 0;
                    if (r.Name == "border")
                        r.StrokeDashArray = new DoubleCollection() { 1, 0 };
                    else
                        r.Opacity = 0;
                }
            }
        }


        private void eventcanvas_up(object sender, MouseButtonEventArgs e)
        {
            if (!ResizeCompleted)
            {
                if (Selecting)
                {
                    Selecting = false;
                    SelectCompleted = true;
                    Cursor = Cursors.Arrow;
                    MouseUpPostion = e.GetPosition(this);
                    if (MouseDownPostion == MouseUpPostion)
                    {
                        double left = MouseDownPostion.X - 14 - 50;
                        double top = MouseDownPostion.Y - 14 - 50;
                        double right = MAXWIDTH - MouseDownPostion.X - 50;
                        double bottom = MAXHEIGHT - MouseDownPostion.Y - 50;
                        region.Margin = new Thickness(left, top, right, bottom);
                    }
                    EventCanvas.IsEnabled = false;
                    foreach (Rectangle r in grid.Children)
                    {
                        r.Opacity = 1;
                    }
                    fullregion.Children.Remove(EventCanvas);
                }
                if (Resizing)
                {
                    Resizing = false;
                    fullregion.Children.Remove(EventCanvas);
                    double left = region.Margin.Left >= 0 ? region.Margin.Left : 0;
                    double top = region.Margin.Top >= 0 ? region.Margin.Top : 0;
                    double right = region.Margin.Right >= 0 ? region.Margin.Right : 0;
                    double bottom = region.Margin.Bottom >= 0 ? region.Margin.Bottom : 0;
                    region.Margin = new Thickness(left, top, right, bottom);
                }

            }

            if (Drawing)
            {
                Drawing = false;
                Remove_EventCanvas();
            }
            
            show_hide_up(sender, e);
            camerawiewer_up(sender, e);
        }

        private void eventcanvas_down(object sender, MouseButtonEventArgs e)
        {   
            if (!SelectCompleted && !Selecting)
            {   
                Selecting = true;
                MouseDownPostion = e.GetPosition(this);
                region.Opacity = 1;
                foreach (Rectangle r in grid.Children)
                {
                    if (r.Name != "border")
                    {
                        r.Opacity = 0;                      
                    }
                }                     
               
            }
            if (Drawmode == DrawMode.Rectangle)
            {              
                shape_rect = new Rectangle();
                shape_rect.Uid = "shape";
                region.Children.Add(shape_rect);
                MouseDownPostion = e.GetPosition(region);
                shape_rect.Fill = DrawColor;
                shape_rect.Opacity = 0.5;
                Drawing = true;
            }

            if(Drawmode == DrawMode.Ellipse)
            {
                shape_ellip = new Ellipse();
                shape_ellip.Uid = "shape";
                region.Children.Add(shape_ellip);
                MouseDownPostion = e.GetPosition(region);
                shape_ellip.Fill = DrawColor;               
                shape_ellip.Opacity = 0.5;
                Drawing = true;
            }

            if (Drawmode == DrawMode.Line)
            {
                shape_line = new Line();
                shape_rect.Uid = "shape";
                region.Children.Add(shape_line);
                MouseDownPostion = e.GetPosition(region);
                shape_line.Stroke = DrawColor;
                shape_line.StrokeThickness = 6;
                shape_line.Opacity = 0.5;
                Drawing = true;
            }
            if (Drawmode == DrawMode.FreeDraw10 || Drawmode == DrawMode.FreeDraw5 || Drawmode == DrawMode.FreeDraw2)
            {
                shape_free = new List<Line>();               
                CurPos = e.GetPosition(region);                
                Drawing = true;
            }         
        }

        private void eventcanvas_move(object sender, MouseEventArgs e)
        {   
            if (Selecting && !ResizeCompleted)
            {
                region.Margin = new Thickness(MouseDownPostion.X, MouseDownPostion.Y, MAXWIDTH + 14 - e.GetPosition(this).X, MAXHEIGHT + 14 - e.GetPosition(this).Y);
            }
            if (Resizing && !ResizeCompleted)
            {
                Vector d = e.GetPosition(this) - CurrentPostion;              
                switch (resizeType)
                {
                    case ResizeType.LT:
                        region.Margin = new Thickness(region.Margin.Left + d.X, region.Margin.Top + d.Y, region.Margin.Right, region.Margin.Bottom);
                        break;

                    case ResizeType.T:
                        region.Margin = new Thickness(region.Margin.Left, region.Margin.Top + d.Y, region.Margin.Right, region.Margin.Bottom);
                        break;

                    case ResizeType.RT:
                        region.Margin = new Thickness(region.Margin.Left, region.Margin.Top + d.Y, region.Margin.Right - d.X, region.Margin.Bottom);
                        break;

                    case ResizeType.R:
                        region.Margin = new Thickness(region.Margin.Left, region.Margin.Top, region.Margin.Right - d.X, region.Margin.Bottom);
                        break;

                    case ResizeType.RB:
                        region.Margin = new Thickness(region.Margin.Left, region.Margin.Top, region.Margin.Right - d.X, region.Margin.Bottom - d.Y);
                        break;

                    case ResizeType.B:
                        region.Margin = new Thickness(region.Margin.Left, region.Margin.Top, region.Margin.Right, region.Margin.Bottom - d.Y);
                        break;

                    case ResizeType.LB:
                        region.Margin = new Thickness(region.Margin.Left + d.X, region.Margin.Top, region.Margin.Right, region.Margin.Bottom - d.Y);
                        break;

                    case ResizeType.L:
                        region.Margin = new Thickness(region.Margin.Left + d.X, region.Margin.Top, region.Margin.Right, region.Margin.Bottom);
                        break;
                    case ResizeType.All:
                        region.Margin = new Thickness(region.Margin.Left + d.X, region.Margin.Top +d.Y, region.Margin.Right - d.X, region.Margin.Bottom - d.Y);
                        break;
                    case ResizeType.Symmetry:
                        region.Margin = new Thickness(region.Margin.Left + d.X, region.Margin.Top + d.Y, region.Margin.Right + d.X, region.Margin.Bottom + d.Y);
                        break;                  
                }           
            }
            if (Drawing)
            {
                Vector d = e.GetPosition(region) - MouseDownPostion;
                if (Drawmode == DrawMode.Rectangle)
                {
                    if (e.GetPosition(region).X >= MouseDownPostion.X)
                    {
                        Canvas.SetLeft(shape_rect, MouseDownPostion.X);
                    }
                    else
                    {
                        Canvas.SetLeft(shape_rect, e.GetPosition(region).X );
                    }

                    if (e.GetPosition(region).Y >= MouseDownPostion.Y)
                    {
                        Canvas.SetTop(shape_rect, MouseDownPostion.Y);
                    }
                    else
                    {
                        Canvas.SetTop(shape_rect, e.GetPosition(region).Y);
                    }
                    shape_rect.Width = Math.Abs(d.X) ;
                    shape_rect.Height = Math.Abs(d.Y) ;
                }

                if (Drawmode == DrawMode.Ellipse)
                {
                    if (e.GetPosition(region).X >= MouseDownPostion.X)
                    {
                        Canvas.SetLeft(shape_ellip, MouseDownPostion.X);
                    }
                    else
                    {
                        Canvas.SetLeft(shape_ellip, e.GetPosition(region).X);
                    }

                    if (e.GetPosition(region).Y >= MouseDownPostion.Y)
                    {
                        Canvas.SetTop(shape_ellip, MouseDownPostion.Y);
                    }
                    else
                    {
                        Canvas.SetTop(shape_ellip, e.GetPosition(region).Y);
                    }
                    shape_ellip.Width = Math.Abs(d.X);
                    shape_ellip.Height = Math.Abs(d.Y);
                }              

                if (Drawmode == DrawMode.Line)
                {
                   
                    shape_line.X1 = MouseDownPostion.X;
                    shape_line.Y1 = MouseDownPostion.Y;
                    shape_line.X2 = e.GetPosition(border).X;
                    shape_line.Y2 = e.GetPosition(border).Y;
                }

                if (Drawmode == DrawMode.FreeDraw10)
                {   
                    Point p = e.GetPosition(border);
                    Line line = new Line();
                    line.Uid = "shape";
                    shape_free.Add(line);
                    line.Stroke = DrawColor;
                    line.StrokeThickness = 10;                  
                    region.Children.Add(line);
                    line.X1 = CurPos.X;
                    line.Y1 = CurPos.Y;
                    line.X2 = p.X;
                    line.Y2 = p.Y;
                    CurPos = p;
                }

                if (Drawmode == DrawMode.FreeDraw5)
                {
                    Point p = e.GetPosition(border);
                    Line line = new Line();
                    line.Uid = "shape";
                    shape_free.Add(line);
                    line.Stroke = DrawColor;
                    line.StrokeThickness = 5;
                    region.Children.Add(line);
                    line.X1 = CurPos.X;
                    line.Y1 = CurPos.Y;
                    line.X2 = p.X;
                    line.Y2 = p.Y;
                    CurPos = p;
                }

                if (Drawmode == DrawMode.FreeDraw2)
                {
                    Point p = e.GetPosition(border);
                    Line line = new Line();
                    line.Uid = "shape";
                    shape_free.Add(line);
                    line.Stroke = DrawColor;
                    line.StrokeThickness = 2;
                    region.Children.Add(line);
                    line.X1 = CurPos.X;
                    line.Y1 = CurPos.Y;
                    line.X2 = p.X;
                    line.Y2 = p.Y;
                    CurPos = p;
                }               
            }


            CurrentPostion = e.GetPosition(this);
            size.Margin = new Thickness(region.TranslatePoint(new Point(0, 0), this).X, region.TranslatePoint(new Point(0, 0), this).Y - 20, 0, 0);
            size.Text = "W: " + region.ActualWidth + "   H: " + border.ActualHeight;
            size.FontSize = 12;
            size.Foreground = Brushes.White;
            show_hide_move(sender, e);
            camerawiewer_move(sender, e);
        }



        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------

        private void rec_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_rec, 67.5 - 5);
            Canvas.SetTop(bt_rec, 32.5 - 5);
            bt_rec.Height = 45;
            bt_rec.Width = 45;
        }
        private void rec_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_rec, 67.5);
            Canvas.SetTop(bt_rec, 32.5);
            bt_rec.Height = bt_rec.Width = 35;
        }

        private void rec_down(object sender, MouseButtonEventArgs e)
        {
            if (!Recording)
            {                  
                bt_rec.Source = new BitmapImage(new Uri(@"/Resourses/_rec.png", UriKind.Relative));
                Start();
                HideToolbar();
            }        
        }

        private void stop_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetRight(bt_stop, 67.5 - 5);
            Canvas.SetTop(bt_stop, 32.5 - 5);
            bt_stop.Height = 45;
            bt_stop.Width = 45;
        }

        private void stop_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetRight(bt_stop, 67.5);
            Canvas.SetTop(bt_stop, 32.5);
            bt_stop.Height = bt_stop.Width = 35;
        }

        private void stop_down(object sender, MouseButtonEventArgs e)
        {

            if (isRecordWebcam)
            {
                bt_camera.Source = new BitmapImage(new Uri(@"/Resourses/_no_webcam.png", UriKind.Relative));
                RemoveCameraWiewer();
            }          

            if (Recording)
            {
                Stop();             
            }
            mainwindown.WindowState = WindowState.Normal;
            this.Close();
        }
     

        private void timer_enter(object sender, MouseEventArgs e)
        {
            timercontrol.Opacity = 1;
            Canvas.SetLeft(bt_timer, 120 - 5);
            Canvas.SetTop(bt_timer, 23 - 5);
            bt_timer.Height = 30;
            bt_timer.Width = 30;
        }
        private void timer_leave(object sender, MouseEventArgs e)
        {
            timercontrol.Opacity = 0;
            Canvas.SetLeft(bt_timer, 120);
            Canvas.SetTop(bt_timer, 23);
            bt_timer.Height = bt_timer.Width = 20;
        }

        private void timer_down(object sender, MouseButtonEventArgs e)
        {
            
            if (!Timing)
            {
                timercontrol.textbox.TextChanged += timer_TextChanged;                
                timercontrol.Opacity = 1;              
            }
        }

        private void timer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (timercontrol._time == TimeSpan.Zero && timercontrol.h.Text == "0" && timercontrol.m.Text == "0" && timercontrol.s.Text == "0")
            {
                Trace.WriteLine(timercontrol.h.Text + " " + timercontrol.m.Text + " " + timercontrol.s.Text);
                MessageBox.Show("END");
            }
        }

        private void capture_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_capture, 160 - 5);
            Canvas.SetTop(bt_capture, 23 - 5);
            bt_capture.Height = 30;
            bt_capture.Width = 30;
        }

        private void capture_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_capture, 160);
            Canvas.SetTop(bt_capture, 23);
            bt_capture.Height = bt_capture.Width = 20;
        }


        private void capture_down(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Bitmap captureBitmap = new System.Drawing.Bitmap((int)border.ActualWidth - 6, (int)border.ActualHeight - 6, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            System.Drawing.Graphics captureGraphics = System.Drawing.Graphics.FromImage(captureBitmap);


            captureGraphics.CopyFromScreen((int)border.PointToScreen(new Point(0, 0)).X + 3,
                                           (int)border.PointToScreen(new Point(0, 0)).Y + 3,
                                           0,
                                           0,
                                           new System.Drawing.Size((int)border.ActualWidth - 6, (int)border.ActualHeight - 6)
                                           );

            string path = capture_path + @"\Capture_" + Convert.ToDateTime(DateTime.Now).ToString("MMM_dd_yyyy_HH_mm_ss") + capture_type;
            if (capture_type.Contains("png"))
            {
                captureBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            if(capture_type.Contains("jpg"))
            {
                captureBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                captureBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void camera_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_camera, 200 - 5);
            Canvas.SetTop(bt_camera, 23 - 5);
            bt_camera.Height = 30;
            bt_camera.Width = 30;
        }
     
        private void camera_down(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("create");
            if (isRecordWebcam)
            {  
                bt_camera.Source = new BitmapImage(new Uri(@"/Resourses/_no_webcam.png", UriKind.Relative));              
                RemoveCameraWiewer();
            }
            else
            {
                bt_camera.Source = new BitmapImage(new Uri(@"/Resourses/_webcam.png", UriKind.Relative));              
                CreateCameraWiewer();
            }
        }

        private void camera_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_camera, 200);
            Canvas.SetTop(bt_camera, 23);
            bt_camera.Height = bt_camera.Width = 20;
        }

        private void micro_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_micro, 240 - 5);
            Canvas.SetTop(bt_micro, 23 - 5);
            bt_micro.Height = 30;
            bt_micro.Width = 30;
        }

        private void micro_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_micro, 240);
            Canvas.SetTop(bt_micro, 23);
            bt_micro.Height = bt_micro.Width = 20;
        }

        private void micro_down(object sender, MouseButtonEventArgs e)
        {
           
            if (isRecordMicrophone)
            {
                bt_micro.Source = new BitmapImage(new Uri(@"/Resourses/_no_micro.png", UriKind.Relative));
                isRecordMicrophone = false;
            }
            else
            {
                bt_micro.Source = new BitmapImage(new Uri(@"/Resourses/_micro.png", UriKind.Relative));
                isRecordMicrophone = true;
            }
        }

        private void audio_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_audio, 280 - 5);
            Canvas.SetTop(bt_audio, 23 - 5);
            bt_audio.Height = 30;
            bt_audio.Width = 30;
        }

        private void audio_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_audio, 280);
            Canvas.SetTop(bt_audio, 23);
            bt_audio.Height = bt_audio.Width = 20;
        }

        private void audio_down(object sender, MouseButtonEventArgs e)
        {
           
            if (isRecordAudio)
            {
                bt_audio.Source = new BitmapImage(new Uri(@"/Resourses/_no_audio.png", UriKind.Relative));
                isRecordAudio = false;
            }
            else
            {
                bt_audio.Source = new BitmapImage(new Uri(@"/Resourses/_audio.png", UriKind.Relative));
                isRecordAudio = true;
            }
        }

        private void shape_enter(object sender, MouseEventArgs e)
        {           
            Canvas.SetLeft(bt_shape, 120 - 5);
            Canvas.SetTop(bt_shape, 57 - 5);
            bt_shape.Height = 30;
            bt_shape.Width = 30;
        }

        private void shape_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_shape, 120);
            Canvas.SetTop(bt_shape, 57);
            bt_shape.Height = bt_shape.Width = 20;
        }

        private void pencil_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_pencil, 160 - 5);
            Canvas.SetTop(bt_pencil, 57 - 5);
            bt_pencil.Height = 30;
            bt_pencil.Width = 30;
        }

        private void pencil_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_pencil, 160);
            Canvas.SetTop(bt_pencil, 57);
            bt_pencil.Height = bt_pencil.Width = 20;
        }

        private void color_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_color, 200 - 5);
            Canvas.SetTop(bt_color, 53 - 5);
            bt_color.Height = 30;
            bt_color.Width = 30;
        }

        private void color_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_color, 200);
            Canvas.SetTop(bt_color, 53);
            bt_color.Height = bt_color.Width = 20;
        }

        private void text_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_text, 240 - 5);
            Canvas.SetTop(bt_text, 57 - 5);
            bt_text.Height = 29;
            bt_text.Width = 29;
        }

        private void text_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_text, 240);
            Canvas.SetTop(bt_text, 57);
            bt_text.Height = bt_text.Width = 19;
        }

        private void eraser_enter(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_eraser, 280 - 5);
            Canvas.SetTop(bt_eraser, 57 - 5);
            bt_eraser.Height = 30;
            bt_eraser.Width = 30;
        }

        private void eraser_leave(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(bt_eraser, 280);
            Canvas.SetTop(bt_eraser, 57);
            bt_eraser.Height = bt_eraser.Width = 20;

        }
        private void eraser_down(object sender, MouseButtonEventArgs e)
        {
            foreach(UIElement uie in region.Children)
            {
                if(uie.Uid == "shape")
                {
                    uie.Opacity = 0;
                }
            }
        }


        private void color_down(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DrawColor = _color.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
            }
        }

        private void show_hide_down(object sender, MouseButtonEventArgs e)
        {
            if (!toolbar_Removing)
            {
                toolbar_Removing = true;
                mousedownTime = DateTime.Now;
                mousedownPos = e.GetPosition(this);

                EventCanvas = new Canvas();
                EventCanvas.MouseDown += eventcanvas_down;
                EventCanvas.MouseUp += eventcanvas_up;
                EventCanvas.MouseMove += eventcanvas_move;
                EventCanvas.Background = Brushes.White;
                EventCanvas.Opacity = 0.01;
                fullregion.Children.Add(EventCanvas);
            }
        }

        private void show_hide_up(object sender, MouseButtonEventArgs e)
        {
            if (toolbar_Removing)
            {
                toolbar_Removing = false;              
                if ((e.GetPosition(this) - mousedownPos).Length <= 3)
                {   
                    if (Canvas.GetLeft(toolbar) < MAXWIDTH - 40)
                        Canvas.SetLeft(toolbar, MAXWIDTH - 40);
                    else
                        Canvas.SetLeft(toolbar, MAXWIDTH - 400);
                }
                else
                {
                    if(Canvas.GetTop(toolbar) < 10)
                    {
                        Canvas.SetTop(toolbar, 10);
                    }
                    if (Canvas.GetTop(toolbar) > MAXHEIGHT - 90)
                    {
                        Canvas.SetTop(toolbar, MAXHEIGHT - 90);
                    }
                }
                fullregion.Children.Remove(EventCanvas);
            }
            Cursor = Cursors.Arrow;
        }

 
        private void show_hide_move(object sender, MouseEventArgs e)
        {
            if (toolbar_Removing)
            {   
                Cursor = Cursors.SizeNS;
                Canvas.SetTop(toolbar, e.GetPosition(this).Y - 50);
            }
        }

        private void shape_rect_click(object sender, RoutedEventArgs e)
        {   
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.Rectangle;
            Add_EventCanvas();
        }

        private void shape_ellip_click(object sender, RoutedEventArgs e)
        {
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.Ellipse;
            Add_EventCanvas();
        }

        private void shape_line_click(object sender, RoutedEventArgs e)
        {
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.Line;
            Add_EventCanvas();
        }

        private void pencil10_click(object sender, RoutedEventArgs e)
        {
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.FreeDraw10;
            Add_EventCanvas();
        }
       
        private void pencil5_click(object sender, RoutedEventArgs e)
        {
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.FreeDraw5;
            Add_EventCanvas();
        }
        private void pencil2_click(object sender, RoutedEventArgs e)
        {
            HideToolbar();
            Cursor = Cursors.Pen;
            Drawmode = DrawMode.FreeDraw2;
            Add_EventCanvas();
        }
   



        private void camerawiewer_up(object sender, MouseButtonEventArgs e)
        {
            if (MovingCamerawiewer)
            {
                MovingCamerawiewer = false;
                Remove_EventCanvas();
            }
        }

        private void camerawiewer_down(object sender, MouseButtonEventArgs e)
        {
            if (!MovingCamerawiewer)
            {              
                MovingCamerawiewer = true;
                MouseDownPostion = e.GetPosition(this);
                Add_EventCanvas();
            }
        }

        private void camerawiewer_move(object sender, MouseEventArgs e)
        {    
            if (MovingCamerawiewer)
            {
                Vector d = e.GetPosition(this) - MouseDownPostion;
                Canvas.SetLeft(camerawiewer, MAXWIDTH - 350 + d.X);
                Canvas.SetTop(camerawiewer, MAXHEIGHT -350 + d.Y);
            }
        }

        private void camerawiewer_enter(object sender, MouseEventArgs e)
        {
            camerawiewer.Cursor = Cursors.ScrollAll;
        }
        private void camerawiewer_leave(object sender, MouseEventArgs e)
        {
            camerawiewer.Cursor = Cursors.Arrow;
        }


        private void _timer_Tick(object sender, EventArgs e)
        {           
            Timing = false;
            if (isRecordWebcam)
            {
                bt_camera.Source = new BitmapImage(new Uri(@"/Resourses/_no_webcam.png", UriKind.Relative));
                RemoveCameraWiewer();
            }

            if (Recording)
            {
                Stop();
            }
            mainwindown.WindowState = WindowState.Normal;
            this.Close();       
        }

        private void ShowToolbar()
        {
            Canvas.SetLeft(toolbar, MAXWIDTH - 400);

        }
        private void HideToolbar()
        {
            Canvas.SetLeft(toolbar, MAXWIDTH - 40);
        }

        private void Add_EventCanvas()
        {
            EventCanvas = new Canvas();
            EventCanvas.MouseDown += eventcanvas_down;
            EventCanvas.MouseUp += eventcanvas_up;
            EventCanvas.MouseMove += eventcanvas_move;
            EventCanvas.Background = Brushes.White;
            EventCanvas.Opacity = 0.01;
            fullregion.Children.Add(EventCanvas);
        }
        private void Remove_EventCanvas()
        {
           if(EventCanvas!= null)
                fullregion.Children.Remove(EventCanvas);
        }

        private void CreateCameraWiewer()
        {
            Trace.WriteLine("_Create");
            isRecordWebcam = true;
            camerawiewer = new Image();
            camerawiewer.MouseEnter += camerawiewer_enter;
            camerawiewer.MouseLeave += camerawiewer_leave;
            camerawiewer.MouseDown += camerawiewer_down;
            camerawiewer.MouseMove += camerawiewer_move;
            camerawiewer.MouseUp += camerawiewer_up;
            camerawiewer.Width = 300;
            camerawiewer.Height = 300; 
            camerawiewer.Source = new BitmapImage(new Uri(@"/Resourses/_pause.png", UriKind.Relative));              
            canvas_camera.Children.Add(camerawiewer);
            Canvas.SetLeft(camerawiewer, MAXWIDTH - 350);
            Canvas.SetTop(camerawiewer,MAXHEIGHT - 350);
            CAMERA_REC = new Recorder();
            CAMERA_REC.StartCaptureCapmera();
            CAMERA_REC.CameraSource.NewFrame += new Accord.Video.NewFrameEventHandler(NewFrameCamera); 
        }
    
        private void RemoveCameraWiewer()
        {   
            isRecordWebcam = false;
            try
            {
                CAMERA_REC.StopCaptureCapmera();
                CAMERA_REC.CameraSource.Stop();
                canvas_camera.Children.Remove(camerawiewer);
            }
            catch { }
        }

        private void NewFrameCamera(object sender, Accord.Video.NewFrameEventArgs e)
        {
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)e.Frame.Clone();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            camerawiewer.Dispatcher.Invoke(() => this.camerawiewer.Source = bi);
        }



        private void Start()
        {
            if (!Recording)
            {
                Recording = true;
                Pausing = false;
                bt_rec.Source = new BitmapImage(new Uri(@"/Resourses/_rec.png", UriKind.Relative));
                //set thuoc tinh
                int x = (int)border.PointToScreen(new Point(0, 0)).X + 3;
                int y = (int)border.PointToScreen(new Point(0, 0)).Y + 3;
                int w = (int)(border.ActualWidth - 6) % 2 == 0 ? (int)(border.ActualWidth - 6) : (int)(border.ActualWidth - 6) - 1;
                int h = (int)(border.ActualHeight - 6) % 2 == 0 ? (int)(border.ActualHeight - 6) : (int)(border.ActualHeight - 6) - 1;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x,y,w,h);
                REC = new Recorder(rect, false, false, video_framerate, video_bitrate, video_path, video_name + video_type);
                REC.Start();
            } 
        }

        private void Pause()
        {
            if (Recording)
            {
                Recording = false;
                Pausing = true;
                bt_rec.Source = new BitmapImage(new Uri(@"/Resourses/_pause.png", UriKind.Relative));
            }          
        }

        private void Resume()
        {
            if (!Recording)
            {
                Recording = true;
                Pausing = false;
                bt_rec.Source = new BitmapImage(new Uri(@"/Resourses/_rec.png", UriKind.Relative));
            }
        }



        private void Stop()
        {         
            RemoveCameraWiewer();
            Recording = false;
            Pausing = false;
            REC.Stop();
        }

    }
}
