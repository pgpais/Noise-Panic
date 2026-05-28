using Godot;
using System.Linq;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class Interactor : Area3D
{
	[Signal] public delegate void StartedInteractingEventHandler(Interactable interactable);
	[Signal] public delegate void StoppedInteractingEventHandler(Interactable interactable);

	[Export] public Actor InteractActor { get; private set; }

	Interactable target;
	bool isInteracting = false;

	public override void _Ready()
	{
		base._Ready();

		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
	}

	public void StartInteracting()
	{
		if (HasTarget())
		{
			target.StartInteraction(this);
			isInteracting = true;
			EmitSignal(SignalName.StartedInteracting, target);
		}
	}

	public void StopInteracting()
	{
		if (isInteracting)
		{
			if (HasTarget())
			{
				target.StopInteraction(this);
				isInteracting = false;
				EmitSignal(SignalName.StoppedInteracting, target);
			}
			else
			{
				GD.PrintErr("Tried to stop an interaction without an interaction target!");
			}
		}

	}

	private void OnAreaEntered(Area3D area)
	{
		target = GetOverlappingInteractable();
	}

	private void OnAreaExited(Area3D area)
	{
		target = GetOverlappingInteractable();
	}

	private Interactable GetOverlappingInteractable()
	{
		var areas = GetOverlappingAreas();

		var interactables = areas.Where(x => x is Interactable).Cast<Interactable>().ToArray();

		Interactable closestInteractable = null;
		foreach (var interactable in interactables)
		{
			if (closestInteractable == null)
			{
				closestInteractable = interactable;
				continue;
			}

			bool isClosest = interactable.GlobalPosition.DistanceSquaredTo(GlobalPosition) < closestInteractable.GlobalPosition.DistanceSquaredTo(GlobalPosition);

			if (isClosest)
			{
				closestInteractable = interactable;
			}
		}

		// GD.Print("Interactor " + Name + " has " + interactables.Length + " interactables");
		return closestInteractable;
	}

	public bool HasTarget()
	{
		return target != null;
	}
}
