﻿using System;
using System.Runtime.InteropServices;
using Lift.UI.Data;
using Lift.UI.Themes;
using Lift.UI.Tools;
using Lift.UI.Tools.Interop;

namespace Lift.UI.Controls;

public class BlurWindow : Window
{
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var version = OSVersionHelper.GetOSVersion();
        var versionInfo = new SystemVersionInfo(version.Major, version.Minor, version.Build);

        if (versionInfo >= SystemVersionInfo.Windows10_1903)
        {
            this.GetHwndSource()?.AddHook(HwndSourceHook);
        }

        ThemeManager.Current.ActualApplicationThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(ThemeManager sender, object args)
    {
        EnableBlur(this, true);
    }

    private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        switch (msg)
        {
            case InteropValues.WM_ENTERSIZEMOVE:
                EnableBlur(this, false);
                break;
            case InteropValues.WM_EXITSIZEMOVE:
                EnableBlur(this, true);
                break;
        }

        return IntPtr.Zero;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        EnableBlur(this, true);
    }

    public static void EnableBlur(Window window, bool isEnabled)
    {
        var version = OSVersionHelper.GetOSVersion();
        var versionInfo = new SystemVersionInfo(version.Major, version.Minor, version.Build);

        var accentPolicy = new InteropValues.ACCENTPOLICY();
        var accentPolicySize = Marshal.SizeOf(accentPolicy);

        accentPolicy.AccentFlags = 2;

        if (isEnabled)
        {
            if (versionInfo >= SystemVersionInfo.Windows10_1809)
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            }
            else if (versionInfo >= SystemVersionInfo.Windows10)
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_TRANSPARENTGRADIENT;
            }
        }
        else
        {
            accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND;
        }

        accentPolicy.GradientColor = ResourceHelper.GetResource<uint>(ResourceToken.BlurGradientValue);

        var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
        Marshal.StructureToPtr(accentPolicy, accentPtr, false);

        var data = new InteropValues.WINCOMPATTRDATA
        {
            Attribute = InteropValues.WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
            DataSize = accentPolicySize,
            Data = accentPtr
        };

        InteropMethods.SetWindowCompositionAttribute(window.GetHandle(), ref data);

        Marshal.FreeHGlobal(accentPtr);
    }
}
