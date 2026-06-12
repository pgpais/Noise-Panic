using Godot;
using Godot.Collections;
using System;
using WeWereDumb.Core.Data;

// After main menu, could be in lobby or in a game run
public partial class GameOrchestrator : Node3D
{
	public static GameOrchestrator Instance { get; private set; }

	[Signal] public delegate void GameStartedEventHandler();
	[Signal] public delegate void GameInitializedEventHandler();

	GameSession _gameSession;

	[Export] bool hostLocal = false;

	[Export] LevelLoader _levelLoader;


	[ExportGroup("Levels")]
	[Export] PackedScene hubScene;
	[Export] PackedScene gameRunScene;

	override public void _EnterTree()
	{
		if (Instance != null)
		{
			GD.PrintErr("[GameOrchestrator] " + Multiplayer.GetUniqueId() + ": " + "Instance already set, this should not happen.");
			QueueFree();
			return;
		}
		Instance = this;
	}

	override public void _Ready()
	{
		NetworkManager.Instance.ServerHosted += InitializeSession;
		NetworkManager.Instance.ConnectedToServer += InitializeSession;
		NetworkManager.Instance.ClientConnected += OnClientConnected;

		if (hostLocal)
		{
			HostLocal();
		}
		else
		{
			//TODO: create cases for other provider (?)
			HostSteam();
		}
	}

	override public void _ExitTree()
	{
		Instance = null;
	}

	public void InitializeSession()
	{
		GD.Print("[GameOrchestrator] " + Multiplayer.GetUniqueId() + ": " + "Initializing session...");

		_gameSession = new();

		LoadHub(isSynced: false);
	}

	public void LoadHub(bool isSynced = true)
	{
		GD.Print("[GameOrchestrator] " + Multiplayer.GetUniqueId() + ": " + "Loading hub..." + " isSynced: " + isSynced);
		LoadLevel(hubScene.ResourcePath, isSynced);
	}

	public void LoadGameRun(bool isSynced = true)
	{
		GD.Print("[GameOrchestrator]" + Multiplayer.GetUniqueId() + ": " + "Loading game run..." + " isSynced: " + isSynced);
		LoadLevel(gameRunScene.ResourcePath, isSynced);
	}

	public void StartRun()
	{
		GD.Print("[GameOrchestrator]" + Multiplayer.GetUniqueId() + ": " + "Starting run...");
		EmitSignal(SignalName.GameStarted);
	}

	private void LoadLevel(string levelScenePath, bool isSynced = true)
	{
		if (levelScenePath == null)
			return;

		_levelLoader.LevelLoaded += OnLevelLoaded;
		_levelLoader.LoadLevelRequest(levelScenePath, isSynced: isSynced);
	}

	private void OnLevelLoaded(Node level)
	{
		_levelLoader.LevelLoaded -= OnLevelLoaded;
		if (level is Lobby lobby)
		{
			// Listen to lobby events
			lobby.GameStartRequest += OnGameStartRequested;
			EmitSignal(SignalName.GameInitialized);
		}
	}

	private void OnGameStartRequested()
	{
		LoadGameRun();
	}

	private void HostLocal()
	{
		GD.Print("[GameOrchestrator]" + Multiplayer.GetUniqueId() + ": " + "Trying to host local server...");
		var port = 7777;
		var error = NetworkManager.Instance.CreateServer(port);
		if (error != Error.Ok)
		{
			GD.PushWarning("Failed to host local server: " + error);
			JoinLocal();
		}
	}

	private void JoinLocal()
	{
		GD.Print("[GameOrchestrator]" + Multiplayer.GetUniqueId() + ": " + "Trying to join local server...");
		var port = 7777;
		var ip = "localhost";
		NetworkManager.Instance.CreateClient(ip, port);
	}

	private void HostSteam()
	{
		// NetworkManager.Instance.CreateServer();
		var steamNetworkManager = GetNode("/root/SteamNetworkManager");
		steamNetworkManager.Call("host_server");
		//TODO: move hide somewhere else
	}

	private void OnClientConnected(long id)
	{
		if (IsMultiplayerAuthority())
		{
			var peerData = new PeerData();
			_gameSession.SetPeerData((int)id, peerData);
		}
	}
}
