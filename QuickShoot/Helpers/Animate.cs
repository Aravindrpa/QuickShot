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

        public Animate()
        {
            ControlSBs = new Dictionary<Control, Storyboard>();
            BorderSBs = new Dictionary<Border, Storyboard>();
            WindowSBs = new Dictionary<Window, Storyboard>();

        }

        public void Breath(Window win)
        {
            StopBreath(win);
            Storyboard sb = null;
            if (WindowSBs.Keys.Contains(win))
                sb = WindowSBs[win];
            else
            {
                sb = new Storyboard();
                WindowSBs.Add(win, sb);
            }
            durationMillli = 1300;

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(durationMillli));
            da.EasingFunction = new QuadraticEase();
            da.AutoReverse = true;

            sb.Children.Add(da);
            sb.RepeatBehavior = RepeatBehavior.Forever;

            //item.Opacity = 1;
            win.Visibility = Visibility.Visible;

            Storyboard.SetTarget(sb, win);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));

            sb.Begin();

            sb.Completed += delegate (object sender, EventArgs e)
            {
                win.Visibility = Visibility.Collapsed;
            };

        }
        public void Breath(Border border)
        {
            StopBreath(border);
            Storyboard sb = null;
            if (BorderSBs.Keys.Contains(border))
                sb = BorderSBs[border];
            else
            {
                sb = new Storyboard();
                BorderSBs.Add(border, sb);
            }

            durationMillli = 1300;

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(durationMillli));
            da.EasingFunction = new QuadraticEase();
            da.AutoReverse = true;

            sb.Children.Add(da);
            sb.RepeatBehavior = RepeatBehavior.Forever;

            //item.Opacity = 1;
            border.Visibility = Visibility.Visible;

            Storyboard.SetTarget(sb, border);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));           

            sb.Begin();

            sb.Completed += delegate (object sender, EventArgs e)
            {
                border.Visibility = Visibility.Collapsed;
            };

        }
        public void Breath(Control control)
        {
            StopBreath(control);
            Storyboard sb = null;
            if (ControlSBs.Keys.Contains(control))
                sb = ControlSBs[control];
            else
            {
                sb = new Storyboard();
                ControlSBs.Add(control, sb);
            }
            durationMillli = 1300;

            var da = new DoubleAnimation(0, 1, duration: TimeSpan.FromMilliseconds(durationMillli));
            da.EasingFunction = new QuadraticEase();
            da.AutoReverse = true;

            sb.Children.Add(da);
            sb.RepeatBehavior = RepeatBehavior.Forever;

            //item.Opacity = 1;
            control.Visibility = Visibility.Visible;

            Storyboard.SetTarget(sb, control);
            Storyboard.SetTargetProperty(sb, new PropertyPath(Control.OpacityProperty));

            sb.Begin();

            sb.Completed += delegate (object sender, EventArgs e)
            {
                control.Visibility = Visibility.Collapsed;
            };

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

        public void StopBreath(Window win)
        {
            if(WindowSBs.Keys.Contains(win))
                WindowSBs[win].Stop();
        }
        public void StopBreath(Border border)
        {
            if (BorderSBs.Keys.Contains(border))
                BorderSBs[border].Stop();
        }
        public void StopBreath(Control control)
        {
            if (ControlSBs.Keys.Contains(control))
                ControlSBs[control].Stop();
        }
    }
}
