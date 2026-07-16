using Godot;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Combat.Weapons;

public partial class Gun : Node3D
{
	[Signal] public delegate void ReloadStartedEventHandler();
	[Signal] public delegate void ReloadFinishedEventHandler();
	[Signal] public delegate void ShotEventHandler(Projectile projectile);

	[Export] string poolName = "gun_projectiles";

	[ExportGroup("Shooting Parameters")]
	[Export] float shootingCooldown = 0.2f;
	[Export] int bulletsPerShot = 3;
	[Export] float shotArc = 45f;


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

	public void Shoot(Transform3D transform = default)
	{
		if (!IsMultiplayerAuthority()) return;
		if (!_canShoot) return;
		if (_currentAmmo == 0)
		{
			Reload();
			return;
		}

		if (transform == default) transform = shootingPoint.GlobalTransform;

		var shotArcRads = Mathf.DegToRad(shotArc);
		var increment = shotArcRads / (bulletsPerShot - 1);

		for (int i = 0; i < bulletsPerShot; i++)
		{
			var angleOffset = increment * i - (shotArcRads / 2);

			var bulletTransform = transform.RotatedLocal(Vector3.Up, angleOffset);

			var projectile = SpawnBullet(bulletTransform);

			Rpc(MethodName.SpawnBullet, bulletTransform);
			EmitSignal(SignalName.Shot, projectile);
		}

		_currentAmmo--;
		Rpc(MethodName.SyncAmmo, _currentAmmo);

		if (shootingCooldown > 0)
		{
			StartShootingCooldown();
		}

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
	private void SyncAmmo(int currentAmmo)
	{
		_currentAmmo = currentAmmo;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private Projectile SpawnBullet(Transform3D transform)
	{
		var bullet = ObjectPooler.Instance.RequestObject<Projectile>(poolName);
		EmitSignal(SignalName.Shot, bullet);
		CallDeferred(MethodName.InitBullet, bullet, transform);

		return bullet;
	}

	private void InitBullet(Projectile bullet, Transform3D transform)
	{
		bullet.SetShooter(Owner);
		bullet.GlobalTransform = transform;
	}

}
