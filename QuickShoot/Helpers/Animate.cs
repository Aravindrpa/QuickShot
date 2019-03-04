using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace QuickShoot.Helpers
{
    public class Animate
    {
        //private static Storyboard sb;
        private static int durationMillli { get; set; }
        public Dictionary<Control,Storyboard> ControlSBs { get; set; }
        public Dictionary<Border,Storyboard> BorderSBs { get; set; }
        public Dictionary<Window,Storyboard> WindowSBs { get; set; }
        public Dictionary<DependencyObject, Storyboard> WindowsSBs { get; set; }
    
        public Storyboard SingleStoryBoard { get; set; }

        public Animate()
        {
            ControlSBs = new Dictionary<Control, Storyboard>();
            BorderSBs = new Dictionary<Border, Storyboard>();
            WindowSBs = new Dictionary<Window, Storyboard>();
            WindowsSBs = new Dictionary<DependencyObject, Storyboard>();

        }

        public void Breath(DependencyObject item)
        {
            StopBreath(item);
            Storyboard sb = null;
            if (WindowsSBs.Keys.Contains(item))
                sb = WindowsSBs[item];
            else
            {
                sb = new Storyboard();
                WindowsSBs.Add(item, sb);
            }
            durationMillli = 1300;

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(durationMillli));
            da.EasingFunction = new QuadraticEase();
            da.AutoReverse = true;

            sb.Children.Add(da);
            sb.RepeatBehavior = RepeatBehavior.Forever;


            Storyboard.SetTarget(sb, item);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));

            sb.Begin();

            //sb.Completed += delegate (object sender, EventArgs e)
            //{
            //    win.Visibility = Visibility.Collapsed;
            //};

        }
        
        public async Task<Animate> InitiateBreath()
        {
            if (SingleStoryBoard != null)
                SingleStoryBoard.Stop();
            else
                SingleStoryBoard = new Storyboard();

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(1300));
            da.EasingFunction = new QuadraticEase();
            da.AutoReverse = true;

            SingleStoryBoard.Children.Add(da);
            SingleStoryBoard.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTargetProperty(SingleStoryBoard, new PropertyPath(Control.OpacityProperty));

            SingleStoryBoard.Begin();
            return this;
        }
        public async Task<Animate> AddChild(DependencyObject item)
        {
            Storyboard.SetTarget(SingleStoryBoard, item);
            return this;
        }

        public void FadeIn(Window win)
        {
            durationMillli = 200;
            var sb = new Storyboard();

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(durationMillli));
            da.EasingFunction = new QuadraticEase();

            sb.Children.Add(da);

            win.Visibility = Visibility.Visible;

            Storyboard.SetTarget(sb, win);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));

            sb.Begin();

            sb.Completed += delegate (object sender, EventArgs e)
            {
                win.Visibility = Visibility.Collapsed;
            };
        }

        public void StopBreath(DependencyObject item)
        {
            if(WindowsSBs.Keys.Contains(item))
                WindowsSBs[item].Stop();
        }
    }
}
