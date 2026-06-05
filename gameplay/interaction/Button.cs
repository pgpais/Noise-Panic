using System;
using Godot;

namespace WeWereDumb.Gameplay.Interaction;

public partial class Button : StaticBody3D
{
	[Signal] delegate void PressedEventHandler();

	[Export] Interactable interactable;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		interactable.InteractionStarted += OnInteractionStarted;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		PushButton();
		Rpc(MethodName.PushButton);
	}

	public virtual void PlayButtonFeedback()
	{

	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void PushButton()
	{
		PlayButtonFeedback();
		EmitSignal(SignalName.Pressed);
	}
}
