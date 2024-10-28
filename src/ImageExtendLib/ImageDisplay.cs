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

namespace ImageExtendLib
{

    public class ImageDisplay : Control
    {
        static ImageDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageDisplay), new FrameworkPropertyMetadata(typeof(ImageDisplay)));
        }

        public static readonly DependencyProperty ImageSourceProperty =
          DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageDisplay), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
    }
}
