using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static int WidthWithDPI { get; set; }
        public static int HeightWithDPI { get; set; }

    }
}
