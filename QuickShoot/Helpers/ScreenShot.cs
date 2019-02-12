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

            return CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
        }

        public BitmapSource Take(int left, int top, int width, int height)
        {
            return CopyFromBounds(left, top, width, height);
        }

        private BitmapSource CopyFromBounds(int left, int top, int width, int height)
        {
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(left, top, 0, 0, bmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                }

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
