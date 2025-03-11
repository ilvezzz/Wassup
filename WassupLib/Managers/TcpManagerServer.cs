using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WassupLib.Models;
using System.Security.RightsManagement;
using System.Text.Json;

namespace WassupLib.Managers
{
    public class TcpManagerServer
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

                    var request = JsonSerializer.Deserialize<Command>(jsonRequest);
                    string response = "";

                    switch (request.Type.ToLower())
                    {
                        case "login":
                            // { type:login, content:{ username, password } }
                            response = LoginUser(request.Content?.username, request.Content?.password);
                            break;
                        case "register":
                            // { type:login, content:{ username, password } }
                            response = RegisterUser(request.Content?.username, request.Content?.password);
                            break;
                        case "chat":
                            // { type:chat, content:{ chatid } } 
                            response = LoadChat(request.Content?.chatId);
                            break;
                        case "mess":
                            // { type:mess, content:{ chat, type, content } }
                            response = SendMessage(request.Content?.chatId, request.Content?.messageType, request.Content?.messageContent);
                            break;
                        default:
                            response = "KO";
                            break;
                    }

                    Respond(stream, response);

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

        private void Respond(NetworkStream stream, string response)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        public bool LoginUser(string username, string password)
        {

        }
        public bool RegisterUser(string username, string password)
        {

        }
        public Chat LoadChat(int chatId)
        {

        }
        public void SendMessage(int chatId, string type, string content)
        {

        }

    }
}
