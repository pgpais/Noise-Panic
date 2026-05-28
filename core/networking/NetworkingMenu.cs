using System;
using Godot;

public partial class NetworkingMenu : Control
{
	[ExportGroup("Local")]
	[Export] Button hostButtonLocal;
	[Export] LineEdit IpLocal;
	[Export] LineEdit PortLocal;
	[Export] Button joinButtonLocal;
	[ExportGroup("Steam")]
	[Export] Button hostButtonSteam;
	[Export] LineEdit joinInfoSteam;
	[Export] Button joinButtonSteam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NetworkManager.Instance.ConnectedToServer += OnConnectedToServer;

		hostButtonLocal.Pressed += OnHostLocalPressed;
		joinButtonLocal.Pressed += OnJoinLocalPressed;
		hostButtonSteam.Pressed += OnHostSteamPressed;
		joinButtonSteam.Pressed += OnJoinSteamPressed;

		HandleArguments();
	}

	private void OnConnectedToServer()
	{
		Hide();
	}

	private void HandleArguments()
	{
		var arguments = OS.GetCmdlineArgs();
		for (int i = 0; i < arguments.Length; i++)
		{
			var argument = arguments[i];
			// +connect_lobby <64-bit lobby Steam ID>
			if (argument.StartsWith("+connect_lobby "))
			{
				i++;
				joinInfoSteam.Text = arguments[i];
				OnJoinSteamPressed();
			}
		}
	}

	private void OnHostLocalPressed()
	{
		var port = PortLocal.Text == "" ? 7777 : int.Parse(PortLocal.Text);
		NetworkManager.Instance.CreateServer(port);
		Hide();
	}

	private void OnJoinLocalPressed()
	{
		var port = PortLocal.Text == "" ? 7777 : int.Parse(PortLocal.Text);
		var ip = IpLocal.Text == "" ? "localhost" : IpLocal.Text;
		NetworkManager.Instance.CreateClient(ip, port);
		Hide();
	}

	private void OnHostSteamPressed()
	{
		// NetworkManager.Instance.CreateServer();
		var steamNetworkManager = GetNode("/root/SteamNetworkManager");
		steamNetworkManager.Call("host_server");
		//TODO: move hide somewhere else
		Hide();
	}

	private void OnJoinSteamPressed()
	{
		// NetworkManager.Instance.CreateClient();
		var steamNetworkManager = GetNode("/root/SteamNetworkManager");
		steamNetworkManager.Call("join_lobby", int.Parse(joinInfoSteam.Text));
		//TODO: move hide somewhere else
		Hide();
	}

}
