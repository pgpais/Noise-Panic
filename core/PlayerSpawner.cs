using Godot;
using System;
using WeWereDumb.Gameplay.Actors;

[GlobalClass]
public partial class PlayerSpawner : Marker3D
{
	[Signal]
	public delegate void PlayerSpawnedEventHandler(Player player, int playerId);

	[Export] PackedScene playerScene;

	[Export] bool spawnNewPlayers = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnLocalPlayer();
		SpawnPeers();

		if (spawnNewPlayers)
		{
			NetworkManager.Instance.ClientConnected += OnPlayerJoined;
		}
	}

	private void SpawnPeers()
	{
		GD.Print("[Lobby] " + Multiplayer.GetUniqueId() + ": " + "Spawning peers...");
		var peerIds = Multiplayer.GetPeers();
		foreach (var peerId in peerIds)
		{
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Spawning peer " + peerId);
			SpawnPlayer(peerId.ToString());
		}
	}

	private void SpawnLocalPlayer()
	{
		GD.Print("[Lobby] " + Multiplayer.GetUniqueId() + ": " + "Spawning local player...");
		SpawnPlayer(Multiplayer.GetUniqueId().ToString());
	}

	private void SpawnPlayer(string id)
	{
		var player = playerScene.Instantiate<Node3D>();
		player.Name = id;
		player.SetMultiplayerAuthority((int)long.Parse(id));
		AddChild(player);
		player.GlobalPosition = GlobalPosition;

		EmitSignal(SignalName.PlayerSpawned, player, int.Parse(id));
	}

	private void OnPlayerJoined(long id)
	{
		if (Multiplayer.IsServer()) // This check may not be necessary
		{
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Requesting player " + id + " in position " + GlobalPosition);

			SpawnPlayer(id.ToString());
		}
	}
}
