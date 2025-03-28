using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace WassupLib.Models
{
    [Serializable]
    public class Chat : INotifyPropertyChanged
    {
        #region Variables

        //private int _id;
        private string _username1;
        private string _username2;
        private User _otherUser;
        private bool _isSelected;
        private ObservableCollection<Message> _messages;

        #endregion

        #region Properties

        //public int Id
        //{
        //    get { return _id; }
        //    set
        //    {
        //        _id = value;
        //        OnPropertyChanged(nameof(Id));
        //    }
        //}
        public string Username1
        {
            get { return _username1; }
            set
            {
				_username1 = value;
                OnPropertyChanged(nameof(Username1));
            }
        }
        public string Username2
        {
            get { return _username2; }
            set
            {
				_username2 = value;
                OnPropertyChanged(nameof(Username2));
            }
        }
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                // Removes handler (if there's one)
				if (_messages != null)
					_messages.CollectionChanged -= Messages_CollectionChanged;

				_messages = value;

                // Re-adds event handler
				if (_messages != null)
					_messages.CollectionChanged += Messages_CollectionChanged;

				OnPropertyChanged(nameof(Messages));
				OnPropertyChanged(nameof(LastMessage));
			}
        }
		private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(Messages));
			OnPropertyChanged(nameof(LastMessage));
		}
		[JsonIgnore]
		public string LastMessage
        {
            get
            {
                // No mess --> ""
                if (_messages.Count == 0)
                    return string.Empty;
                // String mess --> ellipsed preview
                else if (_messages.Last().Type == "text")
                {
                    var _ = _messages.Last().Content.ToString();
                    if (_.Length > 50)
                        return _.Substring(0, 50) + "...";
                    else
                        return _;
                }
                // Image mess --> "Image"
                else
                    return "Image";
            }
        }
		[JsonIgnore]
		public User OtherUser
		{
            get { return _otherUser; }
            set
            {
				_otherUser = value;
                OnPropertyChanged(nameof(OtherUser));
            }
		}
        [JsonIgnore]
		public bool IsSelected
		{
            get { return _isSelected; }
            set
            {
				_isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
		}

		#endregion

		#region Constructors

		public Chat() { }

		/// <summary>
		/// Constructor with username & chatId
		/// </summary>
		/// <param name="username1">Username of 1st user</param>
		/// <param name="username2">Username of 2nd user</param>
		public Chat(string username1, string username2)
        {
            //_id = ++lastId;
            Username1 = username1;
            Username2 = username2;
            _messages = new ObservableCollection<Message>();
			_messages.CollectionChanged += Messages_CollectionChanged;
		}

        #endregion

        #region Methods

        public void SendMessage(string sender, string type, string content)
        {
            this.Messages.Add(new Message(this._messages.Count + 1, sender, type, content));
        }
        public int GetLastMessageId()
        {
            return Messages.Count > 0 ? Messages.Last().Id : -1; 
        }
		public void DeleteMessage(int messageId)
		{

		}
        /// <summary>
        /// Returns the other chat's user
        /// </summary>
        /// <param name="username">Username of who calls the method</param>
        /// <returns>The other user's username</returns>
        public string GetOtherUsername(string username)
        {
			if (username == this._username1)
                return Username2;
			else if (username == this._username2)
                return Username1;
            else
                return null;
		}

		#endregion

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

