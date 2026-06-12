using Godot;
using System;
using System.Collections.Generic;

namespace WeWereDumb.Core.Data
{
	public partial class PeerData : GodotObject
	{
		public string SteamId { get; set; }
		public string Username { get; set; }

		public PeerData()
		{
			SteamId = "";
			Username = "Player";
		}

		public Dictionary<string, object> GetData()
		{
			return new Dictionary<string, object>
			{
				{ "SteamId", SteamId },
				{ "Username", Username },
			};
		}

		public void SetData(Dictionary<string, object> data)
		{
			if (data.TryGetValue("SteamId", out object steamId))
			{
				SteamId = steamId.ToString();
			}
			if (data.TryGetValue("Username", out object username))
			{
				Username = username.ToString();
			}
		}
	}
}
