using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lift.UI.V2.Controls.PropertyGrid.Editors;

public class TextBoxEditor : BasePropertyEditor
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem) =>
        new System.Windows.Controls.TextBox()
        {
            IsReadOnly = false
        };

    public override DependencyProperty GetDependencyProperty()
        => System.Windows.Controls.TextBlock.TextProperty;


}
