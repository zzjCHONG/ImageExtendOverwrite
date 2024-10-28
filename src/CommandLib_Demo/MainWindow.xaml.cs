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

namespace CommandLib_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            myCustomControl.MyCustom += MyCustomControl_MyCustom;
        }

        private void MyCustomControl_MyCustom(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Custom event triggered!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 触发自定义事件
            myCustomControl.RaiseMyCustomEvent();
        }
    }
}
