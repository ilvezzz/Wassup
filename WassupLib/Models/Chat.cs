using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WassupLib.Models
{
    [Serializable]
    public class Chat
    {
        #region Variables

        private int _id;
        private User _user;
        private ObservableCollection<Message> _messages;


        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
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
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        #endregion

        #region Constructors

        public Chat() { }

        /// <summary>
        /// Coinstructor with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public Chat(User user, ref int lastId)
        {
            _id = ++lastId;
            _user = user;

            _messages = new ObservableCollection<Message>();
        }

        #endregion

        #region Methods

        public void SendMessage(string type, string content)
        {

        }

		public void DeleteMessage(int messageId)
		{

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

