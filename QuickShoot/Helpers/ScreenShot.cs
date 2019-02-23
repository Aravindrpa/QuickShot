using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace QuickShoot.Helpers
{
    class ScreenShot
    {
        public BitmapSource Take()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            int screenWidthdpi = 0;
            int screenHeightdpi = 0;
            TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);

            var bmp = CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
            return ConvertBmpToSource(bmp);
        }
        public BitmapSource Take(int left, int top, int width, int height)
        {
            var bmp = CopyFromBounds(left, top, width, height);
            return ConvertBmpToSource(bmp);
        }
        public void TakeAndSave(int left, int top, int width, int height,string path)
        {
            Bitmap bmp = CopyFromBounds(left, top, width, height);

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
        private BitmapSource ConvertBmpToSource(Bitmap bmp)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
        private Bitmap CopyFromBounds(int left, int top, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);          
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(left, top, 0, 0, bmp.Size);
                return bmp;
            } 
        }
        public static void TransformToPixels(double unitX,
                               double unitY,
                               out int pixelX,
                               out int pixelY)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                pixelX = (int)((g.DpiX / 96) * unitX);
                pixelY = (int)((g.DpiY / 96) * unitY);
            }

        }
    }
}
