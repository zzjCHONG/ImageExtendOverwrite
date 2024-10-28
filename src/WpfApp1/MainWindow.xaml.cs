using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Lift.UI.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //在App.xaml加载theme后，liftui相关的才可用

            string file = @"C:\Users\Administrator\Desktop\控件测试\3_16bit.tif";
            Mat mat = Cv2.ImRead(file);
            ImageViewer.ImageSource = BitmapFrame.Create(mat.ToBitmapSource());
            Image.Source = BitmapFrame.Create(mat.ToBitmapSource());
        }
    }
}
