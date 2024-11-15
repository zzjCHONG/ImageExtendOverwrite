﻿using System.Windows;
using Lift.UI.Data;

namespace Lift.UI.Controls;

public class ToggleButtonAttach
{
    public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.RegisterAttached(
        "ShowLabel", typeof(bool), typeof(ToggleButtonAttach),
        new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetShowLabel(DependencyObject element, bool value) =>
        element.SetValue(ShowLabelProperty, ValueBoxes.BooleanBox(value));

    public static bool GetShowLabel(DependencyObject element) => (bool) element.GetValue(ShowLabelProperty);
}
