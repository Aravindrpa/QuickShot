using QuickShoot.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuickShoot
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public Point startPoint { get; set; }
        public Rectangle rect { get; set; }
        public Line line { get; set; }
        public TextBox text { get; set; }
        public Border br { get; set; }
        public DShapes shape { get; set; }
        public DColors color { get; set; }
        public string FileName { get; set; }

        //private Task<BitmapSource> img_BlurAsync { get; set; }
        private Task<System.Drawing.Bitmap> img_EditAsync { get; set; }

        public EditorWindow(int left, int top, int width, int height)
        {
            //img_BlurAsync = Glob.ScreenShot.Take();
            //img_EditAsync = Glob.ScreenShot.Take(left, top, height, width);
            img_EditAsync = Glob.ScreenShot.Crop(Glob.BMP, left, top, width, height);
            InitializeComponent();

            if (Glob.Config.EnableBlurEffect)
            {
                grid_Blur.Background = Brushes.Black;
                img_Blur.Opacity = Glob.Config.EditorBackgroundOpacity;
                blurEffect.Radius = Glob.Config.BlurRadious;
            }
            else
            {
                grid_Blur.Background = Brushes.Gray;
                img_Blur.Opacity = 0;
                blurEffect.Radius = 0;
            }
        }

        //ENUMS
        public enum DShapes
        {
            Rectangle,
            Line,
            Text
        }
        public enum DColors
        {
            Red,
            Green,
            Orange,
            Yellow
        }

        //Window load
        private void editor_window_Loaded(object sender, RoutedEventArgs e)
        {
            //img_Edit.Width = canv_Img.ActualWidth - 80;
            //img_Edit.Height = canv_Img.ActualHeight - 80;
            Glob.BMPCropped = img_EditAsync.Result;
            var convertTask = Glob.ScreenShot.ConvertBmpToSource(Glob.BMPCropped);
            SetShape(DShapes.Rectangle);
            SetColor(DColors.Green);
            //Glob.Animate.Breath(lbl_FileName);
            //Glob.Animate.Breath(br_Close);
            this.FileName = DateTime.Now.ToString("yyyyMMddHHmmssff");
            textb_FileName.Text = this.FileName;
            textb_FileName.SelectAll();
            Keyboard.Focus(textb_FileName);

            //img_Blur.Source = img_BlurAsync.Result;
            if (Glob.Config.EnableBlurEffect)
                img_Blur.Source = Glob.Background; //Glob.Background;

            img_Edit.Source = convertTask.Result;
            //ImageBrush ib = new ImageBrush();
            //ib.ImageSource = convertTask.Result;
            //canv_Img.Background = ib;
        }

        //Helper methods
        private void SetShape(DShapes shap)
        {
            this.shape = shap;
            ToggleButton(this.shape);
        }
        private void SetColor(DColors col)
        {
            this.color = col;
            ToggleButton2(this.color);
        }
        public void SaveAndClose()
        {
            Glob.folderManager.DTimer.Start();

            var fileName = "test.png";
            if (!string.IsNullOrEmpty(textb_FileName.Text))
                if (!textb_FileName.Text.Contains(this.FileName))
                    fileName = this.FileName + "-" + textb_FileName.Text + ".png";
                else
                    fileName = textb_FileName.Text + ".png";

            var p = img_Edit.PointToScreen(new Point());
            int wid = 0;
            int hei = 0;
            ScreenShot.TransformToPixels(img_Edit.ActualWidth, img_Edit.ActualHeight, out wid, out hei);
            var bmp = Glob.ScreenShot.CopyFromBounds(
                (int)p.X,
                (int)p.Y,
                wid, hei);
            //var task = Glob.ScreenShot.ConvertBmpToSource(bmp.Result);
            bmp.Result.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName);
            //Clipboard.SetImage(task.Result);
            //new ScreenShot().TakeAndSave(
            //    (int)p.X,
            //    (int)p.Y,
            //    wid, hei,
            //    Glob.folderManager.GetCurrentPath() + "\\" + fileName
            //    );

            //WORKING
            //System.Drawing.Bitmap ff = CanvasImage(canv_Img);
            //var bmp1 = Glob.ScreenShot.MergeAllBitmaps(Glob.BMPCropped, ff).Result;
            //bmp1.Save(Glob.folderManager.GetCurrentPath() + "\\can-" + fileName);

            this.Close();
        }
        public void CopyImage()
        {
            var p = img_Edit.PointToScreen(new Point());
            int wid = 0;
            int hei = 0;
            ScreenShot.TransformToPixels(img_Edit.ActualWidth, img_Edit.ActualHeight, out wid, out hei);
            var bmp = Glob.ScreenShot.CopyFromBounds(
                (int)p.X,
                (int)p.Y,
                wid, hei);
            var task = Glob.ScreenShot.ConvertBmpToSource(bmp.Result);
            Clipboard.SetImage(task.Result);
            this.Close();
        }
        private void ToggleButton(DShapes shape)
        {
            switch (shape)
            {
                case (DShapes.Rectangle):
                    br_T.BorderBrush = Brushes.Black;
                    br_LN.BorderBrush = Brushes.Black;
                    br_SQ.BorderBrush = Brushes.Green;
                    lbl_T.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Black;
                    lbl_SQ.Background = Brushes.Green;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_T);
                        Glob.Animate.Breath(br_LN);
                        Glob.Animate.StopBreath(br_SQ);
                    }
                    break;
                case (DShapes.Line):
                    br_T.BorderBrush = Brushes.Black;
                    br_LN.BorderBrush = Brushes.Green;
                    br_SQ.BorderBrush = Brushes.Black;
                    lbl_T.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Green;
                    lbl_SQ.Background = Brushes.Black;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_T);
                        Glob.Animate.Breath(br_SQ);
                        Glob.Animate.StopBreath(br_LN);
                    }
                    break;
                case (DShapes.Text):
                    br_T.BorderBrush = Brushes.Green;
                    br_LN.BorderBrush = Brushes.Black;
                    br_SQ.BorderBrush = Brushes.Black;
                    lbl_T.Background = Brushes.Green;
                    lbl_LN.Background = Brushes.Black;
                    lbl_SQ.Background = Brushes.Black;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_SQ);
                        Glob.Animate.Breath(br_LN);
                        Glob.Animate.StopBreath(br_T);
                    }
                    break;
            }
        }
        private void ToggleButton2(DColors color)
        {
            switch (color)
            {
                case (DColors.Red):
                    br_Red.BorderBrush = Brushes.Black;
                    br_Green.BorderBrush = Brushes.Transparent;
                    br_Oran.BorderBrush = Brushes.Transparent;
                    br_Yellow.BorderBrush = Brushes.Transparent;
                    Glob.Config.SelectedBrush = Brushes.Red;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.StopBreath(br_Red);
                        Glob.Animate.Breath(br_Green);
                        Glob.Animate.Breath(br_Oran);
                        Glob.Animate.Breath(br_Yellow);
                    }
                    break;
                case (DColors.Green):
                    br_Red.BorderBrush = Brushes.Transparent;
                    br_Green.BorderBrush = Brushes.Black;
                    br_Oran.BorderBrush = Brushes.Transparent;
                    br_Yellow.BorderBrush = Brushes.Transparent;
                    Glob.Config.SelectedBrush = Brushes.LimeGreen;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_Red);
                        Glob.Animate.StopBreath(br_Green);
                        Glob.Animate.Breath(br_Oran);
                        Glob.Animate.Breath(br_Yellow);
                    }
                    break;
                case (DColors.Orange):
                    br_Red.BorderBrush = Brushes.Transparent;
                    br_Green.BorderBrush = Brushes.Transparent;
                    br_Oran.BorderBrush = Brushes.Black;
                    br_Yellow.BorderBrush = Brushes.Transparent;
                    Glob.Config.SelectedBrush = Brushes.Orange;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_Red);
                        Glob.Animate.Breath(br_Green);
                        Glob.Animate.StopBreath(br_Oran);
                        Glob.Animate.Breath(br_Yellow);
                    }
                    break;
                case (DColors.Yellow):
                    br_Red.BorderBrush = Brushes.Transparent;
                    br_Green.BorderBrush = Brushes.Transparent;
                    br_Oran.BorderBrush = Brushes.Transparent;
                    br_Yellow.BorderBrush = Brushes.Black;
                    Glob.Config.SelectedBrush = Brushes.Yellow;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_Red);
                        Glob.Animate.Breath(br_Green);
                        Glob.Animate.Breath(br_Oran);
                        Glob.Animate.StopBreath(br_Yellow);
                    }
                    break;
            }
        }
        private System.Drawing.Bitmap CanvasImage(Canvas canvas)
        {
            int wid = 0; int hei = 0;
            //ScreenShot.TransformToPixels(canvas.ActualWidth, canvas.ActualHeight, out wid, out hei);
            wid = (int)canvas.ActualWidth;
            hei = (int)canvas.ActualHeight;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             wid, hei,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size(wid, hei));
            canvas.Arrange(new Rect(new Size(wid, hei)));

            //renderBitmap.Render(img);
            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
            return new System.Drawing.Bitmap(stream);
            //using (FileStream file = File.Create(Glob.folderManager.GetCurrentPath() + "\\can-" + fileName))
            //{
            //    encoder.Save(file);
            //}
        }


        //Control Event Handlers
        private void lbl_Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void canv_Img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //foreach (var child in canv_Img.Children)
            //{
            //    if (child.GetType() == typeof(TextBox))
            //    {
            //        var tb = (TextBox)child;
            //        tb.BorderThickness = new Thickness(0, 0, 0, 0);
            //    }
            //}
            var effect = new DropShadowEffect();
            effect.Direction = 320;
            effect.BlurRadius = 5;
            effect.ShadowDepth = 4;

            startPoint = Mouse.GetPosition(canv_Img);
            if (e.ButtonState == MouseButtonState.Pressed)
            {

                switch (shape)
                {
                    case (DShapes.Rectangle):
                        br = new Border();
                        Canvas.SetLeft(br, startPoint.X);
                        Canvas.SetTop(br, startPoint.Y);
                        rect = new Rectangle();
                        rect.Stroke = Glob.Config.SelectedBrush;
                        rect.StrokeThickness = 2.2;
                        br.BorderThickness = new Thickness(2);
                        br.BorderBrush = Brushes.Transparent;
                        br.Effect = effect;
                        br.Child = rect;
                        canv_Img.Children.Add(br);
                        break;
                    case (DShapes.Line):
                        line = new Line();
                        line.Stroke = Glob.Config.SelectedBrush;
                        line.StrokeThickness = 2;
                        line.X1 = startPoint.X;
                        line.Y1 = startPoint.Y;
                        line.Effect = effect;
                        canv_Img.Children.Add(line);
                        break;
                    case (DShapes.Text):
                        text = new TextBox();
                        text.Foreground = Glob.Config.SelectedBrush;
                        text.Background = Brushes.Transparent;
                        text.BorderBrush = Glob.Config.SelectedBrush;
                        text.BorderThickness = new Thickness(2,0,0,2);
                        effect.ShadowDepth = 1;
                        effect.BlurRadius = 3;
                        text.Effect = effect;
                        //label.Padding = new Thickness(5,5,5,5);
                        //label.BorderThickness = new Thickness(0);
                        text.FontSize = 18;
                        Canvas.SetLeft(text, startPoint.X);
                        Canvas.SetTop(text, startPoint.Y);
                        canv_Img.Children.Add(text);
                        //text.SelectAll();
                        Keyboard.Focus(text);
                        //text.LostFocus += Text_LostFocus;
                        text.TextChanged += Text_TextChanged;
                        break;

                }
            }
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        //private void Text_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox tb = sender as TextBox;
        //    tb.BorderThickness = new Thickness(0, 0, 0, 0);
        //}

        private void canv_Img_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                double wid = 0;
                double hei = 0;
                var pointNow = Mouse.GetPosition(canv_Img); // new Point(X, Y);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    switch (shape)
                    {
                        case (DShapes.Rectangle):
                            if (pointNow.X < startPoint.X)
                            {
                                wid = startPoint.X - pointNow.X;
                                Canvas.SetLeft(br, pointNow.X);
                            }
                            else
                                wid = pointNow.X - startPoint.X;

                            if (pointNow.Y < startPoint.Y)
                            {
                                hei = startPoint.Y - pointNow.Y;
                                Canvas.SetTop(br, pointNow.Y);
                            }
                            else
                                hei = pointNow.Y - startPoint.Y;

                            rect.Width = wid;
                            rect.Height = hei;
                            break;

                        case (DShapes.Line):
                            line.X2 = pointNow.X;
                            line.Y2 = pointNow.Y;
                            break;
                    }
                }

            }
            catch
            { }
        }
        private void canv_Img_MouseUp(object sender, MouseButtonEventArgs e)
        {


        }
        private void lbl_Save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveAndClose();
        }
        private void lbl_SQ_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetShape(DShapes.Rectangle);
        }
        private void lbl_LN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetShape(DShapes.Line);
        }
        private void lbl_T_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetShape(DShapes.Text);
        }
        private void lbl_Red_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetColor(DColors.Red);
        }
        private void lbl_Green_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetColor(DColors.Green);
        }
        private void lbl_Oran_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetColor(DColors.Orange);
        }
        private void lbl_Yellow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetColor(DColors.Yellow);
        }
        private void lbl_Copy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CopyImage();
        }
    }
}
