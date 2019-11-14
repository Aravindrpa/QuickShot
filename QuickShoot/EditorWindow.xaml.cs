using QuickShoot.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public Ellipse circle { get; set; }
        public TextBox text { get; set; }
        //public Border br { get; set; }
        public DShapes shape { get; set; }
        public DColors color { get; set; }
        public string FileName { get; set; }

        //private Task<BitmapSource> img_BlurAsync { get; set; }
        private Task<System.Drawing.Bitmap> img_EditAsync { get; set; }
        private int MarkingCount { get; set; }
        private ConcurrentDictionary<int,IShapeDetails> MarkingsDictionary { get; set; }
        //private ConcurrentDictionary concurrentDictionary;
        //private Task<System.Drawing.Bitmap> imgTask { get; set; }
        //private Task<BitmapSource> copyTask { get; set; }




        public EditorWindow(int left, int top, int width, int height)
        {
            //img_BlurAsync = Glob.ScreenShot.Take();
            //img_EditAsync = Glob.ScreenShot.Take(left, top, height, width);
            img_EditAsync = Glob.BMP.Crop(left, top, width, height);
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

            MarkingCount = 0;
            MarkingsDictionary = new ConcurrentDictionary<int, IShapeDetails>();
        }

        //ENUMS
        public enum DShapes
        {
            Rectangle,
            Line,
            Text,
            Circle
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
            var convertTask = Glob.BMPCropped.ConvertToBitmapSource();
            //Glob.BMPCropped.Save("D:\\test.png", System.Drawing.Imaging.ImageFormat.Png);
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

            //img_Edit.Source = convertTask.Result;
            //var p = img_Edit.TranslatePoint(new Point(),grid_Blur);
            //MessageBox.Show(p.X + "-" + p.Y);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = convertTask.Result;
            System.Drawing.Bitmap re = null;
            if (Glob.BMPCropped.Height > Glob.ScreenHeightWithDPI - 500)
            {
                re = Glob.BMPCropped.Resize_Picture(0, Glob.ScreenHeightWithDPI - 500).Result;
            }
            else if (Glob.BMPCropped.Height > Glob.ScreenWidthWithDPI - 500)
            {
                re = Glob.BMPCropped.Resize_Picture(Glob.ScreenWidthWithDPI - 500, 0).Result;
            }
            canv_Img.Height = re.Height;
            canv_Img.Width = re.Width;
            canv_Img.Background = ib;

            if (Glob.Config.EnableOnlyClipboard)
                SaveAndClose(); //close it here itself, no need for editor 
        }
        //private void editor_window_Closed(object sender, EventArgs e)
        //{
        //    if (isCopy)
        //        Clipboard.SetImage(copyTask.Result);           
        //    else if(isSave)
        //        imgTask.Result.Save(Glob.folderManager.GetCurrentPath() + "\\" + this.FileName, System.Drawing.Imaging.ImageFormat.Png);

        //}

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

            //this.FileName = fileName;
            if (Glob.Config.EnableOnlyClipboard)
            {
                var imgTask = Glob.BMPCropped.ConvertToBitmapSource();
                this.Close();
                Clipboard.SetImage(imgTask.Result);
            }
            else
            {
                var imgTask = Glob.MergeAllBitmaps(Glob.BMPCropped, canv_Img.ExportCanvasImage().Result);
                //isSave = true;
                this.Close();
                imgTask.Result.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Png);
            }

            Glob.ScreenShot.test(canv_Img, MarkingsDictionary,fileName);

            //var p = img_Edit.PointToScreen(new Point());
            //int wid = 0;
            //int hei = 0;
            //ScreenShot.TransformToPixels(img_Edit.ActualWidth, img_Edit.ActualHeight, out wid, out hei);
            //var bmp = Glob.ScreenShot.CopyFromBounds(
            //    (int)p.X,
            //    (int)p.Y,
            //    wid, hei);
            //var task = Glob.ScreenShot.ConvertBmpToSource(bmp.Result);
            //bmp.Result.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Png);
            //Clipboard.SetImage(task.Result);



            ////WORKING
            ////var g = img_Edit.PointToScreen(new Point());
            ////int gwid = 0;
            ////int ghei = 0;
            ////ScreenShot.TransformToPixels(img_Edit.ActualWidth, img_Edit.ActualHeight, out gwid, out ghei);


            //canvasShapeImage.Save(Glob.folderManager.GetCurrentPath() + "\\shp1-" + fileName, System.Drawing.Imaging.ImageFormat.Png);

            ////this is the rendered image from canvas
            ////But canvas size is different from image captured from bounds of image control above
            ////so this needs to be corrected somehow to merge with actual screen capture

            ////int px = 0;int py = 0;
            ////ScreenShot.TransformToPixels(p.X, p.Y, out px, out py);
            //var correction = (3.59 / 100) * Glob.WidthWithDPI;//Glob.BMPCropped.Width; //3.59 -- was trial and error -- actually 69 width was working for FHD resolution - converted to %
            //var correction1 = (3.59 / 100) * Glob.HeightWithDPI;
            //var rounded = (int)(Math.Round(correction, 0, MidpointRounding.AwayFromZero));
            //var rounded1 = (int)(Math.Round(correction1, 0, MidpointRounding.AwayFromZero));
            //var croppedShpeImage = Glob.ScreenShot.Crop(canvasShapeImage
            //    , 0, 0,
            //   (int)img_Edit.ActualWidth, (int)img_Edit.ActualHeight);
            //var b = croppedShpeImage.Result;
            //b.Save(Glob.folderManager.GetCurrentPath() + "\\shp-" + fileName, System.Drawing.Imaging.ImageFormat.Png);


        }

        public void CopyImage()
        {
            //var p = img_Edit.PointToScreen(new Point());
            //int wid = 0;
            //int hei = 0;
            //ScreenShot.TransformToPixels(img_Edit.ActualWidth, img_Edit.ActualHeight, out wid, out hei);
            //var bmp = Glob.ScreenShot.CopyFromBounds(
            //    (int)p.X,
            //    (int)p.Y,
            //    wid, hei);
            //var task = Glob.ScreenShot.ConvertBmpToSource(bmp.Result);

            //async result is not helping here, no async calls -- maybe a serial call would be faster??
            var firstTask = Glob.MergeAllBitmaps(Glob.BMPCropped, canv_Img.ExportCanvasImage().Result);
            var copyTask = firstTask.Result.ConvertToBitmapSource();
            //isCopy = true;
            this.Close();
            Clipboard.SetImage(copyTask.Result);//after calling close - bit faster putting here
        }
        private void ToggleButton(DShapes shape)
        {
            switch (shape)
            {
                case (DShapes.Rectangle):
                    br_T.BorderBrush = Brushes.Black;
                    br_LN.BorderBrush = Brushes.Black;
                    br_C.BorderBrush = Brushes.Black;
                    br_SQ.BorderBrush = Brushes.Green;
                    lbl_T.Background = Brushes.Black;
                    lbl_C.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Black;
                    lbl_SQ.Background = Brushes.Green;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_T);
                        Glob.Animate.Breath(br_LN);
                        Glob.Animate.Breath(br_C);
                        Glob.Animate.StopBreath(br_SQ);
                    }
                    break;
                case (DShapes.Line):
                    br_T.BorderBrush = Brushes.Black;
                    br_LN.BorderBrush = Brushes.Green;
                    br_SQ.BorderBrush = Brushes.Black;
                    br_C.BorderBrush = Brushes.Black;
                    lbl_T.Background = Brushes.Black;
                    lbl_C.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Green;
                    lbl_SQ.Background = Brushes.Black;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_T);
                        Glob.Animate.Breath(br_SQ);
                        Glob.Animate.Breath(br_C);
                        Glob.Animate.StopBreath(br_LN);
                    }
                    break;
                case (DShapes.Text):
                    br_T.BorderBrush = Brushes.Green;
                    br_LN.BorderBrush = Brushes.Black;
                    br_SQ.BorderBrush = Brushes.Black;
                    br_C.BorderBrush = Brushes.Black;
                    lbl_T.Background = Brushes.Green;
                    lbl_C.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Black;
                    lbl_SQ.Background = Brushes.Black;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_SQ);
                        Glob.Animate.Breath(br_LN);
                        Glob.Animate.Breath(br_C);
                        Glob.Animate.StopBreath(br_T);
                    }
                    break;
                case (DShapes.Circle):
                    br_T.BorderBrush = Brushes.Black;
                    br_LN.BorderBrush = Brushes.Black;
                    br_SQ.BorderBrush = Brushes.Black;
                    br_C.BorderBrush = Brushes.Green;
                    lbl_T.Background = Brushes.Black;
                    lbl_LN.Background = Brushes.Black;
                    lbl_SQ.Background = Brushes.Black;
                    lbl_C.Background = Brushes.Green;
                    if (Glob.Config.EnableAnimations)
                    {
                        Glob.Animate.Breath(br_SQ);
                        Glob.Animate.Breath(br_LN);
                        Glob.Animate.Breath(br_T);
                        Glob.Animate.StopBreath(br_C);
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
        
        private void SaveShape<T>(int key,ShapeDetails<T> shape) where T: class
        {
            MarkingsDictionary.TryAdd(key, shape);
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
            //ABove not

            var effect = new DropShadowEffect();
            effect.Direction = 320;
            effect.BlurRadius = 5.5;
            effect.ShadowDepth = 4.5;

            startPoint = Mouse.GetPosition(canv_Img);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                switch (shape)
                {
                    case (DShapes.Rectangle):
                        MarkingCount++;
                        //br = new Border();
                        rect = new Rectangle();
                        rect.Stroke = Glob.Config.SelectedBrush;
                        rect.StrokeThickness = 2.2;                       
                        //br.BorderThickness = new Thickness(2);
                        //br.BorderBrush = Brushes.Transparent;
                        rect.Effect = effect;
                        Canvas.SetLeft(rect, startPoint.X);
                        Canvas.SetTop(rect, startPoint.Y);
                        //br.Child = rect;
                        canv_Img.Children.Add(rect);
                        break;
                    case (DShapes.Line):
                        MarkingCount++;
                        line = new Line();
                        line.Stroke = Glob.Config.SelectedBrush;
                        line.StrokeThickness = 2.2;
                        line.X1 = startPoint.X;
                        line.Y1 = startPoint.Y;
                        line.Effect = effect;
                        canv_Img.Children.Add(line);
                        break;
                    case (DShapes.Text):
                        MarkingCount++;
                        text = new TextBox();
                        text.Foreground = Glob.Config.SelectedBrush;
                        text.Background = Brushes.Transparent;
                        text.BorderBrush = Glob.Config.SelectedBrush;
                        text.BorderThickness = new Thickness(2, 0, 0, 2);
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
                        text.LostFocus += Text_LostFocus; //do the text add to MarkingsDictionary in lost focus
                        text.TextChanged += Text_TextChanged;
                        break;
                    case (DShapes.Circle):
                        MarkingCount++;
                        circle = new Ellipse();
                        circle.Stroke = Glob.Config.SelectedBrush;
                        circle.StrokeThickness = 2.2;
                        circle.Effect = effect;
                        Canvas.SetLeft(circle, startPoint.X);
                        Canvas.SetTop(circle, startPoint.Y);
                        canv_Img.Children.Add(circle);
                        break;

                }
            }
        }
        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.BorderThickness = new Thickness(0, 0, 0, 0);
        }
        private void Text_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;           
            tb.BorderThickness = new Thickness(0, 0, 0, 0);
            var p = canv_Img.TranslatePoint(new Point(), grid_Blur);//TODO : move to top
            Task.Run(() => SaveShape<TextBox>(MarkingCount, new ShapeDetails<TextBox>(tb,p.X,p.Y, canv_Img.ActualWidth, canv_Img.ActualHeight, grid_Blur)));
        }
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
                                Canvas.SetLeft(rect, pointNow.X);
                            }
                            else
                                wid = pointNow.X - startPoint.X;

                            if (pointNow.Y < startPoint.Y)
                            {
                                hei = startPoint.Y - pointNow.Y;
                                Canvas.SetTop(rect, pointNow.Y);
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
                        case (DShapes.Circle):
                            if (pointNow.X < startPoint.X)
                            {
                                wid = startPoint.X - pointNow.X;
                                Canvas.SetLeft(circle, pointNow.X);
                            }
                            else
                                wid = pointNow.X - startPoint.X;

                            if (pointNow.Y < startPoint.Y)
                            {
                                hei = startPoint.Y - pointNow.Y;
                                Canvas.SetTop(circle, pointNow.Y);
                            }
                            else
                                hei = pointNow.Y - startPoint.Y;

                            circle.Width = wid;
                            circle.Height = hei;
                            break;
                    }
                }

            }
            catch
            { }
        }
        private async void canv_Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //TODO: any mouse up outside canvas bounds will not work - will not save - put it under background mouseup

            var p = canv_Img.TranslatePoint(new Point(), grid_Blur);//TODO: move to top
            switch (shape)
            {
                case (DShapes.Rectangle):
                    //await Task.Run(() => SaveShape<Rectangle>(MarkingCount, new ShapeDetails<Rectangle>(rect,p.X,p.Y, grid_Blur)));
                    SaveShape<Rectangle>(MarkingCount, new ShapeDetails<Rectangle>(rect, p.X, p.Y,canv_Img.ActualWidth,canv_Img.ActualHeight, grid_Blur));
                    break;
                case (DShapes.Line):
                    SaveShape<Line>(MarkingCount, new ShapeDetails<Line>(line,p.X, p.Y, canv_Img.ActualWidth, canv_Img.ActualHeight, grid_Blur));
                    break;
                case (DShapes.Circle):
                    SaveShape<Ellipse>(MarkingCount, new ShapeDetails<Ellipse>(circle, p.X, p.Y, canv_Img.ActualWidth, canv_Img.ActualHeight, grid_Blur));
                    break;

            }
            //var p = canv_Img.TranslatePoint(new Point(), grid_Blur);
            //MessageBox.Show(p.X + "-" + p.Y);
            //var p1 = rect.TranslatePoint(new Point(), grid_Blur);
            //MessageBox.Show(p1.X + "-" + p1.Y);
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
        private void lbl_C_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetShape(DShapes.Circle);
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

    public interface IShapeDetails
    {
        //Marker interface //this is the simplest solution -- //not needed to be mrker interface
        dynamic StoredShape { get; set; }
        Type StoredShapeType { get; set; }
        Point StartPoint { get; set; }
        double parentx { get; set; }
        double parenty { get; set; }
        double parentw { get; set; }
        double parenth { get; set; }
        double height { get; set; }
        double width { get; set; }
        Point EndPoint { get; set; }
    }
    public class ShapeDetails<T>  : IShapeDetails
    {
        public dynamic StoredShape { get; set; }
        public Type StoredShapeType { get; set; }
        public Point StartPoint { get; set; }
        public double parentx { get; set; }
        public double parenty { get; set; }
        public double parentw { get; set; }
        public double parenth { get; set; }
        public Point EndPoint { get; set; }
        public double height { get; set; }
        public double width { get; set; }

        public ShapeDetails(T shape,double parentX,double parentY, double parentW, double parentH, UIElement refObject) 
        {
            StoredShape = shape;
            parentx = parentX;
            parenty = parentY;
            parentw = parentW;
            parenth = parentH;
            StoredShapeType = shape.GetType();
            MethodInfo t = StoredShapeType.GetMethod("TranslatePoint", new[] { typeof(Point), typeof(UIElement) });
            StartPoint = (Point)t.Invoke(shape, new object[] { new Point(), refObject});
            width = (double)shape.GetType().GetProperty("Width").GetValue(shape);
            height = (double)shape.GetType().GetProperty("Height").GetValue(shape);
            EndPoint = new Point(StartPoint.X + width, StartPoint.Y + height);            
            //MethodInfo t = shapeType.GetMethod("TranslatePoint", new [] {typeof(Point), typeof(UIElement) });
            //StartPoint = (Point)t.Invoke(shape, new object[] { new Point(), refObject});
            //EndPoint = new Point(StartPoint.X+);
            //MessageBox.Show(r + "str");
        }

    }
}
