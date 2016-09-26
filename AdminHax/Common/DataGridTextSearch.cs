using System;
using System.Windows;
using System.Windows.Data;

namespace AdminHax.Common
{
	public static class DataGridTextSearch
	{
		public static readonly DependencyProperty SearchValueProperty =
			DependencyProperty.RegisterAttached("SearchValue", typeof(string), typeof(DataGridTextSearch),
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

		public static string GetSearchValue(DependencyObject obj)
		{
			return (string)obj.GetValue(SearchValueProperty);
		}

		public static void SetSearchValue(DependencyObject obj, string value)
		{
			obj.SetValue(SearchValueProperty, value);
		}

		public static readonly DependencyProperty IsTextMatchProperty =
			DependencyProperty.RegisterAttached("IsTextMatch", typeof(bool), typeof(DataGridTextSearch), new UIPropertyMetadata(false));

		public static bool GetIsTextMatch(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsTextMatchProperty);
		}

		public static void SetIsTextMatch(DependencyObject obj, bool value)
		{
			obj.SetValue(IsTextMatchProperty, value);
		}
	}

	public class SearchValueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string cellText = values[0] == null ? string.Empty : values[0].ToString();
			string searchText = values[1] as string;

			if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(cellText))
			{
				return cellText.Equals(searchText, StringComparison.OrdinalIgnoreCase) || cellText.StartsWith(searchText, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
