using System.Collections.Generic;
using Godot;
using PhantomCamera;
using WeWereDumb.Gameplay.Input;

namespace WeWereDumb.Gameplay.Actors;

public partial class Player : Actor
{
	public static List<Player> players = new();

	[Export] public NetworkedVoiceInput VoiceInput { get; private set; }

	[Export] CharacterController characterController;
	[Export] PlayerAimingInput aimingInput;
	[Export] Node3D phantomcamera3d;
	PhantomCamera3D pcam;

	public override void _EnterTree()
	{
		players.Add(this);
	}

	public override void _Ready()
	{
		pcam = phantomcamera3d.AsPhantomCamera3D();
		SetCameraPriority();
	}

	public override void _ExitTree()
	{
		players.Remove(this);
	}

	public void EnableInput()
	{
		EnableAiming();
		EnableMovement();
	}

	public void DisableInput()
	{
		DisableAiming();
		DisableMovement();
	}

	public void EnableAiming()
	{
		aimingInput.Enable();
	}

	public void DisableAiming()
	{
		aimingInput.Disable();
	}

	public void DisableMovement()
	{
		characterController.MovementEnabled = false;
	}

	public void EnableMovement()
	{
		characterController.MovementEnabled = true;
	}

	private void SetCameraPriority()
	{
		if (IsMultiplayerAuthority())
		{
			GD.Print("Player is multiplayer authority");
			pcam.Priority = 10;
		}
		else
		{
			pcam.Priority = 0;
		}
	}
}
