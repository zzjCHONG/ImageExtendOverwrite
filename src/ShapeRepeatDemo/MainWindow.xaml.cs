using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShapeRepeatDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //AddShapes();
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
                Width = 80,
                Height = 50,
                Fill = Brushes.Red,
                Stroke = Brushes.Black
            };
            InkCanvas.SetLeft(ellipse, 250);
            InkCanvas.SetTop(ellipse, 30);
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

        private Point startPoint;
        private RectangleShape? currentRectangle;

        private static int times = 0;

        private void OnDrawRectangleClick(object sender, RoutedEventArgs e)
        {
            times++;

            // 设置起始点和终止点
            var pointStart = new Point(0 + times * 50,0 + times * 50);
            var pointEnd = new Point(150 + times * 50, 150 + times * 50);

            // 创建新的矩形实例
            var rectangle = new RectangleShape
            {
                PointStart = pointStart,
                PointEnd = pointEnd,
                Fill = Brushes.Red // 设置填充颜色
            };

  
            rectangle.Refresh();
            rectangle.Draw(inkCanvas);
        }

        private void AddShapes_Click(object sender, RoutedEventArgs e)
        {
            AddShapes();
        }


        private void Line_Click(object sender, RoutedEventArgs e)
        {
            times++;
            int step = 0;

            // 设置起始点和终止点
            var pointStart = new Point(0 + times * step, 0 + times * step);
            var pointEnd = new Point(100 + times * step, 100 + times * step);

            var line = new LineShape
            {
                PointEnd = pointEnd,
                PointStart = pointStart,

                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Debug.WriteLine($"{pointStart.X}，{pointStart.Y}__{pointEnd.X}，{pointEnd.Y}");

            line.Refresh();
            line.Draw(inkCanvas);
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            //Ellipse ellipse = new Ellipse
            //{
            //    Width = 10,
            //    Height = 10,
            //    Fill = Brushes.Red,
            //    Stroke = Brushes.Black
            //};
            //InkCanvas.SetLeft(ellipse, 200);
            //InkCanvas.SetTop(ellipse, 200);
            //inkCanvas.Children.Add(ellipse);

            var pointStart = new Point(0, 0);
            var ell = new PointShape
            {
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                Point = pointStart,
            };

            ell.Refresh();
            ell.Draw(inkCanvas);

        }

        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            var polygonShape = new PolygonShape();

            polygonShape. Points.Add(new Point(10, 50));
            polygonShape.Points.Add(new Point(100, 10));
            polygonShape.Points.Add(new Point(50, 100));

            polygonShape.Refresh();

            // 绘制到 InkCanvas 上
            polygonShape.Draw(inkCanvas);
        }
    }
}
