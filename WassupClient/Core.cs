using System.ComponentModel;
using WassupLib.Models;
using WassupLib.Managers;
using WassupClient.ViewModels;

namespace WassupClient
{
    public class Core : INotifyPropertyChanged
	{
		private BaseViewModel _selectedViewModel;
		private User _user;
		private TcpManagerClient tcp;

		/// <summary>
		/// View's current ViewModel
		/// </summary>
		public BaseViewModel SelectedViewModel
		{
			get { return _selectedViewModel; }
			set
			{
				_selectedViewModel = value;

				// Sets ViewModel's height and width to the Window (if ViewModel's height/width != null, else 0)
				_selectedViewModel.Height = _selectedViewModel?.Height ?? 0;
				_selectedViewModel.Width = _selectedViewModel?.Width ?? 0;
				// Resets error
				_selectedViewModel.Error = string.Empty;
				// Sets window title
				_selectedViewModel.Title = _selectedViewModel.Title;

				// Calls OnPropertyChanged
				OnPropertyChanged(nameof(SelectedViewModel));
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

		public Core()
		{
			//UpdateViewCommand = new UpdateViewCommand(this);

			// Instantiates the selected ViewModel and sets Login ViewModel as start
			SelectedViewModel = new LoginViewModel();
		}

		public void ChangeView(object parameter)
		{
			if (parameter != null)
			{
				// Gets the ViewModel's name
				string fullViewModelName = $"{_selectedViewModel.GetType().Namespace}.{parameter.ToString()}ViewModel";

				// Gets the ViewModel's type from the string
				Type viewModelType = Type.GetType(fullViewModelName);

				// If the ViewModel exists
				if (viewModelType != null)
				{
					// Creates the type instance (same as _selectedViewModel = new [nome]ViewModel())
					_selectedViewModel = Activator.CreateInstance(viewModelType) as BaseViewModel;

					// Creates a new instance of a client TCP manager + 
					tcp = new TcpManagerClient();
					client.Connect();
					client.Start();
				}
				else
				{
					// Throws exception
					throw new ArgumentException($"ViewModel not found: {parameter.ToString()}");
				}
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
