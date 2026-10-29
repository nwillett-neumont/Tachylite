using System;
using Avalonia;
using Avalonia.Controls;

namespace Tachylite;

public class NativeWebViewHtmlSource : AvaloniaObject
{
    public static readonly AttachedProperty<string> HtmlSourceProperty = 
        AvaloniaProperty.RegisterAttached<NativeWebViewHtmlSource, NativeWebView, string>("HtmlSource");
    
    public static void SetHtmlSource(NativeWebView element, string value) => element.SetValue(HtmlSourceProperty, value);

    public static string GetHtmlSource(NativeWebView element) => element.GetValue(HtmlSourceProperty);

    static NativeWebViewHtmlSource()
    {
        HtmlSourceProperty.Changed.AddClassHandler<NativeWebView>((control, _) => control.NavigateToString(GetHtmlSource(control)));
    }
}
