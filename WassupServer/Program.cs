using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WassupServer
{
    internal class Program
    {
        private TcpListener _server;
        private string _ip;
        private int _port;

        public TcpManagerServer(string ip = "127.0.0.1", int port = 12345)
        {
            _ip = ip;
            _port = port;

            _server = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));

            if (_server == null)
                throw new Exception("Errore durante l'avvio del server TCP");

            _server.Start();

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();

                if (client != null)
                    new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    string jsonRequest = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    var request = JsonSerializer.Deserialize<ClientRequest>(receivedMessage);

                    switch (request.Type.ToLower())
                    {
                        case "login":
                            return LoginUser(request.Username, request.Password);
                        case "register":
                            return RegisterUser(request.Username, request.Password);
                        case "chat":
                            return GetChat(request.ChatId);
                        case "mess":
                            return SendMessage(request.ChatId, request.MessageType, request.MessageContent);
                        default:
                            return "Comando non riconosciuto";
                    }

                    string response = "";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel client: {ex.Message}");
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public List<Chat> Login()
        {

        }
        public bool Register()
        {

        }
        public List<Message> LoadChat()
        {

        }
        public bool GetMessage()
        {

        }
        public void SendMessage()
        {

        }

    }
}
