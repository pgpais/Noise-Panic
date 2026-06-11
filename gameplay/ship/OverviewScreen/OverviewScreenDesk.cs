using Godot;
using System;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Interaction;

public partial class OverviewScreenDesk : Node3D
{
	[Export] Interactable interactable;
	[Export] ShipController shipController;
	[Export] RigidBody3D shipBody;

	bool inUse = false;
	int ownerId = -1;

	Vector2 input;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shipController.SetBody(shipBody);

		interactable.InteractionStarted += OnInteractionStarted;
		interactable.InteractionStopped += OnInteractionStopped;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		if (inUse)
		{
			interactor.CancelInteraction();
			return;
		}

		if (interactor.InteractActor is Player player)
		{
			player.DisableMovement();
		}


		SetUseState(true, Multiplayer.GetUniqueId());
	}

	override public void _Process(double delta)
	{
		if (inUse && ownerId == Multiplayer.GetUniqueId())
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
		shipController.InputDirection = input;
	}

	private void SetUseState(bool inUse, int ownerId)
	{
		this.inUse = inUse;
		this.ownerId = ownerId;
		Rpc(MethodName.SyncUseState, inUse, ownerId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SyncUseState(bool inUse, int ownerId)
	{
		this.inUse = inUse;
		this.ownerId = ownerId;
	}

	private void OnInteractionStopped()
	{
		if (interactable.Interactor.InteractActor is Player player)
		{
			player.EnableMovement();
		}

		SetUseState(false, -1);
	}
}
