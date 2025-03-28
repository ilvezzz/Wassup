using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WassupLib.Models
{
    [Serializable]
    public class Message : INotifyPropertyChanged
    {
        private int _id;
        private string _sender; 
        private string _type;
        private object _content;

		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}
		public string Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                OnPropertyChanged(nameof(Sender));
            }
        }
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public object Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public Message(int id, string sender, string type, object content)
        {
            _id = id;
            _sender = sender;
            _type = type;
            _content = content;
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
