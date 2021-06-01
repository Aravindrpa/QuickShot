using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QuickShoot.Helpers
{
    public class Glob
    {
        public static Config Config { get; set; }
        public static CaptureWindow captureWindow { get; set; }
        public static EditorWindow editorWindow { get; set; }
        public static EditorWindowLite editorWindowLite { get; set; }
        public static FolderManager folderManager { get; set; }
        public static Animate Animate { get; set; }
        public static ScreenShot ScreenShot { get; set; }
        public static System.Windows.Media.ImageSource Background { get; set; }
        public static System.Drawing.Bitmap BMP { get; set; }//test
        public static System.Drawing.Bitmap BMPCropped { get; set; }//test
        public static int ScreenWidthWithDPI { get; set; }
        public static int ScreenHeightWithDPI { get; set; }
        public static double PrimaryScreenWidth { get; set; }
        public static double PrimaryScreenHeight { get; set; }


        public static async Task<Bitmap> MergeAllBitmaps(Bitmap bmp1, Bitmap bmp2)
        {

            Bitmap finalImage = null;

            if (bmp1.Width > bmp1.Height)
            {
                var bmpT = bmp1.Resize_Picture(Glob.ScreenWidthWithDPI, 0); //goes async
                bmp2 = await bmp2.Resize_Picture(Glob.ScreenWidthWithDPI, 0);//does get result
                bmp1 = await bmpT;//result of alresdy completed task --- speed tweeks
            }
            else if (bmp1.Width < bmp1.Height)
            {
                //C:\Users\ar\Documents\GitRepos\QuickShot-actual\QuickShot\QuickShoot\CaptureWindow.xaml
                var bmpT = bmp1.Resize_Picture(0, Glob.ScreenHeightWithDPI);  //goes async
                bmp2 = await bmp2.Resize_Picture(0, Glob.ScreenHeightWithDPI);//does get result
                bmp1 = await bmpT;//result of alresdy completed task --- speed tweeks
            }
            else if (bmp1.Width == bmp1.Height)
            {
                var bmpT = bmp1.Resize_Picture(Glob.ScreenWidthWithDPI, Glob.ScreenHeightWithDPI);  //goes async
                bmp2 = await bmp2.Resize_Picture(Glob.ScreenWidthWithDPI, Glob.ScreenHeightWithDPI);//does get result
                bmp1 = await bmpT;//result of alresdy completed task --- speed tweeks
            }

            finalImage = new Bitmap(bmp1.Width, bmp1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //bmp2.Save(Glob.folderManager.GetCurrentPath() + "\\shp-" + filename);
            //new Bitmap(Glob.WidthWithDPI, Glob.HeightWithDPI, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(finalImage))//get the underlying graphics object from the image.
            {
                //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                //graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                graphics.DrawImage(bmp1, new Rectangle(0, 0, bmp1.Width, bmp1.Height));
                graphics.DrawImage(bmp2, new Rectangle(0, 0, bmp1.Width, bmp1.Height)); //convert/strech to bmp1 size 

                return finalImage;
            }

        }
        /// <summary>
        /// Method for DPI correction
        /// </summary>
        /// <param name="unitX"></param>
        /// <param name="unitY"></param>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
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

        public static void SetAdjustValues()
        {
            if (Glob.PrimaryScreenHeight < 1080)
            {
                Glob.Config.ShapeThickness = 2;
                Glob.Config.ShapeShadowDepth = 3;
                Glob.Config.ShapeShadowBlurRadius = 4;
                Glob.Config.ShapeShadowBlurRadiusForText = 3;
                Glob.Config.ShapeShadowDepthForText = 1;
                Glob.Config.ShapeShadowDirection = 320;
                Glob.Config.ShapeFontSize = 20;
            }
            else
            {
                Glob.Config.ShapeThickness = 3;
                Glob.Config.ShapeShadowDepth = 4;
                Glob.Config.ShapeShadowBlurRadius = 5;
                Glob.Config.ShapeShadowBlurRadiusForText = 3;
                Glob.Config.ShapeShadowDepthForText = 1;
                Glob.Config.ShapeShadowDirection = 320;
                Glob.Config.ShapeFontSize = 30;


            }
        }

    }
}
