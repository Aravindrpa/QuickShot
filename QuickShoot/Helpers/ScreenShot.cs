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

        private Tuple<double, double, double, double> ShapeCalculate(double left, double top, double right, double bottom)
        {
            double x = (left / 100) * Glob.BMPCropped.Width; 
            double y = (top / 100) * Glob.BMPCropped.Height;
            double r = Glob.BMPCropped.Width - ((right / 100) * Glob.BMPCropped.Width);
            double b = Glob.BMPCropped.Height - ((bottom / 100) * Glob.BMPCropped.Height);
            double width = r - x;
            double height = b - y;
            return new Tuple<double, double, double,double>(x,y,width,height);
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
                var shapeDetail = item.Value;
                var tu = ShapeCalculate(shapeDetail.Left, shapeDetail.Top, shapeDetail.Right, shapeDetail.Bottom);
                if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Rectangle))
                {
                    rect = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = Glob.Config.SelectedBrush,
                        StrokeThickness = 2.2,
                        Effect = effect,
                        //Height = shapeDetail.height + (shapeDetail.height + heightPer),
                        //Width = shapeDetail.width + (shapeDetail.width + widthPer)
                        Width = tu.Item3,
                        Height = tu.Item4
                    };
                    //Canvas.SetLeft(rect, shapeDetail.StartPoint.X);
                    //Canvas.SetTop(rect, shapeDetail.StartPoint.Y);
                    Canvas.SetLeft(rect, tu.Item1);
                    Canvas.SetTop(rect, tu.Item2);
                    canv.Children.Add(rect);
                }

                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Ellipse))
                {
                }
                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Shapes.Line))
                {
                }
                else if (shapeDetail.StoredShapeType == typeof(System.Windows.Controls.TextBox))
                {
                }
            }
            var img = canv.ExportCanvasImage().Result;
            img.Save(Glob.folderManager.GetCurrentPath() + "\\" + fileName+"-test.png", System.Drawing.Imaging.ImageFormat.Png);


        }


    }
}
