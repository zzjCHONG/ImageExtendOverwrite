using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageExLib.ShapeEx
{
    public abstract class ShapeBase : Shape
    {
        public Point PointStart { get; set; }

        public Point PointEnd { get; set; }

        public virtual bool isRepeat { get; set; } = true;

        protected override Geometry DefiningGeometry => throw new NotImplementedException();

        public void Clear(InkCanvas canvas)
        {
            canvas.Children.Remove(this);
        }

        public virtual void Refresh()
        {
            throw new NotImplementedException();
        }

        public virtual void Draw(InkCanvas canvas, bool isPreShape=false)
        {
            throw new NotImplementedException();
        }

    }

    public class RectangleShape :ShapeBase
    {
        public override bool isRepeat { get; set; } = false;

        protected override Geometry DefiningGeometry => new RectangleGeometry(new Rect(0, 0, Width, Height));

        public override void Refresh()
        {
            //Width,Height,SetLeft,SetTop
            var start = PointStart;
            var end = PointEnd;
            Width = Math.Abs(start.X - end.X);
            Height = Math.Abs(start.Y - end.Y);
            var position = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            InkCanvas.SetLeft(this, position.X);
            InkCanvas.SetTop(this, position.Y);
        }

        public override void Draw(InkCanvas canvas, bool isPreShape)
        {
            if (!isRepeat)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }
            else
            {               
                var ShapeMarkerStyle = FindName("PART_SHAPE_RECT_MARKER") as RectangleShape;

                if (ShapeMarkerStyle != null)
                {
                    var newRectangle = new RectangleShape
                    {
                        PointStart = this.PointStart,
                        PointEnd = this.PointEnd,
                        isRepeat = this.isRepeat,
                        Fill = ShapeMarkerStyle!.Fill,
                        Stroke = ShapeMarkerStyle.Stroke,
                        Opacity = ShapeMarkerStyle.Opacity,
                        StrokeThickness = ShapeMarkerStyle.StrokeThickness
                    };
                    newRectangle.Refresh();
                    canvas.Children.Add(newRectangle);
                }
                else
                {
                    canvas.Children.Add(this);
                }
            }
        }
    }

    public class LineShape : ShapeBase
    {
        public override bool isRepeat { get; set; } = false;

        protected override Geometry DefiningGeometry => new LineGeometry(PointStart, PointEnd);

        public override void Refresh()
        {
            //设置偏移
            //InkCanvas.SetLeft(this, (PointEnd.X - PointStart.X)*0.01);
            //InkCanvas.SetTop(this, (PointEnd.Y - PointStart.Y)*0.01);
        }

        public override void Draw(InkCanvas canvas, bool isPreShape)
        {
            if (!isRepeat|| isPreShape)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }
            else
            {
                var ShapeMarkerStyle = FindName("PART_SHAPE_LINE_MARKER") as LineShape;
                if(ShapeMarkerStyle != null)
                {
                    var newLine = new LineShape
                    {
                        PointStart = this.PointStart,
                        PointEnd = this.PointEnd,
                        isRepeat = this.isRepeat,
                        Fill = ShapeMarkerStyle!.Fill,
                        Stroke = ShapeMarkerStyle.Stroke,
                        Opacity = ShapeMarkerStyle.Opacity,
                        StrokeThickness = ShapeMarkerStyle.StrokeThickness
                    };
                    newLine.Refresh();
                    canvas.Children.Add(newLine);
                }
                else
                {
                    canvas.Children.Add(this);
                }
            }
        }
    }

    public class PointShape : ShapeBase
    {
        public override bool isRepeat { get; set; } = true;

        public double RadiusX { get; set; }
        public double RadiusY { get; set; } 

        protected override Geometry DefiningGeometry => new EllipseGeometry( PointStart, RadiusX, RadiusY);

        public override void Refresh()
        {
            RadiusX = 5;
            RadiusY = 5;
        }

        public override void Draw(InkCanvas canvas,bool isPreShape)
        {
            if (!isRepeat || isPreShape)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }
            else
            {
                var ShapeMarkerStyle = FindName("PART_SHAPE_POINT_MARKER") as PointShape;
                if (ShapeMarkerStyle != null)
                {
                    var newPoint = new PointShape
                    {
                        PointStart = this.PointStart,
                        isRepeat = this.isRepeat,

                        Fill = ShapeMarkerStyle!.Fill,
                        Stroke = ShapeMarkerStyle.Stroke,
                        Opacity = ShapeMarkerStyle.Opacity,
                        StrokeThickness = ShapeMarkerStyle.StrokeThickness
                    };
                    newPoint.Refresh();
                    canvas.Children.Add(newPoint);
                }
                else
                {
                    canvas.Children.Add(this);
                } 
            }
        }
    }

    public class PolygonShape : ShapeBase
    {
        public override bool isRepeat { get; set; } = false;//暂不考虑实现重复绘制

        public List<Point> Points { get; set; } = new();

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (Points.Count < 2)
                    return Geometry.Empty;

                return new PathGeometry(new[] { new PathFigure(Points[0], Points.Skip(1).Select(p => new LineSegment(p, true)).ToArray(), true) });
            }
        }

        public override void Refresh()
        {

        }

        public override void Draw(InkCanvas canvas, bool isPreShape = false)
        {
            if (!isRepeat)
            {
                canvas.Children.Remove(this);
                canvas.Children.Add(this);
            }
            else
            {
                var ShapeMarkerStyle = FindName("PART_SHAPE_POLYGON_MARKER") as RectangleShape;
                var newRectangle = new RectangleShape
                {
                    PointStart = this.PointStart,
                    PointEnd = this.PointEnd,
                    isRepeat = this.isRepeat,
                    Fill = ShapeMarkerStyle!.Fill,
                    Stroke = ShapeMarkerStyle.Stroke,
                    Opacity = ShapeMarkerStyle.Opacity,
                    StrokeThickness = ShapeMarkerStyle.StrokeThickness
                };
                newRectangle.Refresh();

                canvas.Children.Add(newRectangle);
            }
        }
    }
}
