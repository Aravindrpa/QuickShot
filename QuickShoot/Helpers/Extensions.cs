using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuickShoot.Helpers
{
    public static class Extensions
    {
      

        public static Bitmap ConvertSourceToBmp(this BitmapSource bitmapsource)
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
        public static async Task<BitmapSource> ConvertToBitmapSource(this Bitmap bmp)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
        public static async Task<Bitmap> Resize_Picture(this Bitmap bmp, int FinalWidth, int FinalHeight)
        {
            System.Drawing.Bitmap NewBMP;
            System.Drawing.Graphics graphicTemp;


            int iWidth;
            int iHeight;
            if ((FinalHeight == 0) && (FinalWidth != 0))
            {
                iWidth = FinalWidth;
                iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
            }
            else if ((FinalHeight != 0) && (FinalWidth == 0))
            {
                iHeight = FinalHeight;
                iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
            }
            else
            {
                if (FinalWidth > FinalHeight)
                {
                    iWidth = FinalWidth;
                    iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
                }
                else if (FinalHeight > FinalWidth)
                {
                    iHeight = FinalHeight;
                    iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
                }
                else
                {
                    iWidth = FinalWidth;
                    iHeight = FinalHeight;
                }
            }

            NewBMP = new System.Drawing.Bitmap(iWidth, iHeight);
            using (graphicTemp = System.Drawing.Graphics.FromImage(NewBMP))
            {
                //graphicTemp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                //graphicTemp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //graphicTemp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //graphicTemp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //graphicTemp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphicTemp.DrawImage(bmp, 0, 0, iWidth, iHeight);
                return NewBMP;
                //graphicTemp.Dispose();
            }
            //System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
            //System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ImageQuality);
            //encoderParams.Param[0] = encoderParam;
            //System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            //for (int fwd = 0; fwd <= arrayICI.Length - 1; fwd++)
            //{
            //    if (arrayICI[fwd].FormatDescription.Equals("JPEG"))
            //    {
            //        NewBMP.Save(Des, arrayICI[fwd], encoderParams);
            //    }
            //}

            //NewBMP.Dispose();
            //bmp.Dispose();
        }
        public static async Task<Bitmap> Crop(this Bitmap bmp, int x1, int y1, int width, int height)
        {
            Rectangle cropRect = new Rectangle(x1, y1, width, height);
            return await Crop(bmp, cropRect);
        }
        public static async Task<Bitmap> Crop(this Bitmap bmp, Rectangle rect)
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
        public static async Task<Bitmap> ExportCanvasImage(this Canvas canvas)
        {
            int wid = 0; int hei = 0;
            //ScreenShot.TransformToPixels(canvas.ActualWidth, canvas.ActualHeight, out wid, out hei);
            wid = canvas.ActualWidth != 0 ? (int)canvas.ActualWidth : (int)canvas.Width;
            hei = canvas.ActualHeight != 0 ? (int)canvas.ActualHeight : (int)canvas.Height;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             wid, hei,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new System.Windows.Size(wid, hei));
            canvas.Arrange(new Rect(new System.Windows.Size(wid, hei)));

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
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ConvertToImageSource(this Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static float GetPercentage(this int value, float percentage = 0)
        {
            if (percentage == 0)
            {
                percentage = (Glob.ScreenHeightWithDPI <= 768) ? 12 : Glob.ScreenHeightWithDPI <= 1080 ? 20 : 20;
            }           
            return (float)value * (percentage / 100);
        }

        public static int TransformToDevicePixel(this double unitX)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return (int)((g.DpiX / 96) * unitX);
            }

        }
        public static int TransformToDevicePixel(this int unitX)
        {
            return Convert.ToDouble(unitX).TransformToDevicePixel();
        }
    }
}
