using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class PolygonShape : Shape
{
    public PolygonShape()
    {
        this.Stroke = Brushes.Green;
        this.StrokeThickness = 2;
        this.Fill = Brushes.Transparent;
    }

    // 存储多边形的点集合
    public PointCollection Points { get; set; } = new PointCollection();

    protected override Geometry DefiningGeometry => new PathGeometry(new[] { new PathFigure(Points[0], Points.Skip(1).Select(p => new LineSegment(p, true)).ToArray(), true) });

    public void Draw(InkCanvas canvas)
    {
        canvas.Children.Add(this);
    }

    public void Refresh()
    {

    }
}
