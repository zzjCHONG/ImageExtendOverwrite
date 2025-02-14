﻿using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Lift.UI.Tools;

namespace Lift.UI.Properties.Langs;

/// <summary>
/// usage:
/// <example>
/// <code lang="C#">
/// Properties.Langs.LangProvider.Culture = new CultureInfo("");
/// var jump = Properties.Langs.LangProvider.GetLang("Jump");
/// </code>
/// </example>
/// </summary>
public class LangProvider : INotifyPropertyChanged
{
    internal static LangProvider Instance { get; } = ResourceHelper.GetResourceInternal<LangProvider>("Langs");

    private static string CultureInfoStr;

    public static CultureInfo Culture
    {
        get => Lang.Culture;
        set
        {
            if (value == null) return;
            if (Equals(CultureInfoStr, value.EnglishName)) return;
            Lang.Culture = value;
            CultureInfoStr = value.EnglishName;

            Instance.UpdateLangs();
        }
    }

    public static string GetLang(string key) => Lang.ResourceManager.GetString(key, Culture);

    public static void SetLang(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string key) =>
        BindingOperations.SetBinding(dependencyObject, dependencyProperty, new Binding(key)
        {
            Source = Instance,
            Mode = BindingMode.OneWay
        });

    private void UpdateLangs()
    {
        OnPropertyChanged(nameof(BusyIndicatorLoading));
        OnPropertyChanged(nameof(All));
        OnPropertyChanged(nameof(Am));
        OnPropertyChanged(nameof(Cancel));
        OnPropertyChanged(nameof(Clear));
        OnPropertyChanged(nameof(Close));
        OnPropertyChanged(nameof(CloseAll));
        OnPropertyChanged(nameof(CloseOther));
        OnPropertyChanged(nameof(Confirm));
        OnPropertyChanged(nameof(ErrorImgPath));
        OnPropertyChanged(nameof(ErrorImgSize));
        OnPropertyChanged(nameof(Find));
        OnPropertyChanged(nameof(FormatError));
        OnPropertyChanged(nameof(Interval10m));
        OnPropertyChanged(nameof(Interval1h));
        OnPropertyChanged(nameof(Interval1m));
        OnPropertyChanged(nameof(Interval2h));
        OnPropertyChanged(nameof(Interval30m));
        OnPropertyChanged(nameof(Interval30s));
        OnPropertyChanged(nameof(Interval5m));
        OnPropertyChanged(nameof(IsNecessary));
        OnPropertyChanged(nameof(Jump));
        OnPropertyChanged(nameof(LangComment));
        OnPropertyChanged(nameof(Miscellaneous));
        OnPropertyChanged(nameof(NextPage));
        OnPropertyChanged(nameof(No));
        OnPropertyChanged(nameof(NoData));
        OnPropertyChanged(nameof(OutOfRange));
        OnPropertyChanged(nameof(PageMode));
        OnPropertyChanged(nameof(Pm));
        OnPropertyChanged(nameof(PngImg));
        OnPropertyChanged(nameof(PreviousPage));
        OnPropertyChanged(nameof(ScrollMode));
        OnPropertyChanged(nameof(Tip));
        OnPropertyChanged(nameof(TooLarge));
        OnPropertyChanged(nameof(TwoPageMode));
        OnPropertyChanged(nameof(Unknown));
        OnPropertyChanged(nameof(UnknownSize));
        OnPropertyChanged(nameof(Yes));
        OnPropertyChanged(nameof(ZoomIn));
        OnPropertyChanged(nameof(ZoomOut));
    }

    /// <summary>
    /// 查找类似 全部 的本地化字符串。
    /// </summary>
    public string All => Lang.All;

    /// <summary>
    /// 查找类似 上午 的本地化字符串。
    /// </summary>
    public string Am => Lang.Am;

    public string BusyIndicatorLoading => Lang.BusyIndicatorLoading;

    /// <summary>
    /// 查找类似 取消 的本地化字符串。
    /// </summary>
    public string Cancel => Lang.Cancel;

    /// <summary>
    /// 查找类似 清空 的本地化字符串。
    /// </summary>
    public string Clear => Lang.Clear;

    /// <summary>
    /// 查找类似 关闭 的本地化字符串。
    /// </summary>
    public string Close => Lang.Close;

    /// <summary>
    /// 查找类似 关闭所有 的本地化字符串。
    /// </summary>
    public string CloseAll => Lang.CloseAll;

    /// <summary>
    /// 查找类似 关闭其他 的本地化字符串。
    /// </summary>
    public string CloseOther => Lang.CloseOther;

    /// <summary>
    /// 查找类似 确定 的本地化字符串。
    /// </summary>
    public string Confirm => Lang.Confirm;

    /// <summary>
    /// 查找类似 错误的图片路径 的本地化字符串。
    /// </summary>
    public string ErrorImgPath => Lang.ErrorImgPath;

    /// <summary>
    /// 查找类似 非法的图片尺寸 的本地化字符串。
    /// </summary>
    public string ErrorImgSize => Lang.ErrorImgSize;

    /// <summary>
    /// 查找类似 查找 的本地化字符串。
    /// </summary>
    public string Find => Lang.Find;

    /// <summary>
    /// 查找类似 格式错误 的本地化字符串。
    /// </summary>
    public string FormatError => Lang.FormatError;

    /// <summary>
    /// 查找类似 间隔10分钟 的本地化字符串。
    /// </summary>
    public string Interval10m => Lang.Interval10m;

    /// <summary>
    /// 查找类似 间隔1小时 的本地化字符串。
    /// </summary>
    public string Interval1h => Lang.Interval1h;

    /// <summary>
    /// 查找类似 间隔1分钟 的本地化字符串。
    /// </summary>
    public string Interval1m => Lang.Interval1m;

    /// <summary>
    /// 查找类似 间隔2小时 的本地化字符串。
    /// </summary>
    public string Interval2h => Lang.Interval2h;

    /// <summary>
    /// 查找类似 间隔30分钟 的本地化字符串。
    /// </summary>
    public string Interval30m => Lang.Interval30m;

    /// <summary>
    /// 查找类似 间隔30秒 的本地化字符串。
    /// </summary>
    public string Interval30s => Lang.Interval30s;

    /// <summary>
    /// 查找类似 间隔5分钟 的本地化字符串。
    /// </summary>
    public string Interval5m => Lang.Interval5m;

    /// <summary>
    /// 查找类似 不能为空 的本地化字符串。
    /// </summary>
    public string IsNecessary => Lang.IsNecessary;

    /// <summary>
    /// 查找类似 跳转 的本地化字符串。
    /// </summary>
    public string Jump => Lang.Jump;

    /// <summary>
    /// 查找类似 查找类似 {0} 的本地化字符串。 的本地化字符串。
    /// </summary>
    public string LangComment => Lang.LangComment;

    /// <summary>
    /// 查找类似 杂项 的本地化字符串。
    /// </summary>
    public string Miscellaneous => Lang.Miscellaneous;

    /// <summary>
    /// 查找类似 下一页 的本地化字符串。
    /// </summary>
    public string NextPage => Lang.NextPage;

    /// <summary>
    /// 查找类似 否 的本地化字符串。
    /// </summary>
    public string No => Lang.No;

    /// <summary>
    /// 查找类似 暂无数据 的本地化字符串。
    /// </summary>
    public string NoData => Lang.NoData;

    /// <summary>
    /// 查找类似 不在范围内 的本地化字符串。
    /// </summary>
    public string OutOfRange => Lang.OutOfRange;

    /// <summary>
    /// 查找类似 页面模式 的本地化字符串。
    /// </summary>
    public string PageMode => Lang.PageMode;

    /// <summary>
    /// 查找类似 下午 的本地化字符串。
    /// </summary>
    public string Pm => Lang.Pm;

    /// <summary>
    /// 查找类似 PNG图片 的本地化字符串。
    /// </summary>
    public string PngImg => Lang.PngImg;

    /// <summary>
    /// 查找类似 上一页 的本地化字符串。
    /// </summary>
    public string PreviousPage => Lang.PreviousPage;

    /// <summary>
    /// 查找类似 滚动模式 的本地化字符串。
    /// </summary>
    public string ScrollMode => Lang.ScrollMode;

    /// <summary>
    /// 查找类似 提示 的本地化字符串。
    /// </summary>
    public string Tip => Lang.Tip;

    /// <summary>
    /// 查找类似 过大 的本地化字符串。
    /// </summary>
    public string TooLarge => Lang.TooLarge;

    /// <summary>
    /// 查找类似 双页模式 的本地化字符串。
    /// </summary>
    public string TwoPageMode => Lang.TwoPageMode;

    /// <summary>
    /// 查找类似 未知 的本地化字符串。
    /// </summary>
    public string Unknown => Lang.Unknown;

    /// <summary>
    /// 查找类似 未知大小 的本地化字符串。
    /// </summary>
    public string UnknownSize => Lang.UnknownSize;

    /// <summary>
    /// 查找类似 是 的本地化字符串。
    /// </summary>
    public string Yes => Lang.Yes;

    /// <summary>
    /// 查找类似 放大 的本地化字符串。
    /// </summary>
    public string ZoomIn => Lang.ZoomIn;

    /// <summary>
    /// 查找类似 缩小 的本地化字符串。
    /// </summary>
    public string ZoomOut => Lang.ZoomOut;


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class LangKeys
{
    /// <summary>
    /// 查找类似 全部 的本地化字符串。
    /// </summary>
    public static string All = nameof(All);

    /// <summary>
    /// 查找类似 上午 的本地化字符串。
    /// </summary>
    public static string Am = nameof(Am);

    public static string BusyIndicatorLoading = nameof(BusyIndicatorLoading);

    /// <summary>
    /// 查找类似 取消 的本地化字符串。
    /// </summary>
    public static string Cancel = nameof(Cancel);

    /// <summary>
    /// 查找类似 清空 的本地化字符串。
    /// </summary>
    public static string Clear = nameof(Clear);

    /// <summary>
    /// 查找类似 关闭 的本地化字符串。
    /// </summary>
    public static string Close = nameof(Close);

    /// <summary>
    /// 查找类似 关闭所有 的本地化字符串。
    /// </summary>
    public static string CloseAll = nameof(CloseAll);

    /// <summary>
    /// 查找类似 关闭其他 的本地化字符串。
    /// </summary>
    public static string CloseOther = nameof(CloseOther);

    /// <summary>
    /// 查找类似 确定 的本地化字符串。
    /// </summary>
    public static string Confirm = nameof(Confirm);

    /// <summary>
    /// 查找类似 错误的图片路径 的本地化字符串。
    /// </summary>
    public static string ErrorImgPath = nameof(ErrorImgPath);

    /// <summary>
    /// 查找类似 非法的图片尺寸 的本地化字符串。
    /// </summary>
    public static string ErrorImgSize = nameof(ErrorImgSize);

    /// <summary>
    /// 查找类似 查找 的本地化字符串。
    /// </summary>
    public static string Find = nameof(Find);

    /// <summary>
    /// 查找类似 格式错误 的本地化字符串。
    /// </summary>
    public static string FormatError = nameof(FormatError);

    /// <summary>
    /// 查找类似 间隔10分钟 的本地化字符串。
    /// </summary>
    public static string Interval10m = nameof(Interval10m);

    /// <summary>
    /// 查找类似 间隔1小时 的本地化字符串。
    /// </summary>
    public static string Interval1h = nameof(Interval1h);

    /// <summary>
    /// 查找类似 间隔1分钟 的本地化字符串。
    /// </summary>
    public static string Interval1m = nameof(Interval1m);

    /// <summary>
    /// 查找类似 间隔2小时 的本地化字符串。
    /// </summary>
    public static string Interval2h = nameof(Interval2h);

    /// <summary>
    /// 查找类似 间隔30分钟 的本地化字符串。
    /// </summary>
    public static string Interval30m = nameof(Interval30m);

    /// <summary>
    /// 查找类似 间隔30秒 的本地化字符串。
    /// </summary>
    public static string Interval30s = nameof(Interval30s);

    /// <summary>
    /// 查找类似 间隔5分钟 的本地化字符串。
    /// </summary>
    public static string Interval5m = nameof(Interval5m);

    /// <summary>
    /// 查找类似 不能为空 的本地化字符串。
    /// </summary>
    public static string IsNecessary = nameof(IsNecessary);

    /// <summary>
    /// 查找类似 跳转 的本地化字符串。
    /// </summary>
    public static string Jump = nameof(Jump);

    /// <summary>
    /// 查找类似 查找类似 {0} 的本地化字符串。 的本地化字符串。
    /// </summary>
    public static string LangComment = nameof(LangComment);

    /// <summary>
    /// 查找类似 杂项 的本地化字符串。
    /// </summary>
    public static string Miscellaneous = nameof(Miscellaneous);

    /// <summary>
    /// 查找类似 下一页 的本地化字符串。
    /// </summary>
    public static string NextPage = nameof(NextPage);

    /// <summary>
    /// 查找类似 否 的本地化字符串。
    /// </summary>
    public static string No = nameof(No);

    /// <summary>
    /// 查找类似 暂无数据 的本地化字符串。
    /// </summary>
    public static string NoData = nameof(NoData);

    /// <summary>
    /// 查找类似 不在范围内 的本地化字符串。
    /// </summary>
    public static string OutOfRange = nameof(OutOfRange);

    /// <summary>
    /// 查找类似 页面模式 的本地化字符串。
    /// </summary>
    public static string PageMode = nameof(PageMode);

    /// <summary>
    /// 查找类似 下午 的本地化字符串。
    /// </summary>
    public static string Pm = nameof(Pm);

    /// <summary>
    /// 查找类似 PNG图片 的本地化字符串。
    /// </summary>
    public static string PngImg = nameof(PngImg);

    /// <summary>
    /// 查找类似 上一页 的本地化字符串。
    /// </summary>
    public static string PreviousPage = nameof(PreviousPage);

    /// <summary>
    /// 查找类似 滚动模式 的本地化字符串。
    /// </summary>
    public static string ScrollMode = nameof(ScrollMode);

    /// <summary>
    /// 查找类似 提示 的本地化字符串。
    /// </summary>
    public static string Tip = nameof(Tip);

    /// <summary>
    /// 查找类似 过大 的本地化字符串。
    /// </summary>
    public static string TooLarge = nameof(TooLarge);

    /// <summary>
    /// 查找类似 双页模式 的本地化字符串。
    /// </summary>
    public static string TwoPageMode = nameof(TwoPageMode);

    /// <summary>
    /// 查找类似 未知 的本地化字符串。
    /// </summary>
    public static string Unknown = nameof(Unknown);

    /// <summary>
    /// 查找类似 未知大小 的本地化字符串。
    /// </summary>
    public static string UnknownSize = nameof(UnknownSize);

    /// <summary>
    /// 查找类似 是 的本地化字符串。
    /// </summary>
    public static string Yes = nameof(Yes);

    /// <summary>
    /// 查找类似 放大 的本地化字符串。
    /// </summary>
    public static string ZoomIn = nameof(ZoomIn);

    /// <summary>
    /// 查找类似 缩小 的本地化字符串。
    /// </summary>
    public static string ZoomOut = nameof(ZoomOut);
}
