using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ShapeLib_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int type = 0;
        bool isRepeat = true;
        private Point _startPoint;

        private Rectangle _rectangle;
        private Ellipse _ellipse;
        private Line _line;
        private bool _isDrawing;
        private List<Point> points = new List<Point>();
        private Polygon currentPolygon;

        public MainWindow()
        {
            InitializeComponent();

            AddShapes();
        }

        private void AddShapes()
        {
            // 添加线
            Line line = new Line
            {
                X1 = 10, Y1 = 10,
                X2 = 100, Y2 = 100,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            inkCanvas.Children.Add(line);

            // 添加矩形
            Rectangle rectangle = new Rectangle
            {
                Width = 100,
                Height = 50,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black
            };
            InkCanvas.SetLeft(rectangle, 120);
            InkCanvas.SetTop(rectangle, 30);
            inkCanvas.Children.Add(rectangle);

            // 添加椭圆
            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Stroke = Brushes.Black
            };
            InkCanvas.SetLeft(ellipse, 60);
            InkCanvas.SetTop(ellipse, 60);
            inkCanvas.Children.Add(ellipse);

            // 添加多边形
            Polygon polygon = new Polygon
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };
            polygon.Points.Add(new Point(300, 150));
            polygon.Points.Add(new Point(350, 200));
            polygon.Points.Add(new Point(250, 200));
            inkCanvas.Children.Add(polygon);
        }

        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (type == 0)
            {
                if (_isDrawing)
                {
                    Point currentPoint = e.GetPosition(inkCanvas);
                    double width = currentPoint.X - _startPoint.X;
                    double height = currentPoint.Y - _startPoint.Y;

                    _rectangle!.Width = Math.Abs(width);
                    _rectangle.Height = Math.Abs(height);

                    //重设矩形起点
                    InkCanvas.SetLeft(_rectangle, width < 0 ? currentPoint.X : _startPoint.X);
                    InkCanvas.SetTop(_rectangle, height < 0 ? currentPoint.Y : _startPoint.Y);
                }
            }
            else if (type == 1)
            {
                if (_isDrawing)
                {
                    Point endPoint = e.GetPosition(inkCanvas);
                    _line.X1 = _startPoint.X;
                    _line.Y1 = _startPoint.Y;
                    _line.X2 = endPoint.X;
                    _line.Y2 = endPoint.Y;
                }
            }
            else if (type == 3)
            {
                if (currentPolygon != null && points.Count > 0)
                {
                    Point endPoint = e.GetPosition(inkCanvas);
                    if (points.Count > 1)
                    {
                        currentPolygon.Points[points.Count - 1] = endPoint;
                    }
                }
            }
        }

        private void inkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (type == 0)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (_rectangle != null && !isRepeat)
                        inkCanvas.Children.Remove(_rectangle);           
                    _rectangle = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Fill = Brushes.Transparent,
                        StrokeThickness = 2
                    };

                    inkCanvas.Children.Add(_rectangle);

                    _isDrawing = true;
                    _startPoint = e.GetPosition(inkCanvas);
                }
            }

            else if (type == 1)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (_line != null && !isRepeat)
                        inkCanvas.Children.Remove(_line);

                    _startPoint = e.GetPosition(inkCanvas);

                    _line = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    inkCanvas.Children.Add(_line);

                    _isDrawing = true;
                }
            }
            else if (type == 2)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (_ellipse != null && !isRepeat)
                        inkCanvas.Children.Remove(_ellipse);

                    _startPoint = e.GetPosition(inkCanvas);

                    _ellipse = new Ellipse
                    {
                        Fill = Brushes.Black,
                        Width = 5, // 点的直径
                        Height = 5
                    };

                    // 设置点的位置
                    InkCanvas.SetLeft(_ellipse, _startPoint.X - _ellipse.Width / 2);
                    InkCanvas.SetTop(_ellipse, _startPoint.Y - _ellipse.Height / 2);
                    inkCanvas.Children.Add(_ellipse);
                }
            }
            else if (type == 3)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // 右键停止绘制
                    currentPolygon = null; // 清空当前多边形
                    points.Clear(); // 清空点列表

                    if (currentPolygon != null && !isRepeat)
                        inkCanvas.Children.Remove(currentPolygon);

                    return;
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    Point newPoint = e.GetPosition(inkCanvas);
                    points.Add(newPoint);

                    if (currentPolygon == null)
                    {
                        currentPolygon = new Polygon
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        inkCanvas.Children.Add(currentPolygon);
                    }

                    currentPolygon.Points.Clear();
                    foreach (var point in points)
                    {
                        currentPolygon.Points.Add(point);
                    }
                    // 如果至少有两个点，添加最后一个点以形成闭合形状
                    if (points.Count > 1)
                    {
                        currentPolygon.Points.Add(points[0]); // 形成闭合形状
                    }
                }
            }
        }

        private void inkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) return;

            if (type == 0)
            {
                _isDrawing = false;

                Rect bounds = VisualTreeHelper.GetDescendantBounds(_rectangle);//获取矩形的边界框
                Point topLeft = _rectangle!.TranslatePoint(new Point(0, 0), inkCanvas);//矩形的左上角坐标从矩形自身的坐标系转换到 InkCanvas 的坐标系

                Console.WriteLine($"Left Offset: {topLeft.X}, Top Offset: {topLeft.Y} " +
                    $"Width:{_rectangle!.Width} Height:{_rectangle.Height}");
            }
            else if (type == 1)
            {
                _isDrawing = false;
                Console.WriteLine($"({_line.X1},{_line.Y1})  ({_line.X2},{_line.Y2})");
            }
            else if (type == 2)
            {
                Console.WriteLine($"{inkCanvas.Children}");
            }
        }

        private void Tran_Click(object sender, RoutedEventArgs e)
        {
            if (type == 0)
            {
                type = 1;
                return;
            }
            if (type == 1)
            {
                type = 2;
                return;
            }
            if (type == 2)
            {
                type = 3;
                return;
            }
            if (type == 3)
            {
                type = 0;
                return;
            }
        }

        private void ChildrenInfoShow_Click(object sender, RoutedEventArgs e)
        {
            List<string> shapesInfo = new List<string>();

            foreach (var child in inkCanvas.Children)
            {
                if (child is Line line)
                {
                    shapesInfo.Add($"Line: Start({line.X1.ToString("0.00")}, {line.Y1.ToString("0.00")}), End({line.X2.ToString("0.00")}, {line.Y2.ToString("0.00")})");
                }
                else if (child is Rectangle rectangle)
                {
                    double left = InkCanvas.GetLeft(rectangle);
                    double top = InkCanvas.GetTop(rectangle);
                    shapesInfo.Add($"Rectangle: Position({left.ToString("0.00")}, {top.ToString("0.00")}), Size({rectangle.Width.ToString("0.00")}, {rectangle.Height.ToString("0.00")})");
                }
                else if (child is Ellipse ellipse)
                {
                    double left = InkCanvas.GetLeft(ellipse);
                    double top = InkCanvas.GetTop(ellipse);
                    shapesInfo.Add($"Ellipse: Position({left.ToString("0.00")}, {top.ToString("0.00")}), Size({ellipse.Width.ToString("0.00")}, {ellipse.Height.ToString("0.00")})");
                }
                else if (child is Polygon polygon)
                {
                    string points = string.Join(", ", polygon.Points);
                    shapesInfo.Add($"Polygon: Points({points})");
                }

            }

            // 显示图形信息
            MessageBox.Show(string.Join("\r\n", shapesInfo), "Shapes Info");
   
        }
    }
}
