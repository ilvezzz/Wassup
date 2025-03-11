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
			// Gets the registered users
			var users = FileManager.GetUsers();

			// If the user is already registered
			if (users.Any(user => user.Username == tbUsername.Text && user.Password == pbPassword.Password))
			{
				// Sets the user in the datacontext
				(this.DataContext as Core).User = users.Single(x => x.Username.Equals(tbUsername.Text) && x.Password.Equals(pbPassword.Password));

				// Changes view to Home
				(this.DataContext as Core).ChangeView("Home");
			}
			else
			{
				// Wrong username or password
				((this.DataContext as Core).SelectedViewModel as BaseViewModel).Error = "Username o password errati";
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
			(this.DataContext as Core).ChangeView("Register");
		}
	}
}
