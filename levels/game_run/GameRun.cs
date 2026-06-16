using Godot;
using System;

public partial class GameRun : Node3D
{
	[ExportGroup("Run Parameters")]
	[Export] Timer countdownTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

}
