using Godot;
using System;
using WeWereDumb.Gameplay.Actors;

public partial class PlayerDetectionArea : Area3D
{
	[Signal] public delegate void PlayerEnteredAreaEventHandler(Player player);
	[Signal] public delegate void PlayerExitedAreaEventHandler(Player player);

	public bool IsPlayerDetected { get; private set; }
	public Player CurrentPlayer { get; private set; }

	[Export] MeshInstance3D mesh;
	[Export] Material readyMaterial;
	[Export] Material notReadyMaterial;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;

		mesh.MaterialOverride = notReadyMaterial;
	}

	private void OnBodyExited(Node3D body)
	{
		if (body is Player player && player == CurrentPlayer)
		{
			IsPlayerDetected = false;
			CurrentPlayer = null;

			mesh.MaterialOverride = notReadyMaterial;
			EmitSignal(SignalName.PlayerExitedArea, player);
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		if (CurrentPlayer == null && body is Player player)
		{
			IsPlayerDetected = true;
			CurrentPlayer = player;

			mesh.MaterialOverride = readyMaterial;
			EmitSignal(SignalName.PlayerEnteredArea, player);
		}
	}
}
