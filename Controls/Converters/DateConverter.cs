using System;
using System.Globalization;
using System.Windows.Data;

namespace Controls
{
	public class DateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			DateTime n = (DateTime)value;
			return n.ToString("MM/dd/y");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new Exception("No reverse conversion for string->DateTime");
		}
	}
}
