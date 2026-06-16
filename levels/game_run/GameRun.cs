using Godot;
using System;

public partial class GameRun : Node3D
{
	[Export] PackedScene playerScene;
	[Export] Node3D spawnPoint;
	[Export] Node3D ship;

	[ExportGroup("Run Paremeters")]
	[Export] Timer countdownTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnLocalPlayer();
		SpawnPeers();

		if (IsMultiplayerAuthority())
		{
			StartCountdown();
		}
	}

	[Rpc(CallLocal = true)]
	public void StartCountdown()
	{
		countdownTimer.Start();
	}

	private void SpawnLocalPlayer()
	{
		SpawnPlayer(Multiplayer.GetUniqueId().ToString(), spawnPoint.GlobalPosition);
	}

	private void SpawnPeers()
	{
		var peerIds = Multiplayer.GetPeers();
		foreach (var peerId in peerIds)
		{
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Spawning peer " + peerId);
			SpawnPlayer(peerId.ToString(), spawnPoint.GlobalPosition);
		}
	}

	private void SpawnPlayer(string id, Vector3 spawnPos)
	{
		var player = playerScene.Instantiate<Node3D>();
		player.Name = id;
		player.SetMultiplayerAuthority((int)long.Parse(id));
		AddChild(player);
		player.GlobalPosition = spawnPos;

		player.Reparent(ship);
	}
}
