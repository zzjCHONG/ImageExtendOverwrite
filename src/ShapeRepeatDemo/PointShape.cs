using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeRepeatDemo
{
    public class PointShape : Shape
    {
        public Point Point { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        protected override Geometry DefiningGeometry => new EllipseGeometry(Point, RadiusX, RadiusY);

        public void Draw(InkCanvas canvas)
        {
            canvas.Children.Add(this);
        }

        public void Refresh()
        {
            RadiusX = 50;
            RadiusY = 50;
            //InkCanvas.SetLeft(this, 200);
            //InkCanvas.SetTop(this, 200);
        }
    }
}
