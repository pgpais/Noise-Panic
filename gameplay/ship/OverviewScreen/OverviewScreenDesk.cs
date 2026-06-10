using Godot;
using System;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Interaction;

public partial class OverviewScreenDesk : Node3D
{
	[Export] Interactable interactable;
	[Export] ShipController shipController;

	bool inUse = false;
	Vector2 input;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		interactable.InteractionStarted += OnInteractionStarted;
		interactable.InteractionStopped += OnInteractionStopped;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		if (interactor.InteractActor is Player player)
		{
			player.DisableMovement();
		}

		inUse = true;
	}

	override public void _PhysicsProcess(double delta)
	{
		if (inUse && IsMultiplayerAuthority())
		{
			input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			Rpc(MethodName.SyncInput, input);
		}
		shipController.InputDirection = input;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SyncInput(Vector2 input)
	{
		this.input = input;
	}

	private void OnInteractionStopped()
	{
		if (interactable.Interactor.InteractActor is Player player)
		{
			player.EnableMovement();
		}

		inUse = false;
	}
}
