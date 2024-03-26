using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Lift.UI.Controls;

namespace Lift.UI.V2.Controls.PropertyGrid.Editors;

public class NumericEditor : BasePropertyEditor
{
    /// <inheritdoc />
    public override FrameworkElement CreateElement(PropertyItem propertyItem)
    {
        // todo 添加对Range特性的支持


        var editor = new NumericUpDown();
        return editor;
    }

    public override DependencyProperty GetDependencyProperty()
        => NumericUpDown.ValueProperty;
}
