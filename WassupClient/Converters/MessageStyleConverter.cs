using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WassupClient.Converters
{
	public class MessageStyleConverter : IValueConverter
	{
		public Style SentMessageStyle { get; set; }
		public Style ReceivedMessageStyle { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Gets sender and current user's username
			string sender = value as string;
			//string currentUsername = parameter as string;
			string currentUsername = (Application.Current.MainWindow.DataContext as Core).User.Username;

			bool isCurrentUser = sender == currentUsername;

			Style style = new Style(typeof(Border));

			style.Setters.Add(new Setter(Border.BackgroundProperty, isCurrentUser ? Brushes.LightGreen : Brushes.LightGray));
			style.Setters.Add(new Setter(Border.BorderBrushProperty, isCurrentUser ? Brushes.Green : Brushes.Gray));

			style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(12)));
			style.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(8)));
			style.Setters.Add(new Setter(Border.MarginProperty, new Thickness(5)));

			style.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, isCurrentUser ? HorizontalAlignment.Right : HorizontalAlignment.Left));

			return style;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
