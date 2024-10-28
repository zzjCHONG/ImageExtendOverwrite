using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp2
{
    public class CirclePanel : Panel
    {
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(CirclePanel),
                new FrameworkPropertyMetadata(170.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Diameter
        {
            get => (double) GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public static readonly DependencyProperty KeepVerticalProperty =
          DependencyProperty.Register(nameof(KeepVertical), typeof(bool), typeof(CirclePanel),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool KeepVertical
        {
            get => (bool) GetValue(KeepVerticalProperty);
            set => SetValue(KeepVerticalProperty, value);
        }

        public static readonly DependencyProperty OffsetAngleProperty =
            DependencyProperty.Register(nameof(OffsetAngle), typeof(double), typeof(CirclePanel),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double OffsetAngle
        {
            get => (double) GetValue(OffsetAngleProperty);
            set => SetValue(OffsetAngleProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //return base.MeasureOverride(availableSize);

            var diameter = Diameter;
            var newSize = new Size(diameter, diameter);

            foreach (UIElement element in Children)
            {
                element.Measure(newSize);
            }

            return newSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //return base.ArrangeOverride(finalSize);

            var keepVertical = KeepVertical;
            var offsetAngle = OffsetAngle;
            var radius = Diameter / 2;

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = Children[i];
                double angle = 360.0 / Children.Count * i + offsetAngle;
                double r = Math.PI * angle / 180.0;
                double x = radius * Math.Cos(r);
                double y = radius * Math.Sin(r);
                double centerX = element.DesiredSize.Width / 2;
                double centerY = element.DesiredSize.Height / 2;

                element.RenderTransform = new RotateTransform
                {
                    CenterX = centerX,
                    CenterY = centerY,
                    Angle = keepVertical ? 0 : angle
                };

                double rectX = x + finalSize.Width / 2 - centerX;
                double rectY = y + finalSize.Height / 2 - centerY;

                element.Arrange(new Rect(rectX, rectY, element.DesiredSize.Width, element.DesiredSize.Height));
            }

            return finalSize;
        }
    }
}
