using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WassupLib.Managers;
using WassupLib.Models;

namespace WassupClient.Views
{
	public partial class AccountView : UserControl
	{
		public AccountView()
		{
			InitializeComponent();
			this.DataContext = Application.Current.MainWindow.DataContext;

			tbUsername.Text = (this.DataContext as Core).User.Username;
			pbPassword.Password = (this.DataContext as Core).User.Password;
		}
		/// <summary>
		/// Saves changes
		/// </summary>
		private void SaveAccount(object sender, RoutedEventArgs e)
		{
			string username = tbUsername.Text.Trim();
			string password = pbPassword.Password.Trim();
			string img = ImageManager.ImgToBase64(imgProfile.Source);

			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(img))
				(this.DataContext as Core).EditProfile(username, password, img);
		}
		/// <summary>
		/// Opens ofd to edit image
		/// </summary>
		private void EditImage(object sender, RoutedEventArgs e)
		{
			// Inits the OpenFileDialog with the img filters
			var ofd = new OpenFileDialog();
			ofd.Filter = "Immagini|*.jpg;*.jpeg;*.png";

			// If accepts and gets the image, sets it
			if (ofd.ShowDialog() == true)
			{
				if (new FileInfo(ofd.FileName).Length > 1_073_741_824)
				{
					MessageBox.Show("La dimensione dell'immagine non può superare 1GB", "Immagine troppo grande");
					return;
				}

				imgProfile.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
			}
		}
		/// <summary>
		/// Reloads HomeView
		/// </summary>
		private void GoHome(object sender, RoutedEventArgs e)
		{
			// Goes back to home
			(this.DataContext as Core).ChangeView("Home");
		}
	}
}
