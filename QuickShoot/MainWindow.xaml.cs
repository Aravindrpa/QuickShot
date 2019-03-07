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

            Glob.Config = new Config();
            if(String.IsNullOrEmpty(Glob.Config.ImageStorePath))//Set default save path
                Glob.Config.ImageStorePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\QuickShot";

            Glob.ScreenShot = new ScreenShot();
            Glob.captureWindow = new CaptureWindow();
            Glob.folderManager = new FolderManager();
            Glob.Animate = new Animate();

            //Glob.Animate.Breath(this);

            //CONFIG
            Glob.Config.SelectedBrush = System.Windows.Media.Brushes.Red;
            Glob.Config.EnableAnimations = true;
            Glob.Config.EnableBlurEffect = true;
            Glob.Config.EnableLiteMode = false;


            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 150;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - 150;

            //var t = Glob.Animate.InitiateBreath().Result;
            //Glob.Animate.Breath(this);

            //#### Start key hook
            hookPrntScr = new GlobalKeyboardHook();
            hookPrntScr.KeyboardPressed += HookPrntScr_KeyboardPressed;
        }

        bool ControlPressed = false;
        //Hook Event
        private void HookPrntScr_KeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                switch (e.KeyboardData.VirtualCode)
                {
                    case (GlobalKeyboardHook.VkSnapshot):
                            Glob.captureWindow = new CaptureWindow();
                            Glob.captureWindow.Show();                       
                        break;                      
                    case (GlobalKeyboardHook.VkEscape):
                        foreach (Window win in Application.Current.Windows)
                        {
                            if (win.Name == "capture_window")
                                if (win.IsActive)
                                    win.Close();
                            if (win.Name == "editor_window")
                                if (win.IsActive)
                                    win.Close();
                            if (win.Name == "editor_window_lite")
                                if (win.IsActive)
                                    win.Close();
                        }
                        break;
                    case (GlobalKeyboardHook.VkReturn):
                        foreach (Window win in Application.Current.Windows)
                        {
                            if (win.Name == "editor_window")
                                if (win.IsActive) //Not gonna be null if active
                                {
                                    Glob.editorWindow.SaveAndClose();
                                }
                            if (win.Name == "editor_window_lite")
                                if (win.IsActive) //Not gonna be null if active
                                {
                                    Glob.editorWindowLite.SaveAndClose();
                                }
                        }
                        break;
                    case(162): // this is when control pressed
                        ControlPressed = true;
                        break;
                    default://this is 'C' - and it needs to work only if control was pressed before
                        if (ControlPressed)
                        {
                            foreach (Window win in Application.Current.Windows)
                            {
                                if (win.Name == "editor_window")
                                    if (win.IsActive) //Not gonna be null if active
                                    {
                                        if(e.KeyboardData.VirtualCode == 67)
                                            Glob.editorWindow.CopyImage();
                                        else if(e.KeyboardData.VirtualCode == 83)
                                            Glob.editorWindow.SaveAndClose();
                                    }
                                if (win.Name == "editor_window_lite")
                                    if (win.IsActive) //Not gonna be null if active
                                    {
                                        if (e.KeyboardData.VirtualCode == 67)
                                            Glob.editorWindowLite.CopyImage();
                                        else if (e.KeyboardData.VirtualCode == 83)
                                            Glob.editorWindowLite.SaveAndClose();
                                    }
                            }
                            ControlPressed = false;//release
                        }
                        break;
                }
            }
        }
        //Control Event Handlers
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
        private void Main_Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown(); //make sure when this is closed, app is closed as well
        }
    }
}
