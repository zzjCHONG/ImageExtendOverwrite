using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControlLibrary1
{

    public class MyButton : Button
    {
        static MyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyButton), new FrameworkPropertyMetadata(typeof(MyButton)));
        }

        public static readonly DependencyProperty CustomTextProperty =
             DependencyProperty.Register("CustomText", typeof(string), typeof(MyButton), new PropertyMetadata(string.Empty));

        public string CustomText
        {
            get => (string) GetValue(CustomTextProperty);
            set => SetValue(CustomTextProperty, value);
        }
    }
}
