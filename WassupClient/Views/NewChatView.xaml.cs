using System.Windows;
using System.Windows.Controls;
using WassupLib.Models;

namespace WassupClient.Views
{
	/// <summary>
	/// Logica di interazione per NewChatView.xaml
	/// </summary>
	public partial class NewChatView : UserControl
	{
		public NewChatView()
		{
			InitializeComponent();
			DataContext = Application.Current.MainWindow.DataContext;
		}
		/// <summary>
		/// Creates chat in user & updates db
		/// </summary>
		private void CreateChat(object sender, RoutedEventArgs e)
		{
			// If user doesnt already have chat with everyone
			if ((this.DataContext as Core).AvailableUsers.Count > 0)
				(this.DataContext as Core).CreateChat((cbUsers.SelectedItem as User).Username);
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
