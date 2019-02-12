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
        private static Storyboard sb;
        private static int durationMillli { get; set; }

        public static void Breath(Window win)
        {
            durationMillli = 1300;
            sb = new Storyboard();

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

        public static void FadeIn(Window win)
        {
            durationMillli = 200;
            sb = new Storyboard();

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

        public static void StopBreath()
        {
            sb.Stop();
        }
    }
}
