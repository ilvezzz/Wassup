using System;
using System.Collections.Generic;
using System.Dynamic;
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
		private int _bufferSize;

		public TcpManagerClient(string ip = "127.0.0.1", int port = 12345)
        {
			// Prevents client from starting before server (hopefully)
			Thread.Sleep(500);

			_ip = ip;
            _port = port;
			_bufferSize = 1_073_741_824; //2_147_483_647;

			Connect();

			if (!_client.Connected || _stream == null)
				throw new Exception("Errore durante la connessione al server TCP");
		}

		public void Connect()
		{
			if (_client != null && _client.Connected)
				return;

			_client = new TcpClient();
			_client.Connect(_ip, _port);
			_stream = _client.GetStream();

			if (!_client.Connected || _stream == null)
				throw new Exception("Errore durante la connessione al server TCP");
		}

		public Response ExecuteCommand(Request req, bool expectResponse = true)
		{
			if (!_client.Connected)
				Connect();

			// Serializes req into a json string
			var jsonRequest = JsonSerializer.Serialize(req);
			// Sends message in stream
			byte[] data = Encoding.ASCII.GetBytes(jsonRequest);
			_stream.Write(data, 0, data.Length);

			Response res = null;

			// If expects response, gets it
			if (expectResponse)
				res = GetResponse();

			_stream.Close();
			_client.Close();

			return res;
		}

		private Response GetResponse()
		{
			//if (!_client.Connected)
			//	Connect();

			var receivedData = new List<byte>();
			var buffer = new byte[_bufferSize];

			int bytesRead = _stream.Read(buffer, 0, _bufferSize);

			while (bytesRead != 0)
			{
				// Adds bytes to list
				receivedData.AddRange(buffer.Take(bytesRead));

				// Read bytes
				bytesRead = _stream.Read(buffer, 0, _bufferSize);
			}

			// No data received
			if (receivedData.Count == 0)
				return new Response(false, null);

			// Bytes to json string
			string jsonResponse = Encoding.ASCII.GetString(receivedData.ToArray());

			// Parses json response into an object & returns it
			Response response = JsonSerializer.Deserialize<Response>(jsonResponse);

			return new Response(response.Result, response.Content);

		}
    }
}
