using System;
using Godot;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Combat.Weapons;

public partial class Gun : Node3D
{
	[Export] string poolName = "gun_projectiles";
	[Export] float shootingForce = 20f;
	[Export] Node3D shootingPoint;

	[Export] PackedScene bulletPrefab; //TODO: move to object pooling

	public override void _Ready()
	{
		base._Ready();

		ObjectPooler.Instance.CreatePool<Projectile>(poolName, bulletPrefab);
	}

	public void Shoot(Vector3 rotation = default)
	{
		if (rotation == default) rotation = shootingPoint.GlobalRotation;

		SpawnBullet(shootingPoint.GlobalPosition, rotation);

		Rpc(MethodName.SyncShoot, shootingPoint.GlobalPosition, shootingPoint.GlobalRotation);
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
		GD.Print(bullet.GlobalRotation);
		bullet.ApplyCentralImpulse(-bullet.GlobalTransform.Basis.Z * shootingForce);
	}

	public void Reload()
	{
		throw new NotImplementedException();
	}
}
