using Godot;
using System;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Interaction;

public partial class OverviewScreenDesk : Node3D
{
	[Export] Interactable interactable;

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
	}

	private void OnInteractionStopped()
	{
		if (interactable.Interactor.InteractActor is Player player)
		{
			player.EnableMovement();
		}
	}
}
