using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace ImageExLib
{
    public class ImageEx : ContentControl
    {
        static ImageEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEx), new FrameworkPropertyMetadata(typeof(ImageEx)));
        }

        public ImageEx()
        {
            //图像处理
            CommandBindings.Add(new CommandBinding(ImageProcessCommand, (obj, args) =>
            {
                if (args.Parameter is not string command) return;

                if (command == SaveDump)
                {
                    if (InkCanvas == null) return;

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    saveFileDialog.Filter = "TIFF files (*.tif)|*.tif|TIFF files (*.tiff)|*.tiff|All files (*.*)|*.*";
                    saveFileDialog.DefaultExt = "tif";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.FileName = "DumpImage";

                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        var bitmapImage = InkCanvastoBitmapImage(InkCanvas);
                        SaveBitmapImage(bitmapImage, saveFileDialog.FileName);
                        OpenFolderAndSelectFile(saveFileDialog.FileName);
                    }
                }
                else if(command==MsgBox)
                {
                    MessageBox.Show("asd");
                }
            }));
        }      

        #region ImageProcessCommand

        //todo，尝试将此功能使用外界命令调用
        public static readonly RoutedUICommand ImageProcessCommand = new();

        public const string SaveDump = "SaveDump";

        public const string MsgBox = "MessageBox";

        private BitmapImage InkCanvastoBitmapImage(InkCanvas inkCanvas)
        {
            BitmapImage bitmapImage = new BitmapImage();
            int width = (int) inkCanvas.ActualWidth;
            int height = (int) inkCanvas.ActualHeight;

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(inkCanvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            rtb.Render(dv);

            var encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (MemoryStream memoryStream = new())
            {
                encoder.Save(memoryStream);
                memoryStream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void SaveBitmapImage(BitmapImage bitmapImage, string filePath)
        {
            BitmapEncoder encoder;
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpeg":
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".tiff":
                case ".tif":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported file extension: {extension}");
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private void OpenFolderAndSelectFile(string fileFullName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileFullName;
            Process.Start(psi);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
            Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
            Viewbox = Template.FindName(NamePartViewBox, this) as Viewbox;
            InkCanvas = Template.FindName(NamePartInkCanvas, this) as InkCanvas;

            OnLoad();
        }

        #region ImageSourceProperty

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageEx), new PropertyMetadata(null, (o, p) =>
            {
                //ImageSource变化时回调，平铺对象
                //o 是更改了属性的对象，p 是一个包含旧值和新值的 DependencyPropertyChangedEventArgs 对象

                if (o is not ImageEx ex) return;
                ex.UpdateImageInfo();

                if (p.OldValue is not BitmapSource s1)
                {
                    ex.TileImage();
                    return;
                }

                if (p.NewValue is not BitmapSource s2)
                    return;

                //差异超过0.001，则触发
                if (Math.Abs(s1.Width - s2.Width) > 0.001
                    || Math.Abs(s1.Height - s2.Height) > 0.001) ex.TileImage();

            }));

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        #endregion

        #region 处理缩放&拖拽

        private Point _cursor = new(-1, -1);

        private void OnZoomChanged(object sender, MouseWheelEventArgs e)
        {
            var oldScale = ImagePanelScale;
            if (oldScale <= 0) return;

            var scale = Math.Log10(ImagePanelScale / DefaultImagePanelScale);

            scale = (scale <= 0 ? 0.1 : Math.Pow(10, Math.Floor(scale))) * DefaultImagePanelScale;

            //e.Delta 小于或等于 0，表示滚轮向下滚动，进行缩小操作
            if (e.Delta <= 0)
            {
                ImagePanelScale -= DefaultImagePanelScale * scale;

                // 最小为缩小10倍
                if (ImagePanelScale <= DefaultImagePanelScale * 0.1)
                    ImagePanelScale = DefaultImagePanelScale * 0.1;
            }
            else
            {
                ImagePanelScale += DefaultImagePanelScale * scale;
            }
            if (ImagePanelScale <= DefaultImagePanelScale) return;

            var transform = ImagePanelScale / oldScale;
            var pos = e.GetPosition(Viewbox);
            var target = new Point(pos.X * transform, pos.Y * transform);
            var offset = target - pos;

            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset + offset.X);
            Scroll!.ScrollToVerticalOffset(Scroll.VerticalOffset + offset.Y);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //PreviewMouseDown-常用于需要拦截事件的场景，比如实现自定义行为、阻止某些默认行为（例如在某些条件下阻止进一步的事件传递）等
            //MouseDown-通常用于处理实际的鼠标点击事件，例如启动某个操作或更新 UI 状态。这个事件通常在确认用户已经点击了目标元素后处理

            _cursor = e.GetPosition(this);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _cursor = new(-1, -1);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_cursor.X < 0 || _cursor.Y < 0) return;

            var pos = e.GetPosition(this);
            var offset = pos - _cursor;
            _cursor = pos;

            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset - offset.X);
            Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset - offset.Y);
        }

        private void OnLoad()
        {
            MainPanel!.MouseWheel += OnZoomChanged;
            Scroll!.MouseMove += OnMouseMove;
            Scroll.PreviewMouseDown += OnPreviewMouseDown; ;
            Scroll.PreviewMouseUp += OnPreviewMouseUp;
        }

        private void OnUnload()
        {
            MainPanel!.MouseWheel -= OnZoomChanged;
            Scroll!.MouseMove -= OnMouseMove;
            Scroll.PreviewMouseDown -= OnPreviewMouseDown; ;
            Scroll.PreviewMouseUp -= OnPreviewMouseUp;
        }

        ~ImageEx() => OnUnload();


        #endregion

        #region Name_Part

        private Panel? MainPanel;

        private ScrollViewer? Scroll;

        private Viewbox? Viewbox;

        private InkCanvas? InkCanvas;

        public const string NamePartMainPanel = "PART_MAIN_PANEL";

        public const string NamePartScrollView = "PART_SCROLL";

        public const string NamePartViewBox = "PART_VIEWBOX";

        public const string NamePartInkCanvas = "PART_INKCANVAS";

        public const string NamePartImage = "PART_IMAGEMAIN";

        #endregion

        #region Render Size Info

        double DefaultImagePanelScale = 0;
        (double Width, double Height) DefaultImageSize;

        public static readonly DependencyProperty ImagePanelScaleProperty = DependencyProperty.Register(
            nameof(ImagePanelScale), typeof(double), typeof(ImageEx), new PropertyMetadata((double) -1, OnImagePanelScale));

        private static void OnImagePanelScale(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ImageEx ex || e.NewValue is not double n || n <= 0) return;
            if (ex.Viewbox is null) return;

            ex.Viewbox.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;
            ex.Viewbox.Width = ex.DefaultImageSize.Width * ex.ImagePanelScale;
        }

        [Browsable(true)]
        [Category("SizeInfo")]
        [ReadOnly(true)]
        public double ImagePanelScale
        {
            get => (double) GetValue(ImagePanelScaleProperty);
            set => SetValue(ImagePanelScaleProperty, value);
        }

        private void TileImage()
        {
            ImagePanelScale = DefaultImagePanelScale;

            if (Scroll is null) return;

            Scroll.ScrollToVerticalOffset(0);
            Scroll.ScrollToHorizontalOffset(0);
        }

        private void UpdateImageInfo()
        {
            if (ImageSource is null) return;

            DefaultImagePanelScale = Math.Min(ActualWidth / ImageSource.Width,
                ActualHeight / ImageSource.Height);

            DefaultImageSize = (ImageSource.Width, ImageSource.Height);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdateImageInfo();
            TileImage();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            UpdateImageInfo();
        }

        #endregion
    }
}
