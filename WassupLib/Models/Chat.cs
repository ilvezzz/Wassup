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

        private int _chatId;
        private User _user;
        private ObservableCollection<Message> _messages;


        #endregion

        #region Properties
        public int ChatId
        {
            get { return _chatId; }
            set
            {
                _chatId = value;
                OnPropertyChanged(nameof(ChatId));
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
            _chatId = ++lastId;
            _user = user;

            _messages = new ObservableCollection<Message>();
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

