using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using PhantomCamera;
using WeWereDumb.Gameplay.Input;

namespace WeWereDumb.Gameplay.Actors;

public partial class Player : Actor
{
	// @MrCoolerPants: Prometi à stream voltar ao Visage - Cheese
	//@alexmfcamara: Alex esteve aqui.

	public static List<Player> players = new();

	[Export] public NetworkedVoiceInput VoiceInput { get; private set; }

	[Export] CharacterController characterController;
	[Export] Array<Node> inputs;
	[Export] Node3D phantomcamera3d;
	[Export] HealthComponent healthComponent;
	PhantomCamera3D pcam;

	[ExportGroup("HUD")]
	[Export] CanvasLayer playerHud;

	public override void _EnterTree()
	{
		players.Add(this);
	}

	public override void _Ready()
	{
		healthComponent.HealthDepleted += OnHealthDepleted;
		EventBus.Instance.GamePaused += OnGamePaused;

		if (!IsMultiplayerAuthority())
		{
			playerHud.Hide();
			return;
		}
		pcam = phantomcamera3d.AsPhantomCamera3D();
		SetCameraPriority();
	}

	private void OnHealthDepleted()
	{
		GD.Print($"[Player] {Multiplayer.GetUniqueId()}: Health depleted!");
		EventBus.Instance.EmitSignal(EventBus.SignalName.PlayerDied);
	}

	public override void _ExitTree()
	{
		players.Remove(this);
	}

	public void EnableInput()
	{
		foreach (var input in inputs)
		{
			if (input is IInput iInput)
			{
				iInput.Enable();
			}
		}
	}

	public void DisableInput()
	{
		foreach (var input in inputs)
		{
			if (input is IInput iInput)
			{
				iInput.Disable();
			}
		}
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

	private void OnGamePaused(bool paused)
	{
		if (IsInstanceValid(this) && !IsMultiplayerAuthority()) return;
		if (paused)
		{
			DisableInput();
		}
		else
		{
			EnableInput();
		}
	}
}
