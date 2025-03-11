using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WassupLib;
using WassupLib.Managers;
using WassupLib.ViewModels;

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
			// Gets the user
			var user = new User(tbUsername.Text, pbPassword.Password);

			// Gets the registered users
			var users = FileManager.GetUsers();

			// If the user isn't registered
			if (!users.Any(x => x.Username == user.Username))
			{
				// Adds the user to the list
				users.Add(user);

				// Updates the db
				FileManager.UpdateDb(users);

				// Sets the user in the datacontext
				((this.DataContext as Core).SelectedViewModel as BaseViewModel).User = user;

				// Changes view to Home
				(this.DataContext as Core).ChangeView("Home");
			}
			else
			{
				// User already registered
				((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error = "Utente già registrato";
			}
		}

		private void tbUsername_GotFocus(object sender, RoutedEventArgs e)
		{
			if (((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error != string.Empty)
				((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error = string.Empty;
		}

		private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
		{
			if (((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error != string.Empty)
				((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error = string.Empty;
		}

		private void OnLinkClick(object sender, RoutedEventArgs e)
		{
			(this.DataContext as Core).ChangeView("Login");
		}
	}
}
