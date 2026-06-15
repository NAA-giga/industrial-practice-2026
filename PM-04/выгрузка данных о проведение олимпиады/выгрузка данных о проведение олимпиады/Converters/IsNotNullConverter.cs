using System;
using System.Globalization;
using System.Windows.Data;

namespace выгрузка_данных_о_проведение_олимпиады.Converters;

public class IsNotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value != null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}