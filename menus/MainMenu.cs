using System;
using Godot;

public partial class MainMenu : Control
{
	[Export] PackedScene lobbyScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// NetworkManager.Instance.ConnectedToServer += OnConnectedToServer;
		// NetworkManager.Instance.ServerHosted += OnConnectedToServer;
	}

	private void OnConnectedToServer()
	{
		GetTree().ChangeSceneToPacked(lobbyScene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
