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
			dynamic res = (this.DataContext as Core).Login(tbUsername.Text, pbPassword.Password);

			if (res != null)
			{
                var user = JsonSerializer.Deserialize<User>(res.user);
                var users = JsonSerializer.Deserialize<ObservableCollection<User>>(res.users);
                var chats = JsonSerializer.Deserialize<ObservableCollection<Chat>>(res.chats);

                // If login successful
                if (user != null)
                {
                    // Sets user & chats in datacontext
                    (this.DataContext as Core).User = user;
                    (this.DataContext as Core).Users = users;
                    (this.DataContext as Core).UserChats = chats;
                    // Changes view to Home
                    (this.DataContext as Core).ChangeView("Home");
                }
                else
                {
                    // Wrong username or password
                    (this.DataContext as Core).Error = "Username o password errati";
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
			(this.DataContext as Core).ChangeView("Register");
		}
	}
}
