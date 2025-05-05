using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace constructionOrgManagement.Converters
{
    public class NumericConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return System.Convert.ToDecimal(value);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
