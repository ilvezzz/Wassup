using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WassupLib.Models;

namespace WassupClient
{
    public partial class MainWindow : Window
    {
		private Chat _rightClickedChat;

		public MainWindow()
        {
            InitializeComponent();
			this.DataContext = new Core();
        }
        /// <summary>
        /// Loads 1st view (login) as app finished loading
        /// </summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            (this.DataContext as Core).LoadFirstView();
		}
        /// <summary>
        /// Changes view to NewChat
        /// </summary>
        private void btnNewChat_Click(object sender, RoutedEventArgs e)
        {
			if ((this.DataContext as Core).Users != null && (this.DataContext as Core).Users.Count > 0)
			{
				// Changes to NewChatView
				(this.DataContext as Core).ChangeView("NewChat");
			}
		}
		/// <summary>
		/// Selects a Chat
		/// </summary>
		private void SelectChat(object sender, RoutedEventArgs e)
		{
			// On chat left click --> selects chat
			if (sender is Border border && border.DataContext is Chat chat)
			{
				(this.DataContext as Core).SelectChat(chat);
			}
		}
		/// <summary>
		/// Right click on chat
		/// </summary>
		private void RightClickChat(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// On chat right click --> opens popup
			if (sender is Border border && border.DataContext is Chat chat)
			{
				// Saves current chat
				_rightClickedChat = chat;

				// Show popup
				border.ContextMenu.IsOpen = true;
				e.Handled = true;
			}
		}
		/// <summary>
		/// Deletes a Chat
		/// </summary>
		private void DeleteChat(object sender, RoutedEventArgs e)
		{
			if (_rightClickedChat != null)
			{
				(this.DataContext as Core).DeleteChat(_rightClickedChat);
				_rightClickedChat = null;
			}
		}
		/// <summary>
		/// Opens account page
		/// </summary>
		private void OpenAccount(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.Name == "btnAccount" && (this.DataContext as Core).User != null)
			{
				// Changes to account view
				(this.DataContext as Core).ChangeView("Account");
			}
		}
	}
}
