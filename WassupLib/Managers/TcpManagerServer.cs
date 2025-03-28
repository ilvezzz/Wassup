using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.Json;
using WassupLib.Models;
using System.Dynamic;

namespace WassupLib.Managers
{
    public class TcpManagerServer
    {
        private TcpListener _server;
        private string _ip;
        private int _port;
		private int _bufferSize;
		private int _lastChatId;
        private int _lastMessageId;

        public TcpManagerServer(string ip = "127.0.0.1", int port = 12345)
        {
            _lastChatId = 0;
            _lastMessageId = 0;

            _ip = ip;
            _port = port;
			_bufferSize = 1_073_741_824; //2_147_483_647;

			_server = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));
            
            if (_server == null)
                throw new Exception("Errore durante l'avvio del server TCP");

            _server.Start();

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();

                if (client != null)
                {
					//Console.WriteLine("Client connesso");
					//new Thread(() => HandleClient(client)) { IsBackground = true }.Start();

					NetworkStream stream = client.GetStream();
					var buffer = new byte[_bufferSize];

					try
					{
						int bytesRead = stream.Read(buffer, 0, buffer.Length);

						string jsonRequest = Encoding.ASCII.GetString(buffer, 0, bytesRead);

						//var request = JsonSerializer.Deserialize<Request>(jsonRequest);
						dynamic request = JsonSerializer.Deserialize<ExpandoObject>(jsonRequest);
						object response;

						switch (request.Type.ToString().ToLower())
						{
							case "login":
								// { type:login, content:{ username, password } }
								response = LoginUser(request.Content.GetProperty("username").ToString(), request.Content.GetProperty("password").ToString());
								Respond(stream, response);
								break;
							case "register":
								// { type:register, content:{ username, password } }
								response = RegisterUser(request.Content.GetProperty("username").ToString(), request.Content.GetProperty("password").ToString());
								Respond(stream, response);
								break;
                            case "new-chat":
                                // { type:new-chat, content:{ username1, username2 } }
                                response = NewChat(request.Content.GetProperty("username1").ToString(), request.Content.GetProperty("username2").ToString());
                                Respond(stream, response);
								break;
							case "del-chat":
								// { type:del-chat, content:{ username1, username2 } }
								DeleteChat(request.Content.GetProperty("username1").ToString(), request.Content.GetProperty("username2").ToString());
								break;
							case "new-mess":
								// { type:new-mess, content:{ username1, username2, type, content } }
								response = SendMessage(request.Content.GetProperty("username1").ToString(), request.Content.GetProperty("username2").ToString(), request.Content.GetProperty("type").ToString(), request.Content.GetProperty("content").ToString());
								Respond(stream, response);
								break;
							case "del-mess":
								// { type:del-mess, content:{ username1, username2, id } }
								DeleteMessage(request.Content.GetProperty("username1").ToString(), request.Content.GetProperty("username2").ToString(), Convert.ToInt32(request.Content.GetProperty("id")));
								break;
							case "edit-profile":
								// { type:edit-profile, content:{ username, password, img } }
								response = EditProfile(request.Content.GetProperty("oldUsername").ToString(), request.Content.GetProperty("newUsername").ToString(), request.Content.GetProperty("newPassword").ToString(), request.Content.GetProperty("newImg").ToString());
								Respond(stream, response);
								break;
							default:
								Respond(stream, new { error = $"Tipo richiesta '{request.Type.ToString().ToLower()}' invalido." });
								break;
						}
					}
					catch (Exception ex)
					{
						//Console.WriteLine($"Errore nel client: {ex.Message}");
					}
					finally
					{
						client.Close();
						//Console.WriteLine("Client disconnesso");
					}
				}
            }
        }

        private void Respond(NetworkStream stream, object response)
        {
            try
            {
				// Serializes into a json string
				var jsonResponse = JsonSerializer.Serialize(response);
				// Sends message in stream
				byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
				stream.Write(responseBytes, 0, responseBytes.Length);
			}
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Gets username and password & logs in user
        /// </summary>
        /// <returns>True if login succesful, otherwise false</returns>
        public Response LoginUser(string username, string password)
        {
			// Get all users
			var users = FileManager.GetUsers();
			// Gets the user
			var user = users.Find(x => x.Username == username && x.Password == password);
			if (user != null)
			{
				// Get user's chats
				var chats = FileManager.GetChats().Where(x => x.Username1.Equals(username) || x.Username2.Equals(username)).ToList();
				// Returns result & user if found
				return new Response(true, new { user, users, chats });
			}
			// Returns user not found
			return new Response(false, null);
		}
        /// <summary>
        /// Gets username and password & registers user
        /// </summary>
        /// <returns>True if register succesful, otherwise false</returns>
        public Response RegisterUser(string username, string password)
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
                FileManager.UpdateUsers(users);
                // Returns result
                return new Response(true, new { user, users });
            }
            else
                return new Response(false, null);

		}
        public Response NewChat(string username1, string username2)
        {
			// Gets users & chats
			var users = FileManager.GetUsers();
			var chats = FileManager.GetChats();

			// Conds
			bool bothUsersExist = users.Any(x => x.Username.Equals(username1)) && users.Any(x => x.Username.Equals(username2));
			bool chatBetweenThemExists = chats.Any(x => (x.Username1.Equals(username1) && x.Username2.Equals(username2)) || (x.Username1.Equals(username2) && x.Username2.Equals(username1)));

			// If users exist && chat between them doesnt --> create chat
			if (bothUsersExist && !chatBetweenThemExists)
            {
                FileManager.AddChat(new Chat(username1, username2));
                return new Response(true, null);
            }

            return new Response(false, null);
		}
		public Response DeleteChat(string username1, string username2)
        {
            var chats = FileManager.GetChats();
			var _ = chats.Find(x => (x.Username1.Equals(username1) && x.Username2.Equals(username2)) || (x.Username1.Equals(username2) && x.Username2.Equals(username1)));
            // If found chat
            if (_ != null)
            {
                // Deletes it
                chats.Remove(_);
                FileManager.UpdateChats(chats);
				return new Response(true, null);
			}

			return new Response(false, null);
		}
		/// <summary>
		/// Sends the message to the specified chat
		/// </summary>
		/// <param name="username1">Username of the sender in chat</param>
		/// <param name="username2">Username of the reciever in chat</param>
		/// <param name="type">Message type: text or image</param>
		/// <param name="content">Message content: text or base64 string?</param>
		public Response SendMessage(string username1, string username2, string type, string content)
        {
			// Gets chat index
			var chats = FileManager.GetChats();
			var i = chats.FindIndex(x => (x.Username1.Equals(username1) && x.Username2.Equals(username2)) || (x.Username1.Equals(username2) && x.Username2.Equals(username1)));

			// If found chat
			if (i != -1)
			{
                // Sends message to chat
                chats.ElementAt(i).SendMessage(username1, type, content);
				// Updates the db
				FileManager.UpdateChats(chats);
				return new Response(true, new { id = chats.ElementAt(i).GetLastMessageId() });
			}

			return new Response(false, null);
		}
        public Response DeleteMessage(string username1, string username2, int messId)
        {
            // Gets chat index
			var chats = FileManager.GetChats();
			var i = chats.FindIndex(x => (x.Username1.Equals(username1) && x.Username2.Equals(username2)) || (x.Username1.Equals(username2) && x.Username2.Equals(username1)));
			// If found chat
			if (i != -1)
			{
				// Gets message
				var _mess = chats.ElementAt(i).Messages.ToList().Find(x => x.Id == messId);
				// If found message
				if (_mess != null)
				{
					// Deletes it
					chats.ElementAt(i).Messages.Remove(_mess);
					FileManager.UpdateChats(chats);
					return new Response(true, null);
				}
			}

			return new Response(false, null);
		}
		public Response EditProfile(string oldUsername, string newUsername, string newPassword, string newImg)
		{
			// Gets users & chats
			var users = FileManager.GetUsers();
			var chats = FileManager.GetChats();

			var i = users.FindIndex(x => x.Username.Equals(oldUsername));
			if (i != -1)
			{
				// Update username
				if (!string.IsNullOrEmpty(newUsername))
				{
					users.ElementAt(i).Username = newUsername;

					foreach (var chat in chats)
					{
						if (chat.Username1.Equals(oldUsername))
							chat.Username1 = newUsername;
						if (chat.Username2.Equals(oldUsername))
							chat.Username2 = newUsername;
					}

					FileManager.UpdateChats(chats);
				}
				// Update password
				if (!string.IsNullOrEmpty(newPassword))
				{
					users.ElementAt(i).Password = newPassword;
				}
				// Update img
				if (!string.IsNullOrEmpty(newImg))
				{
					users.ElementAt(i).Image = newImg;
				}

				FileManager.UpdateUsers(users);

				return new Response(true, new { user = users.ElementAt(i), userChats = chats.Where(x => x.Username1.Equals(newUsername) || x.Username2.Equals(newUsername)).ToList() });
			}

			return new Response(false, null);
		}
    }
}
