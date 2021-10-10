using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace MidnaHelmetHelperWPF
{
    /// <summary>
    /// Interaction logic for OvelayPopup.xaml
    /// </summary>
    public partial class OvelayPopup : Window
    {
        public OvelayPopup()
        {
            InitializeComponent();
            Create3DViewPort();
        }
        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            //var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
        }
        public void load3dModel(string modelLocation = "")
        {
            //ObjReader CurrentHelixObjReader = new ObjReader();
            ObjReader CurrentHelixObjReader = new HelixToolkit.Wpf.ObjReader();

            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
            //System.Windows.Media.Media3D.Model3DGroup MyModel = CurrentHelixObjReader.Read(@"C:\Users\Jiji\Desktop\Midna Helmet Helper\Fused Shadow.obj");
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (modelLocation == "")
                modelLocation = currentDirectory + @"\Models\FusedShadowNoBackTopo.obj";
            try
            {
                System.Windows.Media.Media3D.Model3DGroup MyModel = CurrentHelixObjReader.Read(modelLocation);
                model.Content = MyModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hit an error while trying to load the model: " + ex.Message, "Model Loader", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //MyModel.Children.Add(MyModel);
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
            transparentBrush.Color = Color.FromArgb(1, 255, 0, 0);
            //this.Background = (Brush)bc.ConvertFrom("#FF222222");
            // this.Background = (Brush)imageBrush;
            //http://blogs.interknowlogy.com/2007/06/20/transparent-windows-in-wpf-2/
            this.Background = transparentBrush;
        }
    }
}
