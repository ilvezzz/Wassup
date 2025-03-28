using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WassupLib.Managers;

namespace WassupClient.Converters
{
	public class Base64ToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string mess = value.ToString(), type = parameter.ToString();

			if (type == "image")
				return ImageManager.Base64ToImg(mess);
			else
				return mess;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
