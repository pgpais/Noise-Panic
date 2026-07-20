using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles.Modifiers
{
    [GlobalClass]
    public partial class SlowdownOverTimeProjectileMovementModifier : ProjectileMovementModifier
    {
        [Export] Curve curve;

        public override Vector3 CalculateVelocity(Node3D node, Vector3 velocity, float timeRatio)
        {
            if (timeRatio < 0 || timeRatio > 1)
            {
                GD.PrintErr("Time ratio out of range");
                return Vector3.Zero;
            }
            GD.Print("Time ratio: " + timeRatio + ", sample: " + curve.Sample(timeRatio));
            return -velocity * curve.Sample(timeRatio);
        }
    }
}