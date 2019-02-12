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

        int brThickness = 0;

        public CaptureWindow()
        {
            startPoint = new Point();
            br = new Border();
            rect = new Rectangle();

            ScreenShot.TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);
            brThickness = screenWidthdpi * 2; //to make sure the border has enough thickness to overflow the screen area while capturing

            InitializeComponent();

            img_Back.Source = new ScreenShot().Take();

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
        }

        private void canvas_Draw_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(this);
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
                if (e.GetPosition(this).X < startPoint.X)
                {
                    wid = startPoint.X - e.GetPosition(this).X;
                    Canvas.SetLeft(br, e.GetPosition(this).X - brThickness);
                }
                else
                    wid = e.GetPosition(this).X - startPoint.X;

                if (e.GetPosition(this).Y < startPoint.Y)
                {
                    hei = startPoint.Y - e.GetPosition(this).Y;
                    Canvas.SetTop(br, e.GetPosition(this).Y - brThickness);
                }
                else
                    hei = e.GetPosition(this).Y - startPoint.Y;

                rect.Width = wid;
                rect.Height = hei;
            }
        }

        private void canvas_Draw_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int l = 0;
            int t = 0;
            int wid = 0;
            int hei = 0;

            //CORRECT DPI again 

            ScreenShot.TransformToPixels(
                Canvas.GetLeft(br) + brThickness,
                Canvas.GetTop(br) + brThickness
                , out l, out t);

            wid = (int)rect.Width;
            hei = (int)rect.Height;

            if (wid <= 0 || hei <= 0) //take the whole screen if it was just a click
            {
                l = 0; t = 0;
                wid = screenWidthdpi;
                hei = screenHeightdpi;
            }

            rect.Width = 0;
            rect.Height = 0;

            this.Hide();

            EditorWindow win = new EditorWindow(l,t,wid,hei);
            win.Show();

        }
    }
}
