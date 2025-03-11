using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WassupLib.Models
{
    [Serializable]
    public class Command
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public Command(string type, string content)
        {
            Type = type;
            Content = content;
        }
    }
}
