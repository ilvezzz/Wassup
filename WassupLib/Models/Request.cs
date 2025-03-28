using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WassupLib.Models
{
    [Serializable]
    public class Request
    {
        public string Type { get; set; }
        public object Content { get; set; }
        public Request(string type, object content)
        {
            Type = type;
            Content = content;
        }
    }
}
