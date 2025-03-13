using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WassupClient.ViewModels;
using WassupLib;
using WassupLib.Managers;
using WassupLib.Models;

namespace WassupClient.Views
{
	public partial class RegisterView : UserControl
	{
		public RegisterView()
		{
			InitializeComponent();
			DataContext = Application.Current.MainWindow.DataContext;
		}

		/// <summary>
		/// Checks if a user is registered and tries to log in
		/// </summary>
		private void btnRegister_Click(object sender, RoutedEventArgs e)
		{
			var user = (this.DataContext as Core).Register(tbUsername.Text, pbPassword.Password);

			// If register successful
			if (user != null)
			{
				// Sets the user in the datacontext
				(this.DataContext as Core).User = user;
				// Changes view to Home
				(this.DataContext as Core).ChangeView("Home");
			}
			else
			{
				// Already registered
				(this.DataContext as Core).Error = "Utente già registrato";
			}
		}

		private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
		{
			if ((this.DataContext as Core).Error != string.Empty)
				(this.DataContext as Core).Error = string.Empty;
		}

		private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
		{
			if ((this.DataContext as Core).Error != string.Empty)
				(this.DataContext as Core).Error = string.Empty;
		}

		private void OnLinkClick(object sender, RoutedEventArgs e)
		{
			(this.DataContext as Core).ChangeView("Login");
		}
	}
}
