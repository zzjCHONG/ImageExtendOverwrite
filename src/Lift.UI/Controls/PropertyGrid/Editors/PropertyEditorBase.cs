﻿using System.Windows;
using System.Windows.Data;

namespace Lift.UI.Controls;

public abstract class PropertyEditorBase : DependencyObject
{
    public abstract FrameworkElement CreateElement(PropertyItem propertyItem);

    public virtual void CreateBinding(PropertyItem propertyItem, DependencyObject element)
    {
        var name = propertyItem.PropertyName;
        var k = propertyItem.Category;
        var ks = propertyItem.DisplayName;
        var sfe = propertyItem.Value;

        var binding = new Binding($"{propertyItem.PropertyName}")
        {
            Source = propertyItem.Value,
            Mode = GetBindingMode(propertyItem),
            UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
            Converter = GetConverter(propertyItem)
        };

        BindingOperations.SetBinding(element, GetDependencyProperty(), binding);
    }


    public abstract DependencyProperty GetDependencyProperty();

    public virtual BindingMode GetBindingMode(PropertyItem propertyItem) =>
        propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

    public virtual UpdateSourceTrigger GetUpdateSourceTrigger(PropertyItem propertyItem) =>
        UpdateSourceTrigger.PropertyChanged;

    protected virtual IValueConverter GetConverter(PropertyItem propertyItem) => null;
}
