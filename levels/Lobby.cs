using Godot;
using Godot.Collections;

/// <summary>
/// The Lobby class is responsible for managing the lobby scene.
///
/// </summary>
public partial class Lobby : Node3D
{
	[Signal] public delegate void GameStartRequestEventHandler();

	[Export] Node3D spawnPos;
	[Export] PackedScene playerScene;

	Array<Node> lobbyPlayers = new Array<Node>();

	public override void _Ready()
	{
		SpawnLocalPlayer();
		SpawnPeers();

		GD.Print(Multiplayer.GetUniqueId() + ": " + "Lobby ready");

		NetworkManager.Instance.ClientConnected += OnPlayerJoined;
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

	public void RequestStartGame()
	{
		if (Multiplayer.IsServer())
		{
			Rpc(MethodName.FreePlayers);
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Starting game");
			EmitSignal(SignalName.GameStartRequest);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void FreePlayers()
	{
		foreach (var player in lobbyPlayers)
		{
			player.QueueFree();
		}
	}
}
