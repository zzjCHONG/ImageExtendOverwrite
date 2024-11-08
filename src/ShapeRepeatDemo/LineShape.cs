using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace ShapeRepeatDemo
{
    public class LineShape:Shape
    {
        public Point PointStart { get; set; }
        public Point PointEnd { get; set; }
        protected override Geometry DefiningGeometry => new LineGeometry(PointStart,PointEnd);

        public void Draw(InkCanvas canvas)
        {
            canvas.Children.Add(this);
        }

        public void Refresh()
        {
            //var start = PointStart;
            //var end = PointEnd;

            //this.PointEnd = end;
            //this.PointStart = start;

            //InkCanvas.SetLeft(this, end.X - start.X);
            //InkCanvas.SetTop(this, end.Y - start.Y);
        }
    }
}
