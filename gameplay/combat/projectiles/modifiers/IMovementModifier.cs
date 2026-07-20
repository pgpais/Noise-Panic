using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles.Modifiers
{
    public interface IMovementModifier
    {
        public Vector3 CalculateVelocity(Node3D node, Vector3 velocity, float delta);
    }
}