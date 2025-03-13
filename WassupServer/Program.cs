using WassupLib.Managers;

namespace WassupServer
{
    public class Program
    {
        public static void Main()
        {
            var server = new TcpManagerServer();
        }
    }
}
