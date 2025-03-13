using System.ComponentModel;
using WassupLib.Models;
using WassupLib.Managers;
using System;
using System.Windows.Controls;
using WassupClient.Views;
using System.Linq;

namespace WassupClient
{
    public class Core : INotifyPropertyChanged
	{
		private UserControl _selectedView;
		private User _user;
		private TcpManagerClient tcp;
		private string _error;

		/// <summary>
		/// View's current ViewModel
		/// </summary>
		public UserControl SelectedView
		{
			get { return _selectedView; }
			set
			{
				_selectedView = value;

				// Sets View's height and width to the Window (if ViewModel's height/width != null, else 0)
				//_selectedView.Height = _selectedView?.Height ?? 0;
				//_selectedView.Width = _selectedView?.Width ?? 0;
				//// Resets error
				//_selectedView.Error = string.Empty;
				//// Sets window title
				//_selectedView.Title = _selectedView.Title;

				// Calls OnPropertyChanged
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public User User
		{
			get { return _user; }
			set
			{
				_user = value;
				OnPropertyChanged(nameof(User));
			}
		}

		public string Error
		{
			get { return _error; }
			set
			{
				_error = value;
				OnPropertyChanged(nameof(Error));
			}
		}

		public Core()
		{
			// Creates tcp client
			tcp = new TcpManagerClient();
		}

		public void LoadFirstView()
		{
			// Gets the View's type from the string
			Type viewType = Type.GetType("WassupClient.Views.LoginView");

			// If the View exists
			if (viewType != null)
			{
				// Creates the type instance (same as _selectedView = new [nome]View())
				SelectedView = Activator.CreateInstance(viewType) as UserControl;
			}
			else
			{
				// Throws exception
				throw new ArgumentException($"View not found: Login");
			}
		}

		public void ChangeView(object parameter)
		{
			if (parameter != null)
			{
				// Gets the View's name
				string fullViewName = $"{_selectedView.GetType().Namespace}.{parameter.ToString()}View";

				// Gets the View's type from the string
				Type viewType = Type.GetType(fullViewName);

				// If the View exists
				if (viewType != null)
				{
					// Creates the type instance (same as _selectedView = new [nome]View())
					SelectedView = Activator.CreateInstance(viewType) as UserControl;					
				}
				else
				{
					// Throws exception
					throw new ArgumentException($"View not found: {parameter.ToString()}");
				}
			}
		}

		public User Login(string username, string password)
		{
			Response response = tcp.ExecuteCommand(new Request("login", new { username, password }));

			// If response is not succesful
			if (!response.Result)
				this.Error = "Username o password errati";

			// Gets the user from the response
			return (User)((dynamic)response.Content)?.user;
		}
		public User Register(string username, string password)
		{
			Response response = tcp.ExecuteCommand(new Request("register", new { username, password }));

			// If response is not succesful
			if (!response.Result)
				this.Error = "Utente già registrato";

			// Gets the user from the response
			return (User)response?.Content;
		}
		public void Message(int chatId, string type, string content)
		{
			// Wrapper to send message to server
			tcp.ExecuteCommand(new Request("mess", new { User.Username, chatId, type, content }));
		}

		#region PropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
