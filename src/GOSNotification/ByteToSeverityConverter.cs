using System;
using System.Globalization;
using Avalonia.Data.Converters;
using FluentAvalonia.UI.Controls;

namespace GOSAvaloniaControls;

public class ByteToSeverityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Convert(value);
    }

    public static object? Convert(object? value)
    {
        if (value is null)
            return null;
        if (value is byte number)
        {
            return number switch
            { 
                1 => FAInfoBarSeverity.Success,
                2 => FAInfoBarSeverity.Warning,
                3 => FAInfoBarSeverity.Error,
                4 => FAInfoBarSeverity.Informational,
                _ => FAInfoBarSeverity.Informational,
            };
        }
        else if (value is FAInfoBarSeverity info)
        {
#if DEBUG
            var trash = (byte)info;
#endif


            return ((byte)info);
        }
        return null;
    }
}
