using System;
using System.Linq;
using Godot;
using Godot.Collections;
using WeWereDumb.Gameplay.Actors;

/// <summary>
/// The Lobby class is responsible for managing the lobby scene.
///
/// </summary>
public partial class Lobby : Node3D
{
	[Signal] public delegate void GameStartRequestEventHandler();

	[Export] Node3D spawnPos;
	[Export] PackedScene playerScene;
	[Export] Array<PlayerDetectionArea> playerDetectionAreas;

	[Export] PackedScene gameScene;

	Array<Node> lobbyPlayers = new Array<Node>();
	Array<Player> readyPlayers = new Array<Player>();

	public override void _Ready()
	{
		SpawnLocalPlayer();
		SpawnPeers();

		GD.Print(Multiplayer.GetUniqueId() + ": " + "Lobby ready");

		NetworkManager.Instance.ClientConnected += OnPlayerJoined;

		foreach (var area in playerDetectionAreas)
		{
			area.PlayerEnteredArea += OnPlayerReady;
			area.PlayerExitedArea += OnPlayerUnready;
		}
	}

	private void SpawnLocalPlayer()
	{
		SpawnPlayer(Multiplayer.GetUniqueId().ToString(), spawnPos.GlobalPosition);
	}

	private void SpawnPeers()
	{
		var peerIds = Multiplayer.GetPeers();
		foreach (var peerId in peerIds)
		{
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Spawning peer " + peerId);
			SpawnPlayer(peerId.ToString(), spawnPos.GlobalPosition);
		}
	}

	private void SpawnPlayer(string id, Vector3 spawnPos)
	{
		var player = playerScene.Instantiate<Node3D>();
		player.Name = id;
		player.SetMultiplayerAuthority((int)long.Parse(id));
		AddChild(player);
		player.GlobalPosition = spawnPos;

		lobbyPlayers.Add(player);
	}

	private void OnPlayerJoined(long id)
	{
		if (Multiplayer.IsServer()) // This check may not be necessary
		{
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Requesting player " + id + " in position " + spawnPos.GlobalPosition);

			SpawnPlayer(id.ToString(), spawnPos.GlobalPosition);
		}
	}

	public void OnPlayerReady(Player player)
	{
		readyPlayers.Add(player);
		var peerCount = Multiplayer.GetPeers().Count() + 1;
		if (readyPlayers.Count == peerCount)
		{
			if (Multiplayer.IsServer())
			{
				RequestStartGame();
			}
		}
	}

	private void OnPlayerUnready(Player player)
	{
		readyPlayers.Remove(player);
	}

	public void RequestStartGame()
	{
		if (Multiplayer.IsServer())
		{
			// Rpc(MethodName.FreePlayers);
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Starting game");
			Rpc(MethodName.SyncGameStart);
			EmitSignal(SignalName.GameStartRequest);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void SyncGameStart()
	{
		GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, gameScene);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void FreePlayers()
	{
		foreach (var player in lobbyPlayers)
		{
			player.CallDeferred(MethodName.QueueFree);
		}
	}
}
