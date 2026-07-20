using System;
using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles.Modifiers
{
    [GlobalClass]
    public partial class RotatingProjectileMovementModifier : ProjectileMovementModifier
    {
        [Export] public float rotationSpeed = 0.1f;

        public override Vector3 CalculateVelocity(Node3D node, Vector3 velocity, float delta)
        {
            // @LeConde: Eu tentei arrumar essa parte por que a bala estava rodando em círculos. Eu nao faço ideia do que causou isso, mas depois de deletar 35 linhas de código o problema parece ter sido resolvido.
            node.Rotate(Vector3.Up, rotationSpeed);
            return Vector3.Zero;
        }
    }
}