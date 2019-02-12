using QuickShoot.Helpers;
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
using System.Windows.Shapes;

namespace QuickShoot
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public EditorWindow(int left, int top, int height, int width)
        {
            InitializeComponent();
            img_Blur.Source = new ScreenShot().Take();
            img_Edit.Source = new ScreenShot().Take(left, top, height, width);
        }

        private void lbl_Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
