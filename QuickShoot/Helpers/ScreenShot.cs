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


        public void test(Bitmap canvasImage, System.Collections.Concurrent.ConcurrentDictionary<int, IShapeDetails> MarkingsDictionary)
        {
            //Correct resolution test
            System.Drawing.Bitmap finalImage = new System.Drawing.Bitmap(Glob.BMPCropped.Width, Glob.BMPCropped.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int widthPer = ((Glob.BMPCropped.Width - canvasImage.Width)/Glob.BMPCropped.Width); 
            //bmp2.Save(Glob.folderManager.GetCurrentPath() + "\\shp-" + filename);
            //new Bitmap(Glob.WidthWithDPI, Glob.HeightWithDPI, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(finalImage))//get the underlying graphics object from the image.
            {

                graphics.DrawImage(Glob.BMPCropped, new Rectangle(0, 0, Glob.BMPCropped.Width, Glob.BMPCropped.Height));
                // graphics.DrawImage(bmp2, new Rectangle(0, 0, bmp1.Width, bmp1.Height)); //convert/strech to bmp1 size 

                foreach (var item in MarkingsDictionary)
                {
                    var sshape = item.Value.GetStoredShape();

                    if (sshape.GetType() == typeof(System.Windows.Shapes.Rectangle))
                    {
                        Console.WriteLine(sshape);
                        var ss = sshape;
                        //graphics.DrawRectangle(new Pen(Brushes.Beige), (System.Windows.Shapes.Rectangle)sshape);
                        //graphics.DrawEllipse(new Pen(Brushes.Beige))
                    }
                    //else if (sshape.GetType() == typeof(Line))
                    //{
                    //}
                    //else if (sshape.GetType() == typeof(Ellipse))
                    //{

                    //}
                    //else if (sshape.GetType() == typeof(TextBox))
                    //{

                    //}

                }
                

            }
           
            
        }


    }
}
