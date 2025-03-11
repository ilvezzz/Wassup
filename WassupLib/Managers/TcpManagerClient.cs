using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WassupLib.Managers
{
    public class TcpManagerClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
		private string _ip;
		private int _port;

		public TcpManagerClient(string ip = "127.0.0.1", int port = 12345)
        {
            // TODO
			_ip = ip;
            _port = port;
        }

        public void Connect()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_ip, _port);
                _stream = _client.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nella connessione: " + ex.Message);
            }
        }

        public void Start()
        {
            // Avvia un thread in background per la ricezione dei messaggi
            Thread thread = new Thread(ReceiveMessages)
            {
                IsBackground = true
            };
            thread.Start();

            MessageBox.Show("Inserisci i messaggi da inviare al server (digita 'exit' per uscire):");

            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                    break;

                // Costruisce un comando JSON con tipo "mess"
                var command = new
                {
                    type = "mess",
                    content = new
                    {
                        chatId = 1,
                        messageType = "text",
                        messageContent = input
                    }
                };

                string jsonCommand = JsonSerializer.Serialize(command);
                SendMessage(jsonCommand);
            }

            // Chiude la connessione
            _stream.Close();
            _client.Close();
        }

        private void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante l'invio del messaggio: " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Connessione chiusa dal server.");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Risposta dal server: " + response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante la ricezione: " + ex.Message);
            }
        }

    }
}
