using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace ConfigEditorUI.Services
{
	public class ListValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var list = value as List<string>;
			if (list == null) return String.Empty;

			return String.Join(",", list.ToArray());
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			if (value == null) return new List<string>();
			return new List<string>(value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}
