using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
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
            TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);

            var bmp = await CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
            return await ConvertBmpToSource(bmp);
        }
        public async Task<BitmapSource> Take(int left, int top, int width, int height)
        {
            var bmp = await CopyFromBounds(left, top, width, height);
            return await ConvertBmpToSource(bmp);
        }
        public async Task<Bitmap> TakeBitmap()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;
            int screenWidthdpi = 0;
            int screenHeightdpi = 0;
            TransformToPixels(screenWidth, screenHeight, out screenWidthdpi, out screenHeightdpi);

            return await CopyFromBounds((int)screenLeft, (int)screenTop, screenWidthdpi, screenHeightdpi);
        }

        public async Task<Bitmap> Crop(Bitmap bmp,int x1, int y1, int width, int height)
        {
            Rectangle cropRect = new Rectangle(x1, y1, width, height);
            return await Crop(bmp, cropRect);
        }
        public async Task<Bitmap> Crop(Bitmap bmp, Rectangle rect)
        {
            Bitmap target = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(bmp,
                    new Rectangle(0, 0, target.Width, target.Height),
                    rect,
                    GraphicsUnit.Pixel);
            }
            return target;
        }
        public async Task<Bitmap> MergeAllBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap finalImage = new Bitmap(bmp1.Width, bmp1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(finalImage))//get the underlying graphics object from the image.
            {

                graphics.DrawImage(bmp1, new Rectangle(0, 0, bmp1.Width, bmp1.Height));
                graphics.DrawImage(bmp2, new Rectangle(0, 0, bmp2.Width, bmp2.Height));

                return finalImage;
            }

        }

        public async void TakeAndSave(int left, int top, int width, int height,string path)
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
        public async Task<BitmapSource> ConvertBmpToSource(Bitmap bmp)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
        public Bitmap ConvertSourceToBmp(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
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
