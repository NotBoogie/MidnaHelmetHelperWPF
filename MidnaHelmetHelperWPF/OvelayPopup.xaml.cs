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

        public MainWindow myMainWindow;
        public OvelayPopup()
        {
            InitializeComponent();
            Create3DViewPort();
        }
        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
        public void closePopup(object sender, RoutedEventArgs e)
        {
            //((MainWindow)this.Owner).hideOverlay();
            this.Hide();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // do some stuff here.
            // this.IsHitTestVisible = false;
            //var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
        }

        public void syncWindowPlacement()
        {
            //TODO figure out how to do this crap properly
            var thin = new System.Windows.Thickness(0, 0, 0, 0);
           
            if (HelixViewport.Margin == thin)
            {
                // HelixViewport.Height = 1000;
                var thicc = new System.Windows.Thickness(0, 0, 0, -300);
                HelixViewport.Margin = thicc;
                VerticalPlacementButton.Content = "ADJUSTED";
            }
            else
            {
                //HelixViewport.Height = Double.NaN;
                HelixViewport.Margin = thin;
                VerticalPlacementButton.Content = "CENTERED";
            }
        }
        private void toggleVerticalPlacement(object sender, RoutedEventArgs e)
        {
            //TODO figure out a non-shit way to do this
            bool isCentered = ((string)VerticalPlacementButton.Content == "CENTERED" ? true : false);

            if (isCentered)
            {
                VerticalPlacementButton.Content = "ADJUSTED";
            }
            else
            {
                VerticalPlacementButton.Content = "CENTERED";
            }
            toggleVerticalPlacement();
        }
        public void toggleVerticalPlacement()
        {
            //TODO figure out a non-shit way to do this
            bool isCentered = ((string)VerticalPlacementButton.Content == "CENTERED" ? true : false);
            double xpThick = 0;
            double xnThick = 0;
            double ypThick = 0;
            double ynThick = 0;

            if (myMainWindow.xPaddingSlider.Value < 0)
                xpThick = -myMainWindow.xPaddingSlider.Value;

            if (myMainWindow.xPaddingSlider.Value > 0)
                xpThick = -myMainWindow.xPaddingSlider.Value;

            if (myMainWindow.yPaddingSlider.Value < 0)
                ynThick = -myMainWindow.yPaddingSlider.Value;

            if (myMainWindow.yPaddingSlider.Value > 0)
                ynThick = -myMainWindow.yPaddingSlider.Value;

            var thin = new System.Windows.Thickness(xnThick, ypThick, xpThick, ynThick);

            if (!isCentered)
            {
                thin = new System.Windows.Thickness(xnThick, ypThick, xpThick, ynThick-300);
            }
            else
            {
            }

            HelixViewport.Margin = thin;

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
