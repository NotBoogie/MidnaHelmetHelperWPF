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
using HelixToolkit;
using HelixToolkit.Wpf;

namespace MidnaHelmetHelperWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Create3DViewPort();

        }

        private void Create3DViewPort()
        {
            var hVp3D = new HelixViewport3D();
            var lights = new DefaultLights();
            var teaPot = new Teapot();
            hVp3D.Children.Add(lights);
            hVp3D.Children.Add(teaPot);
            //this.AddChild(hVp3D);
            var bc = new BrushConverter();
            var imageBrush = new ImageBrush(Clipboard.GetImage());
            imageBrush.Stretch = Stretch.None;
            var transparentBrush = new SolidColorBrush();
            transparentBrush.Opacity = 0;
            transparentBrush.Color = Color.FromArgb(1,255,0,0);
            //this.Background = (Brush)bc.ConvertFrom("#FF222222");
           // this.Background = (Brush)imageBrush;
            this.Background = transparentBrush;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExNotTransparent(hwnd);
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExNotTransparent(hwnd);
        }
        //https://stackoverflow.com/questions/4647345/how-can-i-make-a-window-invisible-to-mouse-events-in-wpf
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExNotTransparent(hwnd);
        }
    }
}
