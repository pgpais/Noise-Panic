using System;
using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles.Modifiers
{
    [GlobalClass]
    public abstract partial class ProjectileMovementModifier : ProjectileModifier, IMovementModifier
    {
        public abstract Vector3 CalculateVelocity(Node3D node, Vector3 velocity, float timeRatio);
    }
}