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

namespace StaticLib
{

    public class CustomControl1 : Control
    {
        static CustomControl1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
        }

        //public static readonly RoutedUICommand MyCommand = new RoutedUICommand(
        //   "My Command", "MyCommand", typeof(CustomControl1));
        public static readonly RoutedUICommand MyCommand = new();

        public CustomControl1()
        {
            //CommandBindings.Add(new CommandBinding(MyCommand, ExecuteMyCommand));

            CommandBindings.Add(new CommandBinding(MyCommand, (obj, args) =>
            {
                if (args.Parameter is not string command) return;

                MessageBox.Show("MyCommand executed!");
            }));

        }

        private void ExecuteMyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("MyCommand executed!");
        }
    }
}
