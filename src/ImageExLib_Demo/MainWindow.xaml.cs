using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ImageExLib;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Color = System.Drawing.Color;
using Window = System.Windows.Window;

namespace ImageExLib_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            bool loop = false;
            if (loop)
            {
                _timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                _timer.Tick += (s, e) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        bool isCustom=false;
                        if (isCustom)
                        {
                            var color = _colors[(_count++) % _colors.Count];
                            var img = new Mat(new OpenCvSharp.Size(1660, 1242), MatType.CV_8UC4,
                            new Scalar(color.B, color.G, color.R, color.A));
                            var source = img.ToWriteableBitmap(0, 0, PixelFormats.Bgr32, null);
                            ImageEx.ImageSource = source;
                        }
                        else
                        {
                            var img = Cv2.ImRead(@"C:\Users\Administrator\Desktop\控件测试\2_24bit.jpg");
                            var source = img.ToBitmapSource();
                            ImageEx.ImageSource = source;
                        }                 
                    });
                };
                _timer.Start();

            }
            else
            {
                bool isCustom = true;
                if (isCustom)
                {
                    var color = _colors[1];
                    var img = new Mat(new OpenCvSharp.Size(1660, 1242), MatType.CV_8UC4,
                    new Scalar(color.B, color.G, color.R, color.A));
                    var source = img.ToWriteableBitmap(0, 0, PixelFormats.Bgr32, null);
                    ImageEx.ImageSource = source;
                }
                else
                {
                    var img = Cv2.ImRead(@"C:\Users\Administrator\Desktop\UI\2_24bit.jpg");
                    var source = img.ToBitmapSource();
                    ImageEx.ImageSource = source;
                }
            }          
        }

        #region
        private int _count = 0;
        private DispatcherTimer? _timer;
        private List<Color> _colors = new()
    {
        Color.Transparent,
        Color.AliceBlue,
        Color.AntiqueWhite,
        Color.Aqua,
        Color.Aquamarine,
        Color.Azure,
        Color.Beige,
        Color.Bisque,
        Color.Black,
        Color.BlanchedAlmond,
        Color.Blue,
        Color.BlueViolet,
        Color.Brown,
        Color.BurlyWood,
        Color.CadetBlue,
        Color.Chartreuse,
        Color.Chocolate,
        Color.Coral,
        Color.CornflowerBlue,
        Color.Cornsilk,
        Color.Crimson,
        Color.Cyan,
        Color.DarkBlue,
        Color.DarkCyan,
        Color.DarkGoldenrod,
        Color.DarkGray,
        Color.DarkGreen,
        Color.DarkKhaki,
        Color.DarkMagenta,
        Color.DarkOliveGreen,
        Color.DarkOrange,
        Color.DarkOrchid,
        Color.DarkRed,
        Color.DarkSalmon,
        Color.DarkSeaGreen,
        Color.DarkSlateBlue,
        Color.DarkSlateGray,
        Color.DarkTurquoise,
        Color.DarkViolet,
        Color.DeepPink,
        Color.DeepSkyBlue,
        Color.DimGray,
        Color.DodgerBlue,
        Color.Firebrick,
        Color.FloralWhite,
        Color.ForestGreen,
        Color.Fuchsia,
        Color.Gainsboro,
        Color.GhostWhite,
        Color.Gold,
        Color.Goldenrod,
        Color.Gray,
        Color.Green,
        Color.GreenYellow,
        Color.Honeydew,
        Color.HotPink,
        Color.IndianRed,
        Color.Indigo,
        Color.Ivory,
        Color.Khaki,
        Color.Lavender,
        Color.LavenderBlush,
        Color.LawnGreen,
        Color.LemonChiffon,
        Color.LightBlue,
        Color.LightCoral,
        Color.LightCyan,
        Color.LightGoldenrodYellow,
        Color.LightGray,
        Color.LightGreen,
        Color.LightPink,
        Color.LightSalmon,
        Color.LightSeaGreen,
        Color.LightSkyBlue,
        Color.LightSlateGray,
        Color.LightSteelBlue,
        Color.LightYellow,
        Color.Lime,
        Color.LimeGreen,
        Color.Linen,
        Color.Magenta,
        Color.Maroon,
        Color.MediumAquamarine,
        Color.MediumBlue,
        Color.MediumOrchid,
        Color.MediumPurple,
        Color.MediumSeaGreen,
        Color.MediumSlateBlue,
        Color.MediumSpringGreen,
        Color.MediumTurquoise,
        Color.MediumVioletRed,
        Color.MidnightBlue,
        Color.MintCream,
        Color.MistyRose,
        Color.Moccasin,
        Color.NavajoWhite,
        Color.Navy,
        Color.OldLace,
        Color.Olive,
        Color.OliveDrab,
        Color.Orange,
        Color.OrangeRed,
        Color.Orchid,
        Color.PaleGoldenrod,
        Color.PaleGreen,
        Color.PaleTurquoise,
        Color.PaleVioletRed,
        Color.PapayaWhip,
        Color.PeachPuff,
        Color.Peru,
        Color.Pink,
        Color.Plum,
        Color.PowderBlue,
        Color.Purple,
        Color.Red,
        Color.RosyBrown,
        Color.RoyalBlue,
        Color.SaddleBrown,
        Color.Salmon,
        Color.SandyBrown,
        Color.SeaGreen,
        Color.SeaShell,
        Color.Sienna,
        Color.Silver,
        Color.SkyBlue,
        Color.SlateBlue,
        Color.SlateGray,
        Color.Snow,
        Color.SpringGreen,
        Color.SteelBlue,
        Color.Tan,
        Color.Teal,
        Color.Thistle,
        Color.Tomato,
        Color.Turquoise,
        Color.Violet,
        Color.Wheat,
        Color.White,
        Color.WhiteSmoke,
        Color.Yellow,
        Color.YellowGreen,
        Color.RebeccaPurple,
    };
        #endregion

        private void anotherway_Click(object sender, RoutedEventArgs e)
        {
            ImageEx.XGridCount = 10;
            ImageEx.YGridCount = 15;
            ImageEx.XGridFillCount = 6;
            ImageEx.YGridFillCount = 5;
            ImageEx.RefreshFillGridBrush();
        }

        private void ImageEx_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not ImageEx viewer) return;

            var pos = viewer.ImageCurrentPosition;
            Debug.WriteLine($"ImageCurrentPosition {pos.x}-{pos.y}");

            var gridXY = viewer.GridFillCurrentPos;
            Debug.WriteLine($"GridCurrentPos  {gridXY.x}-{gridXY.y}");

        }

        private void ReDraw_Click(object sender, RoutedEventArgs e)
        {
            ImageEx.isPolygonDrawingRedraw = true;
        }
    }
}
