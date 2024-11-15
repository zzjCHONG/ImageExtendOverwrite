﻿using System;
using System.Collections.Generic;
using System.Windows;
using Lift.UI.Tools;

namespace Lift.UI.Themes;

public class SharedResourceDictionary : ResourceDictionary
{
    public static Dictionary<Uri, ResourceDictionary> SharedDictionaries = new();

    private Uri _sourceUri;

    public new Uri Source
    {
        get => DesignerHelper.IsInDesignMode ? base.Source : _sourceUri;
        set
        {
            if (value == null) return;
            if (DesignerHelper.IsInDesignMode)
            {
                base.Source = value;
                return;
            }

            _sourceUri = value;

            if (!SharedDictionaries.ContainsKey(value))
            {
                base.Source = value;
                SharedDictionaries.Add(value, this);
            }
            else
            {
                MergedDictionaries.Add(SharedDictionaries[value]);
            }
        }
    }
}
