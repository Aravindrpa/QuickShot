using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace QuickShoot.Helpers
{
    public class ScreenShot
    {
        public ScreenShot()
        {
        }
        public async Task<BitmapSource> Take()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            int screenWidthdpi = 0;
            int screenHeightdpi = 0;
            Glob.TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);

            var bmp = await CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
            return await bmp.ConvertToBitmapSource();
        }
        public async Task<BitmapSource> Take(int left, int top, int width, int height)
        {
            var bmp = await CopyFromBounds(left, top, width, height);
            return await bmp.ConvertToBitmapSource();
        }
        public async Task<Bitmap> TakeBitmap()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            int screenWidthdpi = 0;
            int screenHeightdpi = 0;
            Glob.TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);

            return await CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
        }
        public async Task<Bitmap> CopyFromBounds(int left, int top, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(left, top, 0, 0, bmp.Size);
                return bmp;
            }
        }
        public async void TakeAndSave(int left, int top, int width, int height, string path)
        {
            Bitmap bmp = await CopyFromBounds(left, top, width, height);
            //Attach shadow try1
            //Bitmap shadow = (Bitmap)bmp.Clone();

            //for (int y = 0; y < shadow.Height; y++)
            //{
            //    for (int x = 0; x < shadow.Width; x++)
            //    {
            //        var color = shadow.GetPixel(x, y);
            //        color = Color.FromArgb((int)((double)color.A * 0.2), 0, 0, 0);
            //        shadow.SetPixel(x, y, color);
            //    }
            //}

            //var finalComposite = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalComposite))
            //{
            //        g.Transform = new System.Drawing.Drawing2D.Matrix(
            //        new Rectangle(0, 0, shadow.Width, shadow.Height), 
            //        new System.Drawing.Point[]{
            //        new System.Drawing.Point(50,20),
            //        new System.Drawing.Point(width+50, 20),
            //        new System.Drawing.Point(0, height)
            //    });

            //    g.DrawImageUnscaled(shadow, new System.Drawing.Point(0, 0));

            //    g.ResetTransform();
            //    g.DrawImageUnscaled(bmp, new System.Drawing.Point(0, 0));
            //}

            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }

        private Tuple<double, double,double,double,double, double> ShapeCalculate(double left, double top, double right, double bottom)
        {
            double x = (left / 100) * Glob.BMPCropped.Width; 
            double y = (top / 100) * Glob.BMPCropped.Height;
            double r = Glob.BMPCropped.Width - ((right / 100) * Glob.BMPCropped.Width);
            double b = Glob.BMPCropped.Height - ((bottom / 100) * Glob.BMPCropped.Height);
            double width = r - x;
            double height = b - y;
            return new Tuple<double, double, double, double, double,double>(x,y,r,b,width,height);
        }

        public void test(Canvas canv_Img, System.Collections.Concurrent.ConcurrentDictionary<int, IShapeDetails> MarkingsDictionary, string fileName)
        {
            //Correct resolution test
            //double widthDiff = Glob.BMPCropped.Width - canv_Img.Width > 0 ? (double)Glob.BMPCropped.Width - canv_Img.Width : (double)canv_Img.Width - Glob.BMPCropped.Width;
            //bool isSmallWidth = Glob.BMPCropped.Width - canv_Img.Width > 0 ? false : true;
            //double heightDiff = Glob.BMPCropped.Height - canv_Img.Height > 0 ? (double)Glob.BMPCropped.Height - canv_Img.Height : (double)canv_Img.Height - Glob.BMPCropped.Height;
            //bool isSmallHeight = Glob.BMPCropped.Height - canv_Img.Height > 0 ? false : true;

            //double widthPer = !isSmallWidth ? widthDiff / Glob.BMPCropped.Width : widthDiff / canv_Img.Width;
            //double heightPer = !isSmallHeight ? heightDiff / Glob.BMPCropped.Height : heightDiff / canv_Img.Height;

            Canvas canv = new Canvas();
            canv.Height = Glob.BMPCropped.Height;
            canv.Width = Glob.BMPCropped.Width;
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = Glob.BMPCropped.ConvertToImageSource();
            canv.Background = ib;

            //bmp.Result.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Png);
            var effect = new DropShadowEffect();
            effect.Direction = 320;
            effect.BlurRadius = 5.5;
            effect.ShadowDepth = 4.5;
            System.Windows.Shapes.Rectangle rect = null;
            System.Windows.Shapes.Ellipse circ = null;
            System.Windows.Shapes.Line line = null;
            System.Windows.Controls.TextBox text = null;
            foreach (var item in MarkingsDictionary)
            {
                effect.BlurRadius = 5.5;
                effect.ShadowDepth = 4.5;
                var shapeDetail = item.Value;
                var tu = ShapeCalculate(shapeDetail.Left, shapeDetail.Top, shapeDetail.Right, shapeDetail.Bottom);
                if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Rectangle))
                {
                    rect = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = shapeDetail.brush,
                        StrokeThickness = 2.2,
                        Effect = effect,
                        Width = tu.Item5,
                        Height = tu.Item6
                    };
                    Canvas.SetLeft(rect, tu.Item1);
                    Canvas.SetTop(rect, tu.Item2);
                    canv.Children.Add(rect);
                }
                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Ellipse))
                {
                    circ = new System.Windows.Shapes.Ellipse
                    {
                        Stroke = shapeDetail.brush,
                        StrokeThickness = 2.2,
                        Effect = effect,
                        Width = tu.Item5,
                        Height = tu.Item6
                    };
                    Canvas.SetLeft(circ, tu.Item1);
                    Canvas.SetTop(circ, tu.Item2);
                    canv.Children.Add(circ);
                }
                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Line))
                {
                    if (shapeDetail.LineInvert)
                    {
                        line = new System.Windows.Shapes.Line
                        {
                            Stroke = shapeDetail.brush,
                            StrokeThickness = 2.2,
                            X1 = tu.Item3,
                            Y1 = tu.Item2,
                            X2 = tu.Item1,
                            Y2 = tu.Item4,
                            Effect = effect
                        };
                    }
                    else
                    {
                        line = new System.Windows.Shapes.Line
                        {
                            Stroke = shapeDetail.brush,
                            StrokeThickness = 2.2,
                            X1 = tu.Item1,
                            Y1 = tu.Item2,
                            X2 = tu.Item3,
                            Y2 = tu.Item4,
                            Effect = effect
                        };
                    }
                    canv.Children.Add(line);
                }
                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Controls.TextBox))
                {
                    effect.ShadowDepth = 1;
                    effect.BlurRadius = 3;
                    text = new TextBox()
                    {
                        Foreground = shapeDetail.brush,
                        Background = System.Windows.Media.Brushes.Transparent,
                        BorderBrush = shapeDetail.brush,
                        BorderThickness = new Thickness(2, 0, 0, 2),
                        Effect = effect,
                        FontSize = 50
                    };
                    Canvas.SetLeft(text, tu.Item1);
                    Canvas.SetTop(text, tu.Item2);
                    canv.Children.Add(text);
                }

                //case (DShapes.Line):
                //        MarkingCount++;
                //line = new Line();
                //line.Stroke = Glob.Config.SelectedBrush;
                //line.StrokeThickness = 2.2;
                //line.X1 = startPoint.X;
                //line.Y1 = startPoint.Y;
                //line.Effect = effect;
                //canv_Img.Children.Add(line);
                //break;
                //    case (DShapes.Text):
                //        MarkingCount++;
                //text = new TextBox();
                //text.Foreground = Glob.Config.SelectedBrush;
                //text.Background = Brushes.Transparent;
                //text.BorderBrush = Glob.Config.SelectedBrush;
                //text.BorderThickness = new Thickness(2, 0, 0, 2);
                //effect.ShadowDepth = 1;
                //effect.BlurRadius = 3;
                //text.Effect = effect;
                ////label.Padding = new Thickness(5,5,5,5);
                ////label.BorderThickness = new Thickness(0);
                //text.FontSize = 18;
                //Canvas.SetLeft(text, startPoint.X);
                //Canvas.SetTop(text, startPoint.Y);
                //canv_Img.Children.Add(text);
                ////text.SelectAll();
                //Keyboard.Focus(text);
                //text.LostFocus += Text_LostFocus; //do the text add to MarkingsDictionary in lost focus
                //text.TextChanged += Text_TextChanged;
                //break;
                //    case (DShapes.Circle):
                //        MarkingCount++;
                //circle = new Ellipse();
                //circle.Stroke = Glob.Config.SelectedBrush;
                //circle.StrokeThickness = 2.2;
                //circle.Effect = effect;
                //Canvas.SetLeft(circle, startPoint.X);
                //Canvas.SetTop(circle, startPoint.Y);
                //canv_Img.Children.Add(circle);
                //break;
            }
            var img = canv.ExportCanvasImage().Result;
            img.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName+"-test.png", System.Drawing.Imaging.ImageFormat.Png);


        }


    }
}
