using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
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
			dynamic res = (this.DataContext as Core).Register(tbUsername.Text, pbPassword.Password);

			if (res != null)
			{
				var user = JsonSerializer.Deserialize<User>(res.user);
				var users = JsonSerializer.Deserialize<ObservableCollection<User>>(res.users);

				// If register successful
				if (user != null)
				{
					// Sets user & chats in datacontext
					(this.DataContext as Core).User = user;
					(this.DataContext as Core).Users = users;
					(this.DataContext as Core).UserChats = new ObservableCollection<Chat>();
					// Changes view to Home
					(this.DataContext as Core).ChangeView("Home");
				}
				else
				{
					// Already registered
					(this.DataContext as Core).Error = "Utente già registrato";
				}
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
