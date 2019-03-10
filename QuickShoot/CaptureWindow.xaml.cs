using QuickShoot.Helpers;
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

namespace QuickShoot
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        double screenWidth = SystemParameters.VirtualScreenWidth;
        double screenHeight = SystemParameters.VirtualScreenHeight;

        int screenWidthdpi = 0;
        int screenHeightdpi = 0;

        Point startPoint;
        Rectangle rect;
        Border br;

        Task<System.Drawing.Bitmap> task;

        int brThickness = 0;

        public CaptureWindow()
        {
            task = Glob.ScreenShot.TakeBitmap();
            InitializeComponent();
        }

        private void capture_window_Loaded(object sender, RoutedEventArgs e)
        {
            startPoint = new Point();
            br = new Border();
            rect = new Rectangle();
            Glob.BMP = task.Result;
            var anotherTask = Glob.ScreenShot.ConvertBmpToSource(Glob.BMP);

            ScreenShot.TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);
            Glob.WidthWithDPI = screenWidthdpi;
            Glob.HeightWithDPI = screenHeightdpi;
            brThickness = screenWidthdpi * 2; //to make sure the border has enough thickness to overflow the screen area while capturing

            br = new Border();
            br.BorderThickness = new Thickness(brThickness);
            br.BorderBrush = Brushes.White;
            br.Opacity = .7;

            rect = new Rectangle();
            rect.Stroke = Brushes.LightBlue;
            rect.StrokeThickness = 3;
            Canvas.SetLeft(br, 0);
            Canvas.SetTop(br, 0);
            rect.Width = 0;
            rect.Height = 0;

            br.Child = rect;
            canvas_Draw.Children.Add(br);
            Glob.Background = anotherTask.Result;
            img_Back.Source = Glob.Background;
        }

        //Control Event Handlers
        private void canvas_Draw_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                startPoint = Mouse.GetPosition(canvas_Draw); //e.GetPosition(this);
                Canvas.SetLeft(br, startPoint.X - brThickness);
                Canvas.SetTop(br, startPoint.Y - brThickness);
            }
        }
        private void canvas_Draw_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double wid = 0;
                double hei = 0;
                var pointNow = Mouse.GetPosition(canvas_Draw);//e.GetPosition(this);
                if (pointNow.X < startPoint.X)
                {
                    wid = startPoint.X - pointNow.X;
                    Canvas.SetLeft(br, pointNow.X - brThickness);
                }
                else
                    wid = pointNow.X - startPoint.X;

                if (pointNow.Y < startPoint.Y)
                {
                    hei = startPoint.Y - pointNow.Y;
                    Canvas.SetTop(br, pointNow.Y - brThickness);
                }
                else
                    hei = pointNow.Y - startPoint.Y;

                rect.Width = wid;
                rect.Height = hei;
            }
        }
        public void TakeWholeScreen()
        {
            int l = 0;
            int t = 0;
            int wid = screenWidthdpi;
            int hei = screenHeightdpi;
            this.Hide();

            if (Glob.Config.EnableLiteMode)
            {
                Glob.editorWindowLite = new EditorWindowLite(l, t, wid, hei);
                Glob.editorWindowLite.Show();
            }
            else
            {
                Glob.editorWindow = new EditorWindow(l, t, wid, hei);
                Glob.editorWindow.Show();
            }

            this.Close();
        }
        private void canvas_Draw_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int l = 0;
            int t = 0;
            int l1 = 0;
            int t1 = 0;
            int wid = 0;
            int hei = 0;

            //CORRECT DPI again 
            ScreenShot.TransformToPixels(
                //Canvas.GetLeft(br) + brThickness-3,
                //Canvas.GetTop(br) + brThickness-3
                startPoint.X - 3,
                startPoint.Y - 3
                , out l, out t);

            var p = Mouse.GetPosition(canvas_Draw);
            ScreenShot.TransformToPixels(
                p.X - 3,
                p.Y - 3
                , out l1, out t1);

            wid = l1 - l;//Including the stroke thickness//(int)rect.Width;
            hei = t1 - t;//(int)rect.Height;
            if (wid <= 0 || hei <= 0) //take the whole screen if it was just a click
            {
                
                if (wid < -10 || hei < -10) // if it was a reverse draw
                {
                    wid = l - l1;
                    hei = t - t1;
                    l = l1;
                    t = t1; //now that rectangle is inverted, make start point as endponit
                }
                else
                {
                    l = 0; t = 0;
                    wid = screenWidthdpi;
                    hei = screenHeightdpi;
                }
            }

            rect.Width = 0;
            rect.Height = 0;

            this.Hide();

            if (Glob.Config.EnableLiteMode)
            {
                Glob.editorWindowLite = new EditorWindowLite(l, t, wid, hei);
                Glob.editorWindowLite.Show();
            }
            else
            {
                Glob.editorWindow = new EditorWindow(l, t, wid, hei);
                Glob.editorWindow.Show();
            }

            this.Close();

        }


    }
}
