using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageExLib.ShapeEx;
using Microsoft.Win32;

namespace ImageExLib
{
    //todo，返回相对image的坐标√
    //返回图形坐标√
    //网格绘制√
    //command设置填充模式，button设置绘制模式√
    //向外传输当前点击坐标、当前网格坐标、绘制信息——实装时考虑
    //contextmenu事件无法正确传递√

    public class ImageEx : ContentControl
    {
        static ImageEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEx), new FrameworkPropertyMetadata(typeof(ImageEx)));
        }

        public ImageEx()
        {
            CommandBindings.Add(new CommandBinding(MarkerShapeCommand, (obj, args) =>
            {
                if (args.Parameter is not string command) return;

                CleanDraw();//清除之前留下的绘制图形

                ShapePreviewer = Template.FindName(GetCurrentShapeType(true, command), this) as ShapeBase;
                ShapeMarker = Template.FindName(GetCurrentShapeType(false, command), this) as ShapeBase;

                isPolygonDrawingRedraw = true;
            }));

            CommandBindings.Add(new CommandBinding(GridFillCommand, (obj, args) =>
            {
                if (args.Parameter is not string command) return;

                if (command == GridFill)
                {
                    if (ShapeMarker is not RectangleShape rectangleShape && ShapeMarker is not PolygonShape polygonShape) return;

                    RefreshFillGridBrush();
                }
            }));

            CommandBindings.Add(new CommandBinding(MarkMenuCommand, (obj, args) =>
            {
                if (args.Parameter is not string command || ShapeMarker is null) return;

                if (command == Delete)
                {
                    if (command == Delete) ShapeMarker.Visibility = Visibility.Collapsed;
                }
                else if (command == ZoomScale)
                {
                    if (ShapeMarker is RectangleShape)
                    {
                        var marker = ShapeMarker;
                        var start = marker.PointStart;
                        var end = marker.PointEnd;

                        var location = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
                        var center = (location.X + marker.Width / 2, location.Y + marker.Height / 2);

                        var width = marker.Width * DefaultImagePanelScale;
                        var height = marker.Height * DefaultImagePanelScale;

                        var scaleX = ActualWidth * 0.9 / width; // zoom the 90% width or height
                        var scaleY = ActualHeight * 0.9 / height;

                        var scale = Math.Min(scaleX, scaleY);
                        ImagePanelScale = scale * DefaultImagePanelScale;

                        var offsetX = center.Item1 * ImagePanelScale - ActualWidth / 2;
                        var offsetY = center.Item2 * ImagePanelScale - ActualHeight / 2;

                        Scroll?.ScrollToHorizontalOffset(offsetX);
                        Scroll?.ScrollToVerticalOffset(offsetY);
                    }
                    else if (ShapeMarker is PolygonShape polygonShape)
                    {
                        var points = polygonShape!.Points;

                        // 1. 计算多边形的边界框 (bounding box)
                        double minX = points.Min(p => p.X);
                        double minY = points.Min(p => p.Y);
                        double maxX = points.Max(p => p.X);
                        double maxY = points.Max(p => p.Y);

                        // 定义外接矩形的宽度和高度
                        double width = maxX - minX;
                        double height = maxY - minY;

                        // 外接矩形的中心点
                        var boundingBoxCenter = new Point(minX + width / 2, minY + height / 2);

                        // 2. 计算适合窗口的缩放比例
                        double scaleX = ActualWidth * 0.9 / (width * DefaultImagePanelScale);
                        double scaleY = ActualHeight * 0.9 / (height * DefaultImagePanelScale);
                        double scale = Math.Min(scaleX, scaleY);

                        // 更新 ImagePanelScale 的缩放比例
                        ImagePanelScale = scale * DefaultImagePanelScale;

                        // 3. 计算偏移量以居中显示
                        double offsetX = boundingBoxCenter.X * ImagePanelScale - ActualWidth / 2;
                        double offsetY = boundingBoxCenter.Y * ImagePanelScale - ActualHeight / 2;

                        // 4. 更新滚动条位置，使得外接矩形居中显示
                        Scroll?.ScrollToHorizontalOffset(offsetX);
                        Scroll?.ScrollToVerticalOffset(offsetY);
                    }
                }
                else throw new NotImplementedException();
            }));

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
                else if (command == MsgBox)
                {
                    MessageBox.Show("MsgBox Show!");
                }
            }));

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
            Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
            Viewbox = Template.FindName(NamePartViewBox, this) as Viewbox;
            InkCanvas = Template.FindName(NamePartInkCanvas, this) as InkCanvas;

            OnViewerLoad();
            OnCanvasLoad();

            RectFillDefaultBrush = (Template.FindName(GetCurrentShapeType(true, "RectMarker"), this) as ShapeBase)!.Fill;
            PolygonFillDefaultBrush = (Template.FindName(GetCurrentShapeType(true, "PolygonMarker"), this) as ShapeBase)!.Fill;
        }

        public (double x, double y) ImageCurrentPosition
        {
            get
            {
                var pos = Mouse.GetPosition(InkCanvas);
                return ((double) pos.X, (double) pos.Y);
            }
        }

        public (int x, int y) GridFillCurrentPos
        {
            get
            {
                try
                {
                    Point _constrastPoint = new();
                    double width = 0;
                    double height = 0;

                    if (ShapeMarker is RectangleShape)
                    {
                        width = ShapeMarker!.Width;
                        height = ShapeMarker!.Height;
                        _constrastPoint = ShapeMarker!.PointStart;
                    }
                    else
                    {
                        return (-1, -1);
                    }

                    var currentPos = Mouse.GetPosition(InkCanvas);

                    double _gridSpacingX = width / XGridCount;
                    double _gridSpacingY = height / YGridCount;

                    if (_gridSpacingX == 0 || _gridSpacingY == 0) return (-1, -1);

                    int y = (int) Math.Floor((currentPos.Y - _constrastPoint.Y) / _gridSpacingY) + 1;
                    int x = (int) Math.Floor((currentPos.X - _constrastPoint.X) / _gridSpacingX) + 1;

                    if ((y > XGridCount || y <= 0) || (x > YGridCount || x <= 0))
                        return (-1, -1);

                    return (x, y);
                }
                catch (Exception)
                {
                    return (-1, -1);
                }
            }
        }

        #region GridFillCommand

        #region 依赖属性设置

        public static readonly DependencyProperty XGridCountProperty =
            DependencyProperty.Register(nameof(XGridCount), typeof(int), typeof(ImageEx), new PropertyMetadata(5));

        public static readonly DependencyProperty YGridCountProperty =
            DependencyProperty.Register(nameof(YGridCount), typeof(int), typeof(ImageEx), new PropertyMetadata(5)); 

        public static readonly DependencyProperty XGridFillCountProperty =
            DependencyProperty.Register(nameof(XGridFillCount), typeof(int), typeof(ImageEx), new PropertyMetadata(0));

        public static readonly DependencyProperty YGridFillCountProperty =
            DependencyProperty.Register(nameof(YGridFillCount), typeof(int), typeof(ImageEx), new PropertyMetadata(1));

        public static readonly DependencyProperty GridLineColorProperty =
            DependencyProperty.Register(nameof(GridLineColor),typeof(Brush),typeof(ImageEx),new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty GridFillColorProperty =
            DependencyProperty.Register(nameof(GridFillColor),typeof(Brush),typeof(ImageEx),new PropertyMetadata(Brushes.LightYellow));

        public static readonly DependencyProperty GridFillThicknessProperty =
            DependencyProperty.Register(nameof(GridFillThickness),typeof(double),typeof(ImageEx),new PropertyMetadata(1.0));

        public static readonly DependencyProperty XDashesProperty =
            DependencyProperty.Register(nameof(SolidDashes),typeof(int),typeof(ImageEx),new PropertyMetadata(30));

        public static readonly DependencyProperty YDashesProperty =
            DependencyProperty.Register(nameof(VirtualDashes),typeof(int),typeof(ImageEx),new PropertyMetadata(30));


        public int XGridCount
        {
            get => (int) GetValue(XGridCountProperty);
            set => SetValue(XGridCountProperty, value);
        }

        public int YGridCount
        {
            get => (int) GetValue(YGridCountProperty);
            set => SetValue(YGridCountProperty, value);
        }

        public int XGridFillCount
        {
            get => (int) GetValue(XGridFillCountProperty);
            set => SetValue(XGridFillCountProperty, value);
        }

        public int YGridFillCount
        {
            get => (int) GetValue(YGridFillCountProperty);
            set => SetValue(YGridFillCountProperty, value);
        }

        public Brush GridLineColor
        {
            get => (Brush) GetValue(GridLineColorProperty);
            set => SetValue(GridLineColorProperty, value);
        }

        public Brush GridFillColor
        {
            get => (Brush) GetValue(GridFillColorProperty);
            set => SetValue(GridFillColorProperty, value);
        }

        public double GridFillThickness
        {
            get => (double) GetValue(GridFillThicknessProperty);
            set => SetValue(GridFillThicknessProperty, value);
        }

        public int SolidDashes
        {
            get => (int) GetValue(XDashesProperty);
            set => SetValue(XDashesProperty, value);
        }

        public int VirtualDashes
        {
            get => (int) GetValue(YDashesProperty);
            set => SetValue(YDashesProperty, value);
        }
        #endregion

        private static Brush? RectFillDefaultBrush;

        private static Brush? PolygonFillDefaultBrush;

        public static readonly RoutedUICommand GridFillCommand = new();

        public const string GridFill = "GridFill";

        /// <summary>
        /// 更新填充位置，刷新填补画面
        /// </summary>
        public void RefreshFillGridBrush()
        {
            int xGridCount = XGridCount;
            int yGridCount = YGridCount;
            int xGridFillCount = XGridFillCount;
            int yGridFillCount = YGridFillCount;

            if (ShapeMarker is RectangleShape)
            {
                double width = ShapeMarker!.Width;
                double height = ShapeMarker!.Height;

                Brush gridLineColor = GridLineColor;//线颜色
                Brush gridFillColor = GridFillColor;//填充颜色
                double gridFillThickness = GridFillThickness;//线宽
                int solidDashes = SolidDashes;//实虚线间断
                int virtualDashes = VirtualDashes;

                VisualBrush gridBrush = new();
                double gridSpaceX = width / xGridCount;
                double gridSpaceY = height / yGridCount;

                if (gridSpaceX > 0 && gridSpaceY > 0)
                {
                    DrawingVisual gridVisual = new();
                    using (DrawingContext dc = gridVisual.RenderOpen())
                    {
                        Pen gridPen = new Pen(gridLineColor, gridFillThickness) { DashStyle = new DashStyle(new double[] { solidDashes, virtualDashes }, 0) };

                        for (int i = 0; i <= xGridCount; i++)
                        {
                            for (int j = 0; j <= yGridCount; j++)
                            {
                                double cellX = i * gridSpaceX;
                                double cellY = j * gridSpaceY;

                                // 绘制填充矩形
                                if (j < yGridFillCount - 1 || (j == yGridFillCount - 1 && i < xGridFillCount))
                                    dc.DrawRectangle(gridFillColor, null, new Rect(cellX, cellY, gridSpaceX, gridSpaceY));

                                // 绘制垂直线，首尾不绘制
                                if (i > 0 && cellX != width)
                                    dc.DrawLine(gridPen, new Point(cellX, 0), new Point(cellX, height));

                                // 绘制水平线，首尾不绘制
                                if (j > 0 && cellY != height)
                                    dc.DrawLine(gridPen, new Point(0, cellY), new Point(width, cellY));
                            }
                        }
                    }
                    gridBrush = new VisualBrush(gridVisual)
                    {
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Left,
                        AlignmentY = AlignmentY.Top,
                        TileMode = TileMode.None
                    };
                }

                ShapeMarker.Fill = gridBrush;
            }
        }

        #endregion

        #region MarkMenuCommand

        public const string Delete = "Delete";

        public const string ZoomScale = "ZoomScale";

        public static readonly RoutedUICommand MarkMenuCommand = new();

        public static readonly DependencyProperty MarkerMenuProperty =
            DependencyProperty.Register(nameof(MarkerMenu), typeof(ContextMenu), typeof(ImageEx), new PropertyMetadata(default(ContextMenu)));

        public ContextMenu MarkerMenu
        {
            get => (ContextMenu) GetValue(MarkerMenuProperty);
            set => SetValue(MarkerMenuProperty, value);
        }

        #endregion

        #region MarkerShapeCommand

        public static readonly RoutedUICommand MarkerShapeCommand = new();

        public const string PointShape = "PointShape";

        public const string RectangleShape = "RectangleShape";

        public const string LineShape = "LineShape";

        public const string PolygonShape = "PolygonShape";

        private ShapeBase? ShapePreviewer;

        private ShapeBase? ShapeMarker;

        private bool isRecDrawing = false;

        private bool isLineDrawing = false;

        private bool isPointDrawing = false;

        private bool isPolygonDrawing = false;//是否绘制

        private bool isPolygonDrawingDone = false;//是否完成一次多边形绘制

        public bool isPolygonDrawingRedraw = false;//重绘多边形

        private void OnCanvasLoad()
        {
            InkCanvas!.PreviewMouseDown += OnCanvasPreviewMouseDown;
            InkCanvas.PreviewMouseMove += OnCanvasPreviewMouseMove;
            InkCanvas!.PreviewMouseUp += OnCanvasPreviewMouseUp;
        }

        private void OnCanvasUnload()
        {
            InkCanvas!.PreviewMouseDown -= OnCanvasPreviewMouseDown;
            InkCanvas!.PreviewMouseUp -= OnCanvasPreviewMouseUp;
            InkCanvas.PreviewMouseMove -= OnCanvasPreviewMouseMove;
        }

        private void OnCanvasPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ShapePreviewer == null || ShapeMarker == null) return;

            if (ShapeMarker is RectangleShape)
            {
                var rectShapePre = ShapePreviewer as RectangleShape;
                if (!rectShapePre!.isRepeat && !isRecDrawing) return;//不重绘

                ShapePreviewer!.Visibility = Visibility.Collapsed;
                InkCanvas!.Cursor = Cursors.Arrow;

                if (!ValidLocation(e)) return;
                var min = Math.Min(ShapePreviewer!.Height, ShapePreviewer!.Width);
                var threshold = Math.Min(ImageSource!.Height, ImageSource!.Width) * 0.02;
                if (threshold > min) return;

                ShapeMarker.Draw(InkCanvas);
                ShapeMarker!.PointStart = ShapePreviewer!.PointStart;
                ShapeMarker!.PointEnd = ShapePreviewer!.PointEnd;
                ShapeMarker!.Visibility = Visibility.Visible;
                ShapeMarker.Refresh();

                ShapePreviewer.Clear(InkCanvas);
                isRecDrawing = false;
            }
            else if (ShapeMarker is LineShape)
            {
                ShapePreviewer!.Visibility = Visibility.Collapsed;
                InkCanvas!.Cursor = Cursors.Arrow;

                if (!ValidLocation(e)) return;
                var min = Math.Min(ShapePreviewer!.Height, ShapePreviewer!.Width);
                var threshold = Math.Min(ImageSource!.Height, ImageSource!.Width) * 0.02;
                if (threshold > min) return;

                ShapeMarker!.PointStart = ShapePreviewer!.PointStart;
                ShapeMarker!.PointEnd = ShapePreviewer!.PointEnd;
                ShapeMarker!.Visibility = Visibility.Visible;
                ShapeMarker.Draw(InkCanvas);

                ShapePreviewer.Clear(InkCanvas);//去除预览绘制
                isLineDrawing = false;
            }
            else if (ShapeMarker is PointShape)
            {
                ShapePreviewer!.Visibility = Visibility.Collapsed;

                if (!ValidLocation(e)) return;

                ShapeMarker!.PointStart = ShapePreviewer!.PointStart;
                ShapeMarker!.Visibility = Visibility.Visible;
                ShapeMarker!.Refresh();
                ShapeMarker.Draw(InkCanvas!);

                ShapePreviewer.Clear(InkCanvas!);
                isPointDrawing = false;
            }
            else if (ShapeMarker is PolygonShape)
            {
                //todo,增加处理，省去第一次弹窗
            }

            RefreshShapeData();//刷新绘制数据
        }

        private void OnCanvasPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ShapePreviewer == null || ShapeMarker == null) return;

            if (ShapeMarker is RectangleShape)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                var rectShapePre = ShapePreviewer as RectangleShape;
                if (rectShapePre!.isRepeat)
                {
                    //重绘
                    if (isRecDrawing)
                    {
                        InkCanvas!.Cursor = Cursors.Cross;
                        ShapePreviewer!.Visibility = Visibility.Visible;
                        ShapePreviewer!.PointEnd = e.GetPosition(InkCanvas);
                        ShapePreviewer.Refresh();
                    }
                }
                else
                {
                    //不重绘
                    isRecDrawing = true;
                    ShapeMarker.Fill = RectFillDefaultBrush;
                    ShapeMarker!.Visibility = Visibility.Collapsed;
                    InkCanvas!.Cursor = Cursors.Cross;

                    ShapePreviewer!.Visibility = Visibility.Visible;
                    ShapePreviewer!.PointEnd = e.GetPosition(InkCanvas);
                    ShapePreviewer.Refresh();
                    ShapePreviewer.Draw(InkCanvas);
                }
            }
            else if (ShapeMarker is LineShape)
            {
                if (isLineDrawing)
                {
                    InkCanvas!.Cursor = Cursors.Cross;
                    ShapePreviewer.Draw(InkCanvas, true);
                    ShapePreviewer!.PointEnd = e.GetPosition(InkCanvas);
                    ShapePreviewer!.Visibility = Visibility.Visible;
                    ShapePreviewer?.Refresh();
                }
            }
            else if (ShapeMarker is PointShape)
            {
                if (isPointDrawing)
                {
                    ShapePreviewer.Draw(InkCanvas!, true);
                    ShapePreviewer!.PointStart = e.GetPosition(InkCanvas);
                }
            }
            else if (ShapeMarker is PolygonShape)
            {
                var polygonShape = ShapePreviewer as PolygonShape;
                if (polygonShape?.Points?.Count == 0) return;

                var point = e.GetPosition(InkCanvas);
                if (isPolygonDrawing)
                {
                    if (ValidPoint(point, polygonShape!.Points)) polygonShape!.Points.Add(point);
                    isPolygonDrawing = false;
                }
                else
                {
                    polygonShape!.Points[polygonShape!.Points.Count - 1] = point;
                }
                ShapePreviewer.Draw(InkCanvas!);
            }
        }

        private void OnCanvasPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ShapePreviewer == null || ShapeMarker == null) return;

            if (ShapeMarker is RectangleShape)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                var rectShapePre = ShapePreviewer as RectangleShape;
                if (rectShapePre!.isRepeat)
                {
                    //重绘
                    ShapeMarker.Fill = RectFillDefaultBrush;
                    ShapePreviewer.Draw(InkCanvas!);
                    ShapePreviewer.PointStart = e.GetPosition(InkCanvas);
                    isRecDrawing = true;
                }
                else
                {
                    //不重绘
                    ShapePreviewer.PointStart = e.GetPosition(InkCanvas);
                    ShapePreviewer.Draw(InkCanvas!);
                }
            }
            else if (ShapeMarker is LineShape)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;
                ShapePreviewer.PointStart = e.GetPosition(InkCanvas);
                isLineDrawing = true;
            }
            else if (ShapeMarker is PointShape)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                isPointDrawing = true;
                ShapePreviewer.Draw(InkCanvas!, true);
                ShapePreviewer!.PointStart = e.GetPosition(InkCanvas);
                ShapePreviewer!.Visibility = Visibility.Visible;
                ShapePreviewer!.Refresh();
            }
            else if (ShapeMarker is PolygonShape)
            {
                //todo，后续添加多边形网格&网格坐标
                ////网格-每次都初始化fill的设置
                //ShapeMarker.Fill = PolygonFillDefaultBrush;

                if (e.RightButton != MouseButtonState.Pressed) return;

                if (!isPolygonDrawingRedraw) return;
                var polygonShapePre = ShapePreviewer as PolygonShape;

                if (isPolygonDrawingDone)
                {
                    polygonShapePre!.Points.Clear();
                    ShapeMarker!.Visibility = Visibility.Collapsed;
                    ShapePreviewer!.Visibility = Visibility.Visible;
                }

                polygonShapePre!.Points.Add(e.GetPosition(InkCanvas));
                ShapePreviewer.Draw(InkCanvas!);
                isPolygonDrawing = true;
                isPolygonDrawingDone = false;

                if (e.ClickCount == 2)
                {
                    ShapePreviewer!.Visibility = Visibility.Collapsed;
                    ShapeMarker!.Visibility = Visibility.Visible;
                    
                    var sortsPoints = DisposePointsToFormPolygon(polygonShapePre!.Points);
                    if (sortsPoints.Count < 3)
                    {
                        polygonShapePre!.Points.Clear();
                        ShapeMarker!.Visibility = Visibility.Collapsed;
                        isPolygonDrawingDone = false;
                    }

                    var polygonShapeMarker = ShapeMarker as PolygonShape;
                    polygonShapeMarker!.Points = sortsPoints;
                    ShapeMarker?.Draw(InkCanvas!);

                    isPolygonDrawingDone = true;
                    isPolygonDrawingRedraw = false;
                }   
            }
        }

        private  List<Point> DisposePointsToFormPolygon(List<Point> points)
        {
            if (points == null || points.Count < 3)
            {
                throw new ArgumentException("多边形必须至少有三个点。");
            }

            // 去除重复点
            var distinctPoints = points.DistinctBy(p => new { p.X, p.Y }).ToList();

            //排除距离相差小于5个像素的点
            var judge = 5;
            var filteredPoints = new List<Point>();
            filteredPoints.Add(distinctPoints[0]);
            for (var i = 1; i < distinctPoints.Count; i++)
            {
                var currentPoint = distinctPoints[i];
                bool isFarEnough = true;

                foreach (var existingPoint in filteredPoints)
                {
                    double dx = currentPoint.X - existingPoint.X;
                    double dy = currentPoint.Y - existingPoint.Y;
                    if (Math.Sqrt(dx * dx + dy * dy) < judge)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                    filteredPoints.Add(currentPoint);
            }

            // 找到最低且最左的点作为参考点
            var referencePoint = filteredPoints.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            // 按照与参考点的极角排序
            var sortedPoints = filteredPoints.OrderBy(p => Math.Atan2(p.Y - referencePoint.Y, p.X - referencePoint.X)).ToList();

            return sortedPoints;
        }

        private bool ValidPoint(Point point, List<Point> points)
        {
            double threshold = 0.01;
            if (points.Count <= 1) return true;
            foreach (var existingPoint in points)
            {
                var value = Distance(point, existingPoint);
                if (value < threshold) return false;
            }
            return true;
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void CleanDraw()
        {
            for (int i = InkCanvas!.Children.Count - 1; i >= 0; i--)
            {
                if (InkCanvas.Children[i] is RectangleShape ||
                    InkCanvas.Children[i] is LineShape ||
                    InkCanvas.Children[i] is PointShape ||
                    InkCanvas.Children[i] is PolygonShape)
                {
                    InkCanvas.Children.RemoveAt(i);               
                }
            }
        }

        private void RefreshShapeData()
        {
            List<string> shapesInfo = new List<string>();
            foreach (var child in InkCanvas!.Children)
            {
                if (ShapeMarker is LineShape && child is LineShape line)
                {
                    shapesInfo.Add($"Line: Start({line.PointStart.X.ToString("0.00")}, {line.PointStart.Y.ToString("0.00")}), End({line.PointEnd.X.ToString("0.00")}, {line.PointEnd.Y.ToString("0.00")})");
                }
                else if (ShapeMarker is RectangleShape && child is RectangleShape rectangle)
                {
                    double left = InkCanvas.GetLeft(rectangle);
                    double top = InkCanvas.GetTop(rectangle);
                    shapesInfo.Add($"Rectangle: Position({left.ToString("0.00")}, {top.ToString("0.00")}), Size({rectangle.Width.ToString("0.00")}, {rectangle.Height.ToString("0.00")})");
                }
                else if (ShapeMarker is PointShape && child is PointShape ellipse)
                {
                    shapesInfo.Add($"PointShape: Position({ellipse.PointStart.X.ToString("0.00")}, " +
                        $"{ellipse.PointStart.Y.ToString("0.00")}), Size({ellipse.RadiusX.ToString("0.00")}, {ellipse.RadiusY.ToString("0.00")})");
                }
                else if (ShapeMarker is PolygonShape && child is PolygonShape polygon && isPolygonDrawingDone)
                {
                    //var polygonPoints = DisposePointsToFormPolygon(polygon!.Points);
                    string points = string.Join(",# ", polygon!.Points.Select(p => $"{p.X:F2}, {p.Y:F2}"));
                    shapesInfo.Add($"Polygon: Points({points})");
                }
            }
            Debug.WriteLine(string.Join("\r\n", shapesInfo), "Shapes Info");//信息输出demo
        }

        private bool ValidLocation(MouseEventArgs e)
        {
            var pos = e.GetPosition(InkCanvas);
            var width = InkCanvas!.ActualWidth;
            var height = InkCanvas!.ActualHeight;
            return !(pos.X < 0 || pos.Y < 0 || pos.X > width || pos.Y > height);
        }

        private static string GetCurrentShapeType(bool isPreviewer, string command)
        {
            string name = string.Empty;

            switch (command)
            {
                case "PointMarker":
                    name = isPreviewer ? "PART_SHAPE_POINT_PREVIEWER" : "PART_SHAPE_POINT_MARKER";
                    break;
                case "LineMarker":
                    name = isPreviewer ? "PART_SHAPE_LINE_PREVIEWER" : "PART_SHAPE_LINE_MARKER";
                    break;
                case "RectMarker":
                    name = isPreviewer ? "PART_SHAPE_RECT_PREVIEWER" : "PART_SHAPE_RECT_MARKER";
                    break;
                case "PolygonMarker":
                    name = isPreviewer ? "PART_SHAPE_POLYGON_PREVIEWER" : "PART_SHAPE_POLYGON_MARKER";
                    break;
                default:
                    throw new NotImplementedException("Not Valid Command!");
            }
            return name;
        }

        #endregion

        #region ImageProcessCommand

        public static readonly RoutedUICommand ImageProcessCommand = new();

        public const string SaveDump = "SaveDump";

        public const string MsgBox = "MsgBox";

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

        #region 处理缩放&拖拽

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

        private void OnViewerLoad()
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

        ~ImageEx()
        {
            OnUnload();
            OnCanvasUnload();
        }

        #endregion

    }
}
