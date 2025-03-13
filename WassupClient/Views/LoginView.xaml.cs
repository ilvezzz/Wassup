using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WassupClient.ViewModels;
using WassupLib;
using WassupLib.Managers;

namespace WassupClient.Views
{
    public partial class LoginView : UserControl
    {
		public LoginView()
		{
			InitializeComponent();
			DataContext = Application.Current.MainWindow.DataContext;
		}

		/// <summary>
		/// Checks if a user is registered and tries to log in
		/// </summary>
		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			var user = (this.DataContext as Core).Login(tbUsername.Text, pbPassword.Password);

			// If login successful
			if (user != null)
			{
				// Sets the user in the datacontext
				(this.DataContext as Core).User = user;
				// Changes view to Home
				(this.DataContext as Core).ChangeView("Home");
			}
			else
			{
				// Wrong username or password
				(this.DataContext as Core).Error = "Username o password errati";
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
			(this.DataContext as Core).ChangeView("Register");
		}
	}
}
