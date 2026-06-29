using System;
using Godot;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Combat.Weapons;

public partial class Gun : Node3D
{
	[Export] string poolName = "gun_projectiles";
	
	[ExportGroup("Shooting Parameters")]
	[Export] float shootingForce = 20f;
	[Export] float shootingCooldown = 0.2f; 
	
	[ExportGroup("References")]
	[Export] Node3D shootingPoint;
	[Export] PackedScene bulletPrefab;

	private Timer shootingTimer;
	private bool canShoot = true;
	
	public override void _Ready()
	{
		base._Ready();

		shootingTimer = new Timer
		{
			WaitTime = shootingCooldown,
			OneShot = true
		};
		shootingTimer.Timeout += OnShootingCooldownTimeout;
		AddChild(shootingTimer);
		
		ObjectPooler.Instance.CreatePool<Projectile>(poolName, bulletPrefab);
	}

	public void Shoot(Vector3 rotation = default)
	{
		if (!canShoot) return;
		
		if (rotation == default) rotation = shootingPoint.GlobalRotation;
		
		SpawnBullet(shootingPoint.GlobalPosition, rotation);
		StartShootingCooldown();

		Rpc(MethodName.SyncShoot, shootingPoint.GlobalPosition, shootingPoint.GlobalRotation);
	}

	public void Reload()
	{
		throw new NotImplementedException();
	}

	private void StartShootingCooldown()
	{
		canShoot = false;
		shootingTimer.Start();
	}

	private void OnShootingCooldownTimeout()
	{
		canShoot = true;
		
		//TODO: possible visual feedback?
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncShoot(Vector3 position, Vector3 rotation)
	{
		SpawnBullet(position, rotation);
		StartShootingCooldown();
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
