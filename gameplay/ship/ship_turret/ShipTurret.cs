using Godot;
using PhantomCamera;
using System;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Combat.Projectiles;
using WeWereDumb.Gameplay.Interaction;

public partial class ShipTurret : Node3D
{
	[Export] public Node3D phantomCameraNode;
	PhantomCamera3D pcam;

	[Export] Interactable interactable;

	[Export] Node3D turretPivot;


	[ExportGroup("Shooting")]
	[Export] string poolName = "turret_projectiles";
	[Export] PackedScene turretProjectileScene;
	[Export] float shootingForce = 20f;

	[ExportGroup("Input Settings")]
	[Export] float mouseSensitivity = 0.001f;

	Player player;

	bool isActive = false;
	Vector2 mouseDelta;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ObjectPooler.Instance.CreatePool<Projectile>(poolName, turretProjectileScene);

		pcam = phantomCameraNode.AsPhantomCamera3D();

		DeactivateStation();

		interactable.InteractionStarted += OnInteractionStarted;
		interactable.InteractionStopped += OnInteractionStopped;
	}

	override public void _Process(double delta)
	{
		base._Process(delta);
		if (isActive)
		{
			turretPivot.RotateZ(mouseDelta.X * mouseSensitivity);
			turretPivot.RotateX(-mouseDelta.Y * mouseSensitivity);

			if (Input.IsActionJustPressed("shoot"))
			{
				Shoot();
			}
		}
		mouseDelta = Vector2.Zero;
	}

	override public void _UnhandledInput(InputEvent @event)
	{
		if (isActive && @event is InputEventMouseMotion mouseMotion)
		{
			mouseDelta += mouseMotion.ScreenRelative;
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void Shoot(Vector3 position = default, Vector3 rotation = default)
	{
		if (position == default || rotation == default)
		{
			position = turretPivot.GlobalPosition;
			rotation = turretPivot.GlobalRotation;
		}

		if (turretProjectileScene != null)
		{
			if (!IsMultiplayerAuthority())
			{
				// Request shoot, pass local properties
				Rpc(MethodName.Shoot, turretPivot.GlobalPosition, turretPivot.GlobalRotation);
			}
			else
			{
				SpawnBullet(position, rotation);
				Rpc(MethodName.SyncShoot, position, rotation);
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncShoot(Vector3 position, Vector3 rotation)
	{
		SpawnBullet(position, rotation);
	}

	private void SpawnBullet(Vector3 position, Vector3 rotation)
	{
		var bullet = ObjectPooler.Instance.RequestObject<Projectile>(poolName);
		CallDeferred(MethodName.InitBullet, bullet, position, rotation);
	}

	private void InitBullet(Projectile bullet, Vector3 position, Vector3 rotation)
	{
		bullet.GlobalPosition = position;
		bullet.GlobalRotation = rotation;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		if (interactor.InteractActor is Player player)
		{
			this.player = player;
			player.DisableInput();
		}
		ActivateStation();
	}

	private void OnInteractionStopped()
	{
		player.EnableInput();
		player = null;

		DeactivateStation();
	}

	public void ActivateStation()
	{
		pcam.Priority = 10;
		isActive = true;
	}

	public void DeactivateStation()
	{
		pcam.Priority = 0;
		isActive = false;
	}
}
