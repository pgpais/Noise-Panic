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

	public void Shoot()
	{
		var bullet = ObjectPooler.Instance.RequestObject<Projectile>(poolName);
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = shootingPoint.GlobalPosition;
		bullet.GlobalRotation = shootingPoint.GlobalRotation;
		bullet.ApplyCentralImpulse(-shootingPoint.GlobalTransform.Basis.Z * shootingForce);
	}
}
