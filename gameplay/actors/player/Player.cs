using System.Collections.Generic;
using Godot;
using PhantomCamera;

namespace WeWereDumb.Gameplay.Actors;

public partial class Player : Actor
{
	public static List<Player> players = new();

	[Export] CharacterController characterController;
	[Export] Node3D phantomcamera3d;
	PhantomCamera3D pcam;

	public override void _EnterTree()
	{
		players.Add(this);
	}

	public override void _Ready()
	{
		pcam = phantomcamera3d.AsPhantomCamera3D();
		if (IsMultiplayerAuthority())
		{
			pcam.Priority = 1;
		}
	}

	public override void _ExitTree()
	{
		players.Remove(this);
	}

	public void DisableMovement()
	{
		characterController.MovementEnabled = false;
	}

	public void EnableMovement()
	{
		characterController.MovementEnabled = true;
	}
}
