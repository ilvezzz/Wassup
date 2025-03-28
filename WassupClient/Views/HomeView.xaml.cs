using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WassupClient.Converters;
using WassupLib.Managers;
using WassupLib.Models;

namespace WassupClient.Views
{
	public partial class HomeView : UserControl
	{
		public HomeView()
		{
			InitializeComponent();
			DataContext = Application.Current.MainWindow.DataContext;

			this.Resources.Add("Base64ToImageConverter", new Base64ToImageConverter());
		}

		private void SendMessage(object sender, RoutedEventArgs e)
		{
			if ((this.DataContext as Core).SelectedChat != null && !string.IsNullOrWhiteSpace(tbMessage.Text))
			{
				// Sends mess
				(this.DataContext as Core).SendMessage("text", tbMessage.Text);
				tbMessage.Text = string.Empty;
			}
		}

		private void AttachImage(object sender, RoutedEventArgs e)
		{
			if ((this.DataContext as Core).SelectedChat != null)
			{
				// Config ofd
				OpenFileDialog ofd = new OpenFileDialog()
				{
					Filter = "Immagini|*.jpg;*.jpeg;*.png",
					Title = "Seleziona un'immagine"
				};

				// Choose img
				if (ofd.ShowDialog() == true)
				{
					if (new FileInfo(ofd.FileName).Length > 1_073_741_824)
					{
						MessageBox.Show("La dimensione dell'immagine non può superare 1GB", "Immagine troppo grande");
						return;
					}

					// Gets BitmapImage from ofd
					var image = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));

					if (image != null)
					{
						// Converts image to base64
						string base64Image = ImageManager.ImgToBase64(image);

						// Sends image
						if (!string.IsNullOrEmpty(base64Image))
							(this.DataContext as Core).SendMessage("image", base64Image);
					}
				}
			}
		}
	}
}
