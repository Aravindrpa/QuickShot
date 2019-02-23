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
        public static FolderManager folderManager { get; set; }
        public static Animate Animate { get; set; }
        public static ScreenShot ScreenShot { get; set; }
    }
}
