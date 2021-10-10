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
using System.Windows.Media.Media3D;

namespace MidnaHelmetHelperWPF
{
    //https://stackoverflow.com/questions/1153009/how-can-i-convert-system-windows-input-key-to-system-windows-forms-keys
    //http://blogs.interknowlogy.com/2007/06/20/transparent-windows-in-wpf-2/
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class textureControl
    {
        private Button _toggleButton;
        private string _fileLocation;

        public Button toggleButton
        {
            get => _toggleButton;
            set
            {
                    _toggleButton = value;
            }
        }

        public string fileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
            }
        }
    }
    public partial class MainWindow
    {

        //ctrl+f10
        //https://github.com/helix-toolkit/helix-toolkit/wiki/Features-(WPF)
        private HwndSource _source;
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //numbers only
        private OvelayPopup overlayPopup;
        private System.Windows.Media.Media3D.ModelVisual3D overlayPopupModel;
        private Dictionary<string, string> textureDictionary = new Dictionary<string, string>();
        private string currentTexture = "";
        private string baseTexture = "";
        private string lineTexture = "";
        private bool backsShown = false;
        private bool frontsShown = true;
        public MainWindow()
        {
            this.InitializeComponent();
            if (overlayPopup == null)
                overlayPopup = new OvelayPopup();
            loadOverlayModel();
            
            loadTextures();
            highlightButton(BaseMaterial, true);
            swapTexture2(textureDictionary["current"], true, false, false);
            highlightButton(HidePolyBacks, true);
            //overlayPopupModel = overlayPopup.model;
        }
        private void closeApp(object sender, RoutedEventArgs e)
        {
            if (overlayPopup != null)
                overlayPopup.Close();
            Close();
        }
            private void lockTransparency(object sender, RoutedEventArgs e)
        {
            //var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            highlightButton(LockTransparencyButton, true);
            highlightButton(ReleaseTransparencyButton, false);
            var hwnd = new System.Windows.Interop.WindowInteropHelper(overlayPopup).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
        private void releaseTransparency(object sender, RoutedEventArgs e)
        {
            //var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            highlightButton(LockTransparencyButton, false);
            highlightButton(ReleaseTransparencyButton, true);
            var hwnd = new System.Windows.Interop.WindowInteropHelper(overlayPopup).Handle;
            WindowsServices.SetWindowExNotTransparent(hwnd);
        }

        private void showOverlay(object sender, RoutedEventArgs e)
        {
            showOverlay();
        }
        private void hideOverlay(object sender, RoutedEventArgs e)
        {
            hideOverlay();
        }
        private void minimizeToTray(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void helpPopup(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Show the popup, position it over your sketch, position the model as needed, lock and draw.\n\nScroll wheel and M2 can manipulate the model camera, the sliders can be used for refinement.\n\nIf you just want a classic reference, snip a screenshot of your sketch to the clipboard and use the PASTE BG button to do the same thing in the floating window.\n\nYou can also load your own model or textures into the app via the LOAD CUSTOM buttons.  Models need to be of .OBJ format; textures can probably be anything but I stick with .PNG's.\n\nTODO make this a stylized popup or something");
        }
        /*
        public static Point3D GetPosition(this Matrix3D m)
{
    return new Point3D
    {
        X = m.OffsetX,
        Y = m.OffsetY,
        Z = m.OffsetZ
    };
}*/
        private void showOverlay()
        {
            overlayPopup.Show();

            //var brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF06FCB1");
            //SpawnOverlayButton.Background = brushy;

        }
        private void hideOverlay()
        {
            //var brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
            //SpawnOverlayButton.Background = brushy;
            overlayPopup.Hide();
        }
        private void showBacks(object sender, RoutedEventArgs e)
        {
            backsShown = true;
            swapTexture2(textureDictionary["current"], frontsShown, backsShown,false);
            highlightButton(ShowPolyBacks, true);
            highlightButton(HidePolyBacks, false);
        }
        private void hideBacks(object sender, RoutedEventArgs e)
        {
            backsShown = false;
            swapTexture2(textureDictionary["current"], frontsShown, backsShown, false);
            highlightButton(ShowPolyBacks, false);
            highlightButton(HidePolyBacks, true);
        }
        private void loadBaseTexture(object sender, RoutedEventArgs e)
        {
            swapTexture2(textureDictionary["base"], frontsShown, backsShown);
            highlightButton(BaseMaterial, true);
            highlightButton(LineMaterial, false);
            highlightButton(CustomMaterial, false);
        }
        private void loadLineTexture(object sender, RoutedEventArgs e)
        {
            swapTexture2(textureDictionary["line"], frontsShown, backsShown);
            highlightButton(BaseMaterial, false);
            highlightButton(LineMaterial, true);
            highlightButton(CustomMaterial, false);
        }
        private void loadOriginalTexture(object sender, RoutedEventArgs e)
        {
            swapTexture2(textureDictionary["base"], frontsShown, backsShown);
            highlightButton(BaseMaterial, false);
            highlightButton(LineMaterial, true);
            highlightButton(CustomMaterial, false);
        }
        private void highlightButton(Button targ, bool onOff)
        {
            if (onOff)
            {
                var brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF06FCB1");
                targ.Background = brushy;
                brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
                targ.Foreground = brushy;
            }
            else
            {
                var brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#000000");
                targ.Background = brushy;
                brushy = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF06FCB1");
                targ.Foreground = brushy;
            }
        }
        private void loadCustomTexture(object sender, RoutedEventArgs e)
        {
            string fileName = getFileSelection("PNG Files (*.png)|*.png");
            if (fileName != "")
            {
                swapTexture2(fileName, frontsShown, backsShown);
                highlightButton(BaseMaterial, false);
                highlightButton(LineMaterial, false);
                highlightButton(CustomMaterial, true);
            }
        }
        public new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void loadOverlayModel()
        {
            if (overlayPopup != null)
                overlayPopupModel = overlayPopup.model;
        }
        private void loadTextures()
        {
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (!textureDictionary.ContainsKey("current"))
                textureDictionary["current"] = currentDirectory + @"\Models\4.png";
            if (!textureDictionary.ContainsKey("line"))
                textureDictionary["line"] = currentDirectory + @"\Models\4.png";
            if (!textureDictionary.ContainsKey("base"))
                textureDictionary["base"] = currentDirectory + @"\Models\4.png";
            /*
            if (currentTexture == "")
                currentTexture = currentDirectory + @"\Models\4.png";
            if (lineTexture == "")
                lineTexture = currentDirectory + @"\Models\4.png"; 
            if (baseTexture == "")
                baseTexture = currentDirectory + @"\Models\4.png";
            */
        }
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void SetClipboardAsBG(object sender, RoutedEventArgs e)
        {
            var imageBrush = new ImageBrush(Clipboard.GetImage());
            
                imageBrush.AlignmentX = AlignmentX.Center;
                imageBrush.AlignmentY = AlignmentY.Center;

        imageBrush.Stretch = Stretch.None;
            overlayPopup.Background = (Brush)imageBrush;
        }
        private void SetClipboardAsTransparent(object sender, RoutedEventArgs e)
        {
            var brushy = new SolidColorBrush(Colors.Transparent);


            overlayPopup.Background = (Brush)brushy;
        }
        private void SlideOpacity(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double Opac = OpacitySlider.Value/100;
            if (overlayPopupModel != null)
            {
                overlayPopup.HelixViewport.Opacity = Opac;
            }
        }
        private void SlideAll(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (overlayPopupModel != null)
            {
                var currentTransform = overlayPopupModel.Transform.Value;
                var scaleVal = ScaleSlider.Value / 100;
                // overlayPopupModel.Transform = new MatrixTransform3D(matrix);
                overlayPopupModel.Transform = new MatrixTransform3D(CalculateTransformMatrix(ySlider.Value, zSlider.Value, xSlider.Value, scaleVal, scaleVal, scaleVal));
                
            }
        }
        private void SlideScale(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (overlayPopupModel != null)
            {
                var currentTransform = overlayPopupModel.Transform.Value;
                var val = ScaleSlider.Value/100;
                // overlayPopupModel.Transform = new MatrixTransform3D(matrix);
                overlayPopupModel.Transform = new MatrixTransform3D(CalculateScaleMatrix(val, val, val));
                
            }
        }
        private string getFileSelection(string typeFilterString)
        {
            string fileName = "";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".obj";
            dlg.Filter = typeFilterString;


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
                // Open document 
                fileName = dlg.FileName;
            return fileName;
        }
        private void BtnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            string fileName = getFileSelection("OBJ Files (*.obj)|*.obj");
            if (fileName != "")
            {
                overlayPopup.load3dModel(fileName);
            }
        }
        //TODO fug dis
        private void swapMaterial(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Material myMaterial = MaterialHelper.CreateImageMaterial(currentDirectory + @"\Models\4.png", 1);

           // Model3DGroup modelg = (Model3DGroup)overlayPopupModel.Content;
            //GeometryModel3D model = (GeometryModel3D)modelg.Children[0];
            //model.Material = new DiffuseMaterial(Brushes.Blue);

            for (int i = 0; i < ((System.Windows.Media.Media3D.Model3DGroup)overlayPopupModel.Content).Children.Count; i++)
            {

                var m = ((System.Windows.Media.Media3D.Model3DGroup)overlayPopupModel.Content).Children[i];
                //keeping lights
                if (m is Light == false)
                {
                    //Model3DGroup modelg = (Model3DGroup)m;
                    GeometryModel3D modell = (GeometryModel3D)m;
                    var gg =( MeshGeometry3D )modell.Geometry;
                    var previousTextureCoordinates = gg.TextureCoordinates;
                    //gg.TextureCoordinates
                    modell.Material = myMaterial;//new DiffuseMaterial(Brushes.Blue);

                    modell.BackMaterial = new DiffuseMaterial(Brushes.Transparent);
                    gg.TextureCoordinates = previousTextureCoordinates;
                }

            }
        }

        private void swapTexture2(string texturePath, bool fronts, bool backs, bool refreshFront=true)
        {
            if (overlayPopup.model != null)
                return;
            textureDictionary["current"] = texturePath;
            Material myMaterial = MaterialHelper.CreateImageMaterial(texturePath, 1);
            Material transpMaterial = new DiffuseMaterial(Brushes.Transparent);
            for (int i = 0; i < ((System.Windows.Media.Media3D.Model3DGroup)overlayPopupModel.Content).Children.Count; i++)
            {

                var m = ((System.Windows.Media.Media3D.Model3DGroup)overlayPopupModel.Content).Children[i];
                //keeping lights
                if (m is Light == false)
                {
                    GeometryModel3D modell = (GeometryModel3D)m;
                    //gg.TextureCoordinates
                    if(refreshFront)
                    if(fronts)
                        modell.Material = myMaterial;
                    else
                        modell.Material = transpMaterial;

                    if (backs)
                        modell.BackMaterial = myMaterial;
                    else
                        modell.BackMaterial = transpMaterial;
                }

            }
        }

        Matrix3D CalculateRotationMatrix(double x, double y, double z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), z));
            //This format will make it rotate relative to the camera
            //matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }

        Matrix3D CalculateScaleMatrix(double x, double y, double z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Scale(new Vector3D(x, y, z));
            //This format will make it rotate relative to the camera
            //matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }
        Matrix3D CalculateTransformMatrix(double x, double y, double z, double xScale, double yScale, double zScale)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), y));
            matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), z));
            matrix.Scale(new Vector3D(xScale, yScale, zScale));
            //This format will make it rotate relative to the camera
            //matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * matrix, z));

            return matrix;
        }
        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
         public void Button_Click(object sender, MouseButtonEventArgs args)
        {
            
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
            overlayPopup.Close();
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
        private double angleTracker;
        private void rotate(object sender, EventArgs e)
        {
            if (this.angleTracker >= 360)
            {
                this.angleTracker = 0;
            }
            else
            {
                //Nothing to do
            }
            this.angleTracker = this.angleTracker + 0.25;
            //You can adapt the code if you have many children 
            GeometryModel3D geometryModel3D = (GeometryModel3D)((Model3DGroup)overlayPopupModel.Content).Children.First();
            if (geometryModel3D.Transform is RotateTransform3D rotateTransform3 && rotateTransform3.Rotation is AxisAngleRotation3D rotation)
            {
                rotation.Angle = this.angleTracker;
            }
            else
            {
                ///Initialize the Transform (I didn't do it in my example but you could do this initialization in <see Load3dModel/>)
                geometryModel3D.Transform = new RotateTransform3D()
                {
                    Rotation = new AxisAngleRotation3D()
                    {
                        Axis = new Vector3D(0, 1, 0),
                        Angle = this.angleTracker,
                    }
                };
            }
        }
    }
}
