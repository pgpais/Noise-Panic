using Godot;
using System;
using System.Linq;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class Interactor : Node3D
{
	[Signal] public delegate void StartedInteractingEventHandler(Interactable interactable);
	[Signal] public delegate void StoppedInteractingEventHandler(Interactable interactable);

	[Export] public float InteractionDistance { get; private set; } = 6f;

	[Export] public Node3D CameraNode { get; private set; }
	[Export] public Actor InteractActor { get; private set; }

	[Export(PropertyHint.Layers3DPhysics)] public uint InteractCollisionMask { get; private set; }



	Interactable target;
	bool isInteracting = false;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!isInteracting)
		{
			var newTarget = DetectInteractable();
			if (newTarget != null)
			{
				target?.HideHighlight();
				target = newTarget;
				target.ShowHighlight();
			}
			else
			{
				target?.HideHighlight();
				target = null;
			}
		}
	}

	public void StartInteracting()
	{
		if (HasTarget())
		{
			target.StartInteraction(this);
			target.HideHighlight();
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

	private Interactable DetectInteractable()
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		// use global coordinates, not local to node
		Vector3 cameraPos = CameraNode.GlobalPosition;
		Vector3 cameraForward = -CameraNode.GlobalTransform.Basis.Z;
		Vector3 targetPos = cameraPos + cameraForward * InteractionDistance;
		var query = PhysicsRayQueryParameters3D.Create(cameraPos, targetPos);
		query.CollideWithAreas = true;
		query.CollideWithBodies = false;
		query.CollisionMask = InteractCollisionMask;
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
		{
			return null;
		}
		var collider = result["collider"].As<Node3D>();

		if (collider is Interactable interactable)
		{
			return interactable;
		}

		return null;
	}

	public bool HasTarget()
	{
		return target != null;
	}
}
