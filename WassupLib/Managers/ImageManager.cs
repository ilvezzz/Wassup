using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace WassupLib.Managers
{
	public static class ImageManager
	{
		/// <summary>
		/// Converts an image to ImageSource to base64
		/// </summary>
		/// <param name="img">ImageSource to convert</param>
		/// <returns>Base64 string representing the image</returns>
		public static string ImgToBase64(ImageSource img)
		{
			// If the ImageSource is a BitmapImage
			if (img != null && img is BitmapImage bmp)
			{
				// Gets the URI
				Uri uri = bmp.UriSource;

				// If the URI exists
				if (uri != null)
				{
					// Gets the image path
					return Convert.ToBase64String(File.ReadAllBytes(uri.LocalPath));
				}
				// If the URI doesn't exist
				else
				{
					// Reads the source with a memorystream and returns it in base64
					var encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img));
					using (var ms = new MemoryStream())
					{
						encoder.Save(ms);
						return Convert.ToBase64String(ms.ToArray());
					}
				}
			}

			// Returns the default activity image
			return @"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";
		}

		public static BitmapImage Base64ToImg(string base64image)
		{
			byte[] bytes;

			try
			{
				bytes = Convert.FromBase64String(base64image);

				// Gets the img from a MemoryStream and returns it
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.StreamSource = new MemoryStream(bytes);
				bmp.EndInit();
				return bmp;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
