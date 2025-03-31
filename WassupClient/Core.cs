using System.ComponentModel;
using WassupLib.Models;
using WassupLib.Managers;
using System;
using System.Windows.Controls;
using WassupClient.Views;
using System.Linq;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WassupClient
{
    public class Core : INotifyPropertyChanged
	{
		private UserControl _selectedView;

		private ObservableCollection<User> _users;
		private ObservableCollection<Chat> _chats;
		private User _user;
		private Chat _selectedChat;

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

		public ObservableCollection<User> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(nameof(User));
				OnPropertyChanged(nameof(Users));
			}
		}
		public ObservableCollection<Chat> UserChats
		{
			get { return _chats; }
			set
			{
				foreach (var chat in value)
					chat.OtherUser = this.Users.ToList().Find(x => x.Username.Equals(chat.GetOtherUsername(this.User.Username)));

				_chats = value;
				OnPropertyChanged(nameof(SelectedChat));
				OnPropertyChanged(nameof(UserChats));
			}
		}
		public User User
		{
			get { return _user; }
			set
			{
				_user = value;
				OnPropertyChanged(nameof(User));
				OnPropertyChanged(nameof(Users));
			}
		}
		public Chat SelectedChat
		{
			get { return _selectedChat; }
			set
			{
				// Deselect selected chat instance
				if (_selectedChat != null)
					_selectedChat.IsSelected = false;
				
				_selectedChat = value;

				// Set selected chat as selected
				if (_selectedChat != null)
					_selectedChat.IsSelected = true;

				OnPropertyChanged(nameof(SelectedChat));
				OnPropertyChanged(nameof(UserChats));
			}
		}
		public ObservableCollection<User> AvailableUsers
		{
			get 
			{
				// Gets the available users (still without chat with current one)
				var chattedUsers = UserChats.Select(x => User.Username.Equals(x.Username1) ? x.Username2 : x.Username1).ToList();
				var availableUsers = Users.Where(x => !chattedUsers.Contains(x.Username));
				// Returns it as observableCollection
				if (availableUsers.Count() > 0)
					return new ObservableCollection<User>(availableUsers);
				else 
					return new ObservableCollection<User>();
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
			try
			{
				// Creates tcp client
				tcp = new TcpManagerClient();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Impossibile connettersi al server, verificare che sia acceso.\r\n\r\n" + ex.Message, "Errore client", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(0);
			}
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

		public void SelectChat(Chat chat)
		{
			// Deselects all other chats
			foreach (var c in UserChats)
				c.IsSelected = false;

			SelectedChat = chat;
		}

		public dynamic Login(string username, string password)
		{
			Response response = tcp.ExecuteCommand(new Request("login", new { username = username, password = password }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Username o password errati";
				return null;
			}

			//User user = JsonSerializer.Deserialize<User>(response.Content.ToString());
			return JsonSerializer.Deserialize<ExpandoObject>(response.Content.ToString());
		}
		public dynamic Register(string username, string password)
		{
			Response response = tcp.ExecuteCommand(new Request("register", new { username = username, password = password }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Utente già registrato";
				return null;
			}

			//User user = JsonSerializer.Deserialize<User>(response.Content.ToString());
			return JsonSerializer.Deserialize<ExpandoObject>(response.Content.ToString());
		}
		public void EditProfile(string newUsername, string newPassword, string newImg)
		{
			Response response = tcp.ExecuteCommand(new Request("edit-profile", new { oldUsername = User.Username, newUsername, newPassword, newImg }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Errore in modifica profilo";
				ChangeView("Home");
				return;
			}

			dynamic res = JsonSerializer.Deserialize<ExpandoObject>(response.Content.ToString());

			if (res != null)
			{
				User user = JsonSerializer.Deserialize<User>(res.user);
				List<Chat> userChats = JsonSerializer.Deserialize<List<Chat>>(res.userChats);

				Application.Current.Dispatcher.Invoke(() =>
				{
					User = user;
					UserChats = new ObservableCollection<Chat>(userChats);

					OnPropertyChanged(nameof(User));
					OnPropertyChanged(nameof(UserChats));
				});
			}

			ChangeView("Home");
		}
		public void CreateChat(string username)
		{
			// Wrapper to send message to server
			Response response = tcp.ExecuteCommand(new Request("new-chat", new { username1 = User.Username, username2 = username }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Errore creazione chat";
				return;
			}

			//dynamic res = JsonSerializer.Deserialize<ExpandoObject>(response.Content.ToString());

			Application.Current.Dispatcher.Invoke(() =>
			{
				var _ = new Chat(User.Username, username);
				_.OtherUser = Users.ToList().Find(x => x.Username.Equals(username));
				UserChats.Add(_);
			});

			OnPropertyChanged(nameof(UserChats));
		}
		public void DeleteChat(Chat chat)
		{
			// Wrapper to send message to server
			Response response = tcp.ExecuteCommand(new Request("del-chat", new { username1 = User.Username, username2 = chat.OtherUser.Username }));

			// If response is not succesful
			if (!response.Result)
				this.Error = "Errore cancellazione chat";

			// Removes chat
			UserChats.Remove(chat);
			// If selected chat == chat to delete
			if (SelectedChat != null && SelectedChat.Username1.Equals(chat.Username1) && SelectedChat.Username2.Equals(chat.Username2))
			{
				// Reloads view without selected chat ?
				SelectedChat = null;
			}

			OnPropertyChanged(nameof(SelectedChat));
			OnPropertyChanged(nameof(UserChats));
		}
		public void SendMessage(string type, string content)
		{
			// Wrapper to send message to server
			Response response = tcp.ExecuteCommand(new Request("new-mess", new { username1 = User.Username, username2 = SelectedChat.OtherUser.Username, type = type, content = content }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Errore invio messaggio";
				return;
			}

			dynamic res = JsonSerializer.Deserialize<ExpandoObject>(response.Content.ToString());

			if (res != null)
			{
				int id = JsonSerializer.Deserialize<int>(res.id);

				Application.Current.Dispatcher.Invoke(() =>
				{
					SelectedChat.Messages.Add(new Message(Convert.ToInt32(id), User.Username, type, content));
				});

				OnPropertyChanged(nameof(UserChats));
				OnPropertyChanged(nameof(SelectedChat.Messages));
				OnPropertyChanged(nameof(SelectedChat.LastMessage));
			}
		}
		public void DeleteMessage(int messageId)
		{
			// Wrapper to send message to server
			Response response = tcp.ExecuteCommand(new Request("del-mess", new { username1 = User.Username, username2 = SelectedChat.OtherUser.Username, id = messageId }));

			// If response is not succesful
			if (!response.Result)
			{
				this.Error = "Errore cancellazione messaggio";
				return;
			}

			// Removes LOCAL message if found
			var _ = SelectedChat.Messages.ToList().Find(x => x.Id == messageId);
			if (_ != null)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					SelectedChat.Messages.ToList().Remove(_);
				});

				OnPropertyChanged(nameof(SelectedChat.Messages));
				OnPropertyChanged(nameof(SelectedChat.LastMessage));
			}
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
