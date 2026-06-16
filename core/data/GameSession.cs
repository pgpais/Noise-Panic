using Godot;
using System;
using System.Collections.Generic;

namespace WeWereDumb.Core.Data
{
	public partial class GameSession : GodotObject
	{
		Dictionary<int, PeerData> _peerData;

		public GameSession()
		{
			_peerData = [];
		}

		public void SetPeerData(int peerId, PeerData peerData)
		{
			_peerData[peerId] = peerData;
		}

		public PeerData GetPlayerInfo(int peerId)
		{
			return _peerData[peerId];
		}

		public Dictionary<string, object> GetData()
		{
			Dictionary<string, object> data = [];
			foreach (var peerId in _peerData.Keys)
			{
				data[peerId.ToString()] = _peerData[peerId].GetData();
			}
			return data;
		}

		public void SetData(Dictionary<string, object> data)
		{
			foreach (var peerId in data.Keys)
			{
				if (_peerData.TryGetValue(int.Parse(peerId), out PeerData peerData))
				{
					peerData.SetData((Dictionary<string, object>)data[peerId]);
				}
			}
		}
	}
}
