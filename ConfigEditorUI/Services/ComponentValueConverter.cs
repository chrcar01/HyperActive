using System;
using System.Collections.Generic;
using System.Windows.Data;
using HyperActive.Core.Config;

namespace ConfigEditorUI.Services
{
	public class ComponentValueConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			return "Silly";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			return new List<ComponentDescriptor>();
		}
	}
}
