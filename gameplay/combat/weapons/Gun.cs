using Godot;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Combat.Weapons;

public partial class Gun : Node3D
{
	[Export] float shootingForce = 20f;
	[Export] Node3D shootingPoint;

	[Export] PackedScene bulletPrefab; //TODO: move to object pooling



	public void Shoot()
	{
		var bullet = bulletPrefab.Instantiate<Projectile>();
		GetTree().CurrentScene.AddChild(bullet);
		bullet.GlobalPosition = shootingPoint.GlobalPosition;
		bullet.GlobalRotation = shootingPoint.GlobalRotation;
		bullet.ApplyCentralImpulse(-shootingPoint.GlobalTransform.Basis.Z * shootingForce);
	}
}
