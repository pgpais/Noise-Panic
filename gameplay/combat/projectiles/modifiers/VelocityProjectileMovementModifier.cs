using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles.Modifiers
{
    [GlobalClass]
    public partial class VelocityProjectileMovementModifier : ProjectileMovementModifier
    {
        [Export] public float Speed { get; private set; } = 20f;

        public override Vector3 CalculateVelocity(Node3D node, Vector3 velocity, float delta)
        {
            var dir = -node.GlobalTransform.Basis.Z;
            return dir * Speed;
        }
    }
}