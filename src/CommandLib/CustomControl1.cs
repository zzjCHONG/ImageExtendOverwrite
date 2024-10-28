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

namespace CommandLib
{
   
    public class CustomControl1 : Control
    {
        public static readonly RoutedEvent MyCustomEvent = EventManager.RegisterRoutedEvent(
          "MyCustom", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomControl1));

        public event RoutedEventHandler MyCustom
        {
            add { AddHandler(MyCustomEvent, value); }
            remove { RemoveHandler(MyCustomEvent, value); }
        }

        static CustomControl1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
        }

        public void RaiseMyCustomEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(MyCustomEvent);
            RaiseEvent(args);
        }
    }
}
