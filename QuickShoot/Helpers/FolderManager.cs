using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QuickShoot.Helpers
{
    public class FolderManager
    {
        public System.Windows.Threading.DispatcherTimer DTimer { get; set; }
        public FolderManager()
        {
            DTimer = new System.Windows.Threading.DispatcherTimer();
            DTimer.Interval = TimeSpan.FromMilliseconds(1000);
            DTimer.Tick += DTimer_Tick;
            DTimer.Start();
        }

        private void DTimer_Tick(object sender, EventArgs e)
        {
            ManageFolder();
        }

        public void ManageFolder()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");

            if (!Directory.Exists(Glob.Config.ImageStorePath))
                Directory.CreateDirectory(Glob.Config.ImageStorePath);

            if (!Directory.Exists(Glob.Config.ImageStorePath + "\\Today-" + today))
                Directory.CreateDirectory(Glob.Config.ImageStorePath + "\\Today-" + today);

            var li = new DirectoryInfo(Glob.Config.ImageStorePath)
                .EnumerateDirectories()
                .OrderBy(d => d.CreationTime)
                .ToList();
            try
            {

                li.ForEach(dir =>
                {
                    if (dir.Name.Contains("Today-") && !dir.Name.Contains(today))
                    {
                        var newName = dir.Name.Replace("Today-", "");
                        Directory.Move(dir.FullName, dir.Parent.FullName + "\\" + newName);
                        //Remove separation folders and putt all files in parent (optional)
                        foreach (string dirSub in Directory.GetDirectories(dir.Parent.FullName + "\\" + newName))
                        {
                            var direSubInfo = new DirectoryInfo(dirSub);
                            foreach (var file in direSubInfo.GetFiles())
                            {
                                File.Move(file.FullName, dir.Parent.FullName + "\\" + newName + "\\" + file.Name);
                            }
                            Directory.Delete(dirSub);
                        }
                    }
                });
            }
            catch { }

            DTimer.Stop();
        }
        public string GetCurrentPath()
        {
            var parentPath = Glob.Config.ImageStorePath + "\\Today-" + DateTime.Now.ToString("yyyy-MM-dd");
            var path = "";
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (now >= new TimeSpan(04, 0, 0) && now < new TimeSpan(06, 0, 0))
                path = parentPath + "\\Dawn";
            else if (now >= new TimeSpan(06, 0, 0) && now < new TimeSpan(13, 0, 0))
                path = parentPath + "\\Morning";
            //else if (now >= new TimeSpan(11, 0, 0) && now < new TimeSpan(13, 0, 0))
            //    path = parentPath + "//Noon";
            else if (now >= new TimeSpan(13, 0, 0) && now < new TimeSpan(16, 0, 0))
                path = parentPath + "\\Afternoon";
            else if (now >= new TimeSpan(16, 0, 0) && now < new TimeSpan(21, 0, 0))
                path = parentPath + "\\Evening";
            else if (now >= new TimeSpan(21, 0, 0) && now < new TimeSpan(23, 0, 0))
                path = parentPath + "\\Night";
            else if (now >= new TimeSpan(23, 0, 0) || now < new TimeSpan(04, 0, 0))
                path = parentPath + "\\Mid-night";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

    }
}
