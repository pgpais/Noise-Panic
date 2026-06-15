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
	[Export] Array<PlayerDetectionArea> playerDetectionAreas;
	[Export] PlayerSpawner playerSpawner;

	[Export] PackedScene gameScene;

	Array<Node> lobbyPlayers = new Array<Node>();
	Array<Player> readyPlayers = new Array<Player>();

	public override void _Ready()
	{
		foreach (var area in playerDetectionAreas)
		{
			area.PlayerEnteredArea += OnPlayerReady;
			area.PlayerExitedArea += OnPlayerUnready;
		}

		playerSpawner.PlayerSpawned += (player, id) =>
		{
			lobbyPlayers.Add(player);
		};

		GD.Print("[Lobby] " + Multiplayer.GetUniqueId() + ": " + "Lobby ready");
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
