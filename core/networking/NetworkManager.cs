using System;
using System.Threading.Tasks;
using Godot;
using static Godot.Upnp;

/// <summary>
/// The NetworkManager class is responsible for managing the network connection.
/// 
/// It emits signals on different connection events.
/// </summary>
public partial class NetworkManager : Node
{
	const int DEFAULT_PORT = 7788;

	[Signal] public delegate void ServerHostedEventHandler();
	[Signal] public delegate void ConnectedToServerEventHandler();
	[Signal] public delegate void ClientConnectedEventHandler(long id);
	[Signal] public delegate void ClientDisconnectedEventHandler(long id);

	[Signal] public delegate void UpnpCompletedEventHandler(int error);

	public static NetworkManager Instance { get; set; }

	public override void _EnterTree()
	{
		// Singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			GD.PrintErr("NetworkManager already exists");
			QueueFree();
		}
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(Multiplayer.GetUniqueId() + ": " + "NetworkManager Setting up signals");
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
	}

	/// <summary>
	/// Sets up the UPNP discovery.
	/// </summary>
	/// <param name="port"></param>
	/// <returns></returns>
	private async Task<int> SetupUPNP(int port = DEFAULT_PORT)
	{
		GD.Print("[UPNP] Starting UPNP discovery");
		var upnp = new Upnp();
		int err = upnp.Discover();

		if (err != (int)Error.Ok)
		{
			GD.PushWarning("[UPNP] Error: " + (UpnpResult)err);
			GD.Print("[UPNP] Error: " + (UpnpResult)err);
			CallDeferred(MethodName.EmitSignal, SignalName.UpnpCompleted, err);
			return err;
		}

		var gateway = upnp.GetGateway();
		if (gateway.IsValidGateway())
		{
			GD.Print("[UPNP] Gateway found: " + gateway.QueryExternalAddress());
			upnp.AddPortMapping(port, port, ProjectSettings.GetSetting("application/config/name").ToString(), "UDP");
			upnp.AddPortMapping(port, port, ProjectSettings.GetSetting("application/config/name").ToString(), "TCP");

			GD.Print("[UPNP] Mapped port " + port + " to " + gateway.QueryExternalAddress() + ":" + port);
			CallDeferred(MethodName.EmitSignal, SignalName.UpnpCompleted, (int)UpnpResult.Success);
			return err;
		}

		GD.PushWarning("[UPNP] No gateway found");
		return (int)UpnpResult.NoGateway;
	}

	/// <summary>
	/// Creates a server on the specified port.
	/// </summary>
	/// <param name="port"> The port to create the server on. Defaults to 7777.</param>
	/// <returns>The result of the server creation.</returns>
	public Error CreateServer(int port = DEFAULT_PORT)
	{
		var peer = new ENetMultiplayerPeer();
		var result = peer.CreateServer(port);
		if (result == Error.Ok)
		{
			Multiplayer.MultiplayerPeer = peer;
			GD.Print("Started server on port " + port + " with peer ID " + Multiplayer.MultiplayerPeer.GetUniqueId());
			GD.Print(Multiplayer.GetUniqueId() + ": " + "NetworkManager Calling SetupUPNP");
			Task.Run(() => SetupUPNP());
			EmitSignal(SignalName.ServerHosted);
		}
		return result;
	}

	/// <summary>
	/// Creates a client connection to the specified server.
	/// </summary>
	/// <param name="ip">The IP address of the server to connect to. Defaults to "localhost".</param>
	/// <param name="port">The port of the server to connect to. Defaults to 7777.</param>
	/// <returns>The result of the client connection creation.</returns>
	public Error CreateClient(string ip = "localhost", int port = DEFAULT_PORT)
	{
		var peer = new ENetMultiplayerPeer();
		var result = peer.CreateClient(ip, port);
		if (result == Error.Ok)
		{
			Multiplayer.MultiplayerPeer = peer;
			GD.Print("Connected to " + ip + ":" + port + " with peer ID " + Multiplayer.MultiplayerPeer.GetUniqueId());
		}

		return result;
	}

	/// <summary>
	/// Called when a peer connects to the current peer.
	/// If the peer is the server (id == 1), emits the <see cref="SignalName.ConnectedToServer"/> signal.
	/// Otherwise, emits the <see cref="SignalName.ClientConnected"/> signal with the peer's ID.
	/// </summary>
	/// <param name="id">The ID of the connected peer.</param>
	private void OnPeerConnected(long id)
	{
		GD.Print(Multiplayer.GetUniqueId() + ": " + "Peer " + id + " connected.");

		if (id == 1)
		{
			// Connected to server
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Connected to server.");
			EmitSignal(SignalName.ConnectedToServer);
		}
		else
		{
			EmitSignal(SignalName.ClientConnected, id);
		}
	}

	/// <summary>
	/// Called when a peer disconnects from the current peer.
	/// If the peer is the server (id == 1), prints a message indicating that the connection to the server was lost.
	/// Otherwise, emits the <see cref="SignalName.ClientDisconnected"/> signal with the peer's ID.
	/// </summary>
	/// <param name="id">The ID of the disconnected peer.</param>
	private void OnPeerDisconnected(long id)
	{
		GD.Print(Multiplayer.GetUniqueId() + ": " + "Peer " + id + " disconnected.");

		if (id == 1)
		{
			// Disconnected from server
			GD.Print(Multiplayer.GetUniqueId() + ": " + "Disconnected from server.");
		}
		EmitSignal(SignalName.ClientDisconnected, id);
	}


}
