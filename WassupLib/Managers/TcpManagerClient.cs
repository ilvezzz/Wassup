using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using WassupLib.Models;

namespace WassupLib.Managers
{
    public class TcpManagerClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
		private string _ip;
		private int _port;

        public bool Connected { get; set; }

		public TcpManagerClient(string ip = "127.0.0.1", int port = 12345)
        {
			// Prevents client from starting before server (hopefully)
			Thread.Sleep(500);

			_ip = ip;
            _port = port;

            _client = new TcpClient();
			_client.Connect(_ip, _port);
			_stream = _client.GetStream();

			if (_client == null || _stream == null)
				throw new Exception("Errore durante la creazione del client TCP");
		}

		public Response ExecuteCommand(Request req, bool expectResponse = true)
		{
			// Serializes req into a json string
			var jsonRequest = JsonSerializer.Serialize(req);
			// Sends message in stream
			byte[] data = Encoding.ASCII.GetBytes(jsonRequest);
			_stream.Write(data, 0, data.Length);

			// If expects response, gets it
			if (expectResponse)
				return GetResponse();

			return null;
		}

		private Response GetResponse()
		{
			var receivedData = new List<byte>();
			var buffer = new byte[1024];

			while (true)
			{
				// Read bytes
				int bytesRead = _stream.Read(buffer, 0, 1024);
				// No more bytes = message ended
				if (bytesRead == 0) break;
				// Adds bytes to list
				receivedData.AddRange(buffer.Take(bytesRead));
			}

			// No data received
			if (receivedData.Count == 0)
				return new Response(false, null);

			// Bytes to json string
			string jsonResponse = Encoding.ASCII.GetString(receivedData.ToArray());

			// Parses json response into an object & returns it
			return JsonSerializer.Deserialize<Response>(jsonResponse);
		}

		//public void SendMessage(string username, int chatId, string type, string message)
		//{
		//	// Serializes into a json string
		//	var jsonCommand = JsonSerializer.Serialize(new Command("mess", new { username = username, chatId = chatId, messageType = type, messageContent = message }));
		//	// Sends message in stream
		//	byte[] data = Encoding.ASCII.GetBytes(jsonCommand);
		//	_stream.Write(data, 0, data.Length);
		//}
    }
}
