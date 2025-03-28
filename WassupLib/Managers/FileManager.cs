using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WassupLib.Models;

namespace WassupLib.Managers
{
    public static class FileManager
    {
		private static readonly string users_path = Path.Combine(Directory.GetCurrentDirectory(), "users.json");
		private static readonly string chats_path = Path.Combine(Directory.GetCurrentDirectory(), "chats.json");

        // Json Serializer config
		private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions() {
			WriteIndented = true,
			//ReferenceHandler = ReferenceHandler.Preserve, // x riferimenti circolari
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		#region Users

		/// <summary>
		/// Adds an user to the json archive
		/// </summary>
		/// <param name="user">User to add</param>
		public static void AddUser(User user)
		{
			try
			{
				var users = GetUsers();
				users.Add(user);
				UpdateUsers(users);
			}
			catch
			{
				// Log
			}
		}

		/// <summary>
		/// Gets user data from the json archive
		/// </summary>
		/// <returns>List of User</returns>
		public static List<User> GetUsers()
		{
			if (!File.Exists(users_path))
			{
				var newList = new List<User>();
				File.WriteAllText(users_path, JsonSerializer.Serialize(newList, jsonOptions));
				return newList;
			}

			return JsonSerializer.Deserialize<List<User>>(File.ReadAllText(users_path), jsonOptions) ?? new List<User>();
		}

		/// <summary>
		/// Updates users list in the json archive
		/// </summary>
		/// <param name="list">Users to update</param>
		public static void UpdateUsers(List<User> list)
		{
			try
			{
				string json = JsonSerializer.Serialize(list, jsonOptions);
				File.WriteAllText(users_path, json);
			}
			catch
			{
				// Log
			}
		}

		#endregion

		#region Chats

		/// <summary>
		/// Gets chat data from the json archive
		/// </summary>
		/// <returns>List of Chat</returns>
		public static List<Chat> GetChats()
		{
			if (!File.Exists(chats_path))
			{
				var newList = new List<Chat>();
				File.WriteAllText(chats_path, JsonSerializer.Serialize(newList, jsonOptions));
				return newList;
			}

			return JsonSerializer.Deserialize<List<Chat>>(File.ReadAllText(chats_path), jsonOptions) ?? new List<Chat>();
		}
		/// <summary>
		/// Updates chats list in the json archive
		/// </summary>
		/// <param name="list">Chats to update</param>
		public static void UpdateChats(List<Chat> list)
		{
			try
			{
				string json = JsonSerializer.Serialize(list, jsonOptions);
				File.WriteAllText(chats_path, json);
			}
			catch
			{
				// Log
			}
		}
		/// <summary>
		/// Adds a chat to the json archive
		/// </summary>
		/// <param name="chat">Chat to add</param>
		public static void AddChat(Chat chat)
		{
			try
			{
				var chats = GetChats();
				chats.Add(chat);
				UpdateChats(chats);
			}
			catch
			{
				// Log
			}
		}

		#endregion
	}
}
