using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeRepeatDemo
{
    public class RectangleShape : Shape
    {
        public Point PointStart { get; set; }
        public Point PointEnd { get; set; }
        protected override Geometry DefiningGeometry => new RectangleGeometry(new Rect(0, 0, Width, Height));

        public void Draw(InkCanvas canvas)
        {
            // 将矩形添加到 InkCanvas
            canvas.Children.Add(this);
        }

        public void Refresh()
        {
            // 计算矩形的宽度和高度
            var width = Math.Abs(PointStart.X - PointEnd.X);
            var height = Math.Abs(PointStart.Y - PointEnd.Y);
            Width = width;
            Height = height;

            // 设置矩形的位置
            var position = new Point(Math.Min(PointStart.X, PointEnd.X), Math.Min(PointStart.Y, PointEnd.Y));
            InkCanvas.SetLeft(this, position.X);
            InkCanvas.SetTop(this, position.Y);
        }
    }
}
