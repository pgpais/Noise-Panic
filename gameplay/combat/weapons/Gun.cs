using Godot;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Combat.Weapons;

public partial class Gun : Node3D
{
	[Signal] public delegate void ReloadStartedEventHandler();
	[Signal] public delegate void ReloadFinishedEventHandler();

	[Export] string poolName = "gun_projectiles";

	[ExportGroup("Shooting Parameters")]
	[Export] float shootingForce = 20f;
	[Export] float shootingCooldown = 0.2f;
	[ExportSubgroup("Magazine")]
	[Export] float reloadTime = 2f;
	[Export] int maxAmmo = 12;
	private int _currentAmmo;

	[ExportGroup("References")]
	[Export] Node3D shootingPoint;
	[Export] PackedScene bulletPrefab;

	private Timer _shootingTimer;
	private bool _canShoot = true;
	private bool _isReloading = false;

	public override void _Ready()
	{
		base._Ready();

		if (shootingCooldown > 0)
		{
			_shootingTimer = new Timer
			{
				WaitTime = shootingCooldown,
				OneShot = true
			};
			_shootingTimer.Timeout += OnShootingCooldownTimeout;
			AddChild(_shootingTimer);
		}

		ObjectPooler.Instance.CreatePool<Projectile>(poolName, bulletPrefab);

		_currentAmmo = maxAmmo;
	}

	public void Shoot(Vector3 rotation = default)
	{
		if (!IsMultiplayerAuthority()) return;
		if (!_canShoot) return;
		if (_currentAmmo == 0)
		{
			Reload();
			return;
		}

		if (rotation == default) rotation = shootingPoint.GlobalRotation;

		SpawnBullet(shootingPoint.GlobalPosition, rotation);

		_currentAmmo--;
		Rpc(MethodName.SyncAmmo, _currentAmmo);

		if (shootingCooldown > 0)
		{
			StartShootingCooldown();
		}

		Rpc(MethodName.SyncShoot, shootingPoint.GlobalPosition, rotation);
	}

	public void Reload()
	{
		if (!IsMultiplayerAuthority()) return;
		if (_isReloading) return;
		_isReloading = true;

		_currentAmmo = 0;
		Rpc(MethodName.SyncAmmo, _currentAmmo);

		//TODO: play animation
		//TODO: listen to animation finished signal
		GetTree().CreateTimer(reloadTime).Timeout += OnReloadFinished; // Until we have animations


		GD.Print($"[Gun] {Multiplayer.GetUniqueId()}:  Reloading...");
		EmitSignal(SignalName.ReloadStarted);
	}

	private void OnReloadFinished()
	{
		_currentAmmo = maxAmmo;
		_canShoot = true;
		_isReloading = false;

		Rpc(MethodName.SyncAmmo, _currentAmmo);

		GD.Print($"[Gun] {Multiplayer.GetUniqueId()}:  Reload Finished!");
		EmitSignal(SignalName.ReloadFinished);
	}

	private void StartShootingCooldown()
	{
		_canShoot = false;
		_shootingTimer.Start();
	}

	private void OnShootingCooldownTimeout()
	{
		_canShoot = true;

		//TODO: possible visual feedback?
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncShoot(Vector3 position, Vector3 rotation)
	{
		SpawnBullet(position, rotation);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncAmmo(int currentAmmo)
	{
		_currentAmmo = currentAmmo;
	}

	private void SpawnBullet(Vector3 position, Vector3 rotation)
	{
		var bullet = ObjectPooler.Instance.RequestObject<Projectile>(poolName);
		CallDeferred(MethodName.InitBullet, bullet, position, rotation);
	}

	private void InitBullet(Projectile bullet, Vector3 position, Vector3 rotation)
	{
		bullet.SetShooter(Owner);
		bullet.GlobalPosition = position;
		bullet.GlobalRotation = rotation;
		bullet.SetVelocity(-bullet.GlobalTransform.Basis.Z * shootingForce);
	}

}
