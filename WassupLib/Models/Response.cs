using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WassupLib.Models
{
    [Serializable]
    public class Response
    {
        public bool Result { get; set; }
        public object Content { get; set; }
        public Response(bool result, object content)
        {
            Result = result;
            Content = content;
        }
    }
}
