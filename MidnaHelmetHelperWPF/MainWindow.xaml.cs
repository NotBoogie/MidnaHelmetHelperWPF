using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Text.RegularExpressions;

namespace MidnaHelmetHelperWPF
{
    //https://stackoverflow.com/questions/1153009/how-can-i-convert-system-windows-input-key-to-system-windows-forms-keys
    //http://blogs.interknowlogy.com/2007/06/20/transparent-windows-in-wpf-2/
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        //ctrl+f10
        //https://github.com/helix-toolkit/helix-toolkit/wiki/Features-(WPF)
        private HwndSource _source;
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //numbers only
        public MainWindow()
        {
            this.InitializeComponent();
            Create3DViewPort();
            ImageButton.Click += (s, e) => { Close(); };
            MinimizeButton.Click += (s, e) => {
                var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                WindowsServices.SetWindowExTransparent(hwnd);
            };
        }
        public new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        public void load3dModel()
        {
            ObjReader CurrentHelixObjReader = new ObjReader();
            
            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
            //System.Windows.Media.Media3D.Model3DGroup MyModel = CurrentHelixObjReader.Read(@"C:\Users\Jiji\Desktop\Midna Helmet Helper\Fused Shadow.obj");
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            System.Windows.Media.Media3D.Model3DGroup MyModel = CurrentHelixObjReader.Read(currentDirectory+ @"\Models\FusedShadowFull.obj");
            model.Content = MyModel;
            model.AnimateOpacity(50, 1);
        
            //MyModel.Children.Add(MyModel);


        }
        private void SetClipboardAsBG()
        {
            var imageBrush = new ImageBrush(Clipboard.GetImage());
        imageBrush.Stretch = Stretch.None;
            this.Background = (Brush)imageBrush;
        }
        private void SlideOpacity(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var Opac = OpacitySlider.Value;
            model.AnimateOpacity(Opac, 1);
        }
        private void Create3DViewPort()
        {
            var hVp3D = new HelixViewport3D();
            var lights = new DefaultLights();
            hVp3D.Children.Add(lights);
            // hVp3D.Children.Add(teaPot);
            load3dModel();
            //this.AddChild(hVp3D);
            var bc = new BrushConverter();
            
            var transparentBrush = new SolidColorBrush();
            transparentBrush.Opacity = 1;
            transparentBrush.Color = Color.FromArgb(1,255,0,0);
            //this.Background = (Brush)bc.ConvertFrom("#FF222222");
            // this.Background = (Brush)imageBrush;
            //http://blogs.interknowlogy.com/2007/06/20/transparent-windows-in-wpf-2/
            this.Background = transparentBrush;
        }
        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExNotTransparent(hwnd);
        }
        public void CloseOut(object sender, MouseButtonEventArgs args)
        {
            Close();
        }
        public void LockWindow(object sender, MouseButtonEventArgs args)
        {
            
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            WindowsServices w = new WindowsServices();
            _source.AddHook(w.HwndHook);
            w.RegisterHotKey(hwnd);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            //var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
        }

    }
}
