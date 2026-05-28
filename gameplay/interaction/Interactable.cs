using Godot;

namespace WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class Interactable : Area3D
{
	[Signal] public delegate void InteractionStartedEventHandler(Interactor interactor);
	[Signal] public delegate void InteractionStoppedEventHandler(Interactor interactor);

	public virtual void StartInteraction(Interactor interactor)
	{
		EmitSignal(SignalName.InteractionStarted, interactor);
	}

	public virtual void StopInteraction(Interactor interactor)
	{
		EmitSignal(SignalName.InteractionStopped, interactor);
	}
}
