using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickShoot.Helpers;

namespace QuickShoot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalKeyboardHook hookPrntScr;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();

            Glob.captureWindow = new CaptureWindow();

            Glob.Config = new Config();
            if(String.IsNullOrEmpty(Glob.Config.ImageStorePath))//Set default save path
                Glob.Config.ImageStorePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\QuickShot";

            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 150;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - 150;

            Animate.Breath(this);

            //#### Start key hook
            hookPrntScr = new GlobalKeyboardHook();
            hookPrntScr.KeyboardPressed += HookPrntScr_KeyboardPressed;
        }

        private void HookPrntScr_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                switch (e.KeyboardData.VirtualCode)
                {
                    case (GlobalKeyboardHook.VkSnapshot):
                        Glob.captureWindow.Show();
                        break;
                    case (GlobalKeyboardHook.VkEscape):
                        foreach (Window win in Application.Current.Windows)
                        {
                            if (win.Name == "capture_window")
                                if (win.IsActive)
                                    win.Hide();
                            if (win.Name == "editor_window")
                                if (win.IsActive)
                                    win.Close();
                        }
                        break;
                }
            }
        }

        private void grid_Clear_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                isDragging = true;
            }
        }
        private void grid_Clear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    //foreach (Window win in Application.Current.Windows)
                    //    if (win.Name == "main_window")
                    //        win.Close();

                    //mw = new MainWindow();
                    //mw.Show();

                    //Open the image list window if any
                }
                else
                {
                    //menu if any
                }
            }

            isDragging = false;
        }
    }
}
