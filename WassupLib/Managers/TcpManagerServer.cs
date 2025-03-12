using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.RightsManagement;
using System.Text.Json;
using WassupLib.Models;

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
                    object response;

                    switch (request.Type.ToLower())
                    {
                        case "login":
							// { type:login, content:{ username, password } }
							response = LoginUser(((dynamic)request.Content)?.username, ((dynamic)request.Content)?.password);
							Respond(stream, response);
							break;
                        case "register":
                            // { type:register, content:{ username, password } }
                            response = RegisterUser(((dynamic)request.Content)?.username, ((dynamic)request.Content)?.password);
							Respond(stream, response);
							break;
                        case "mess":
                            // { type:mess, content:{ username, chat, type, content } }
                            SendMessage(((dynamic)request.Content)?.username, ((dynamic)request.Content)?.chatId, ((dynamic)request.Content)?.messageType, ((dynamic)request.Content)?.messageContent);
                            break;
                        default:
							Respond(stream, new { error = $"Tipo richiesta '{request.Type.ToLower()}' invalido." });
							break;
                    }

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

        private void Respond(NetworkStream stream, object response)
        {
            // Serializes into a json string
            var jsonResponse = JsonSerializer.Serialize(response);
            // Sends message in stream
            byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        /// <summary>
        /// Gets username and password & logs in user
        /// </summary>
        /// <returns>True if login succesful, otherwise false</returns>
        public object LoginUser(string username, string password)
        {
			// Gets the user
			var user = FileManager.GetUsers().Find(x => x.Username == username && x.Password == password);
            // Returns result & user if found
            if (user != null) 
				return new { result = true, user = user };
			else
				return new { result = false };
		}
        /// <summary>
        /// Gets username and password & registers user
        /// </summary>
        /// <returns>True if register succesful, otherwise false</returns>
        public object RegisterUser(string username, string password)
        {
            // Gets users
            var users = FileManager.GetUsers();
			// Gets the user
			var user = users.Find(x => x.Username == username);

            // If user isn't registered yet
            if (user == null)
            {
                // Creates the user
                user = new User(username, password);
				// Adds the user to the list
				users.Add(user);
				// Updates the db
				FileManager.UpdateDb(users);
				// Returns result
				return new { result = true, user = user };
			}
            else
                return new { result = false };

		}
        /// <summary>
        /// Sends the message to the specified chat
        /// </summary>
        /// <param name="chatId">Id of the chat to send the message to</param>
        /// <param name="type">Message type: text or image</param>
        /// <param name="content">Message content: text or base64 string?</param>
        public void SendMessage(string username, int chatId, string type, string content)
        {
			// Gets users
			var users = FileManager.GetUsers();
			// Gets the user
			var user = users.Find(x => x.Username == username);

			// If user isn't registered yet
			if (user != null)
			{
                // Sends message to chat
                user.Chats.ToList().Find(x => x.Id == chatId).SendMessage(type, content);
				// Updates the db
				FileManager.UpdateDb(users);
			}
		}

    }
}
