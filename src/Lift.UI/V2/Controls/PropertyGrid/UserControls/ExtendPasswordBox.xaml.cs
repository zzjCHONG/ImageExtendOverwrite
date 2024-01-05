using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lift.UI.V2.Controls.PropertyGrid.UserControls;

/// <summary>
/// Interaction logic for ExtendPasswordBox.xaml
/// </summary>
public partial class ExtendPasswordBox : UserControl
{
    public ExtendPasswordBox()
    {
        InitializeComponent();

        this.PasswordBox.PasswordChanged += (s, e) =>
        {
            this.ActualPassword = PasswordBox.Password;
        };
    }

    public static readonly DependencyProperty ActualPasswordProperty = DependencyProperty.Register(
        nameof(ActualPassword), typeof(string), typeof(ExtendPasswordBox), new PropertyMetadata(null));

    public string? ActualPassword
    {
        get => (string?) GetValue(ActualPasswordProperty);
        set => SetValue(ActualPasswordProperty, value);
    }
}
