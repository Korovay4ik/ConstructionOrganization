using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace constructionOrgManagement.Converters
{
    class DateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
                return new DateTimeOffset(new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day));

            if (value is null)
                return null;

            throw new NotSupportedException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);

            if (value is DateTimeOffset dateTimeOffset)
                return DateOnly.FromDateTime(dateTimeOffset.DateTime);

            if (value is null)
                return null;

            throw new NotSupportedException();
        }
    }
}
