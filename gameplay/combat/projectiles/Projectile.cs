using System;
using Godot;

namespace WeWereDumb.Gameplay.Combat.Projectiles
{
    public partial class Projectile : RigidBody3D
    {
        [Export] Hurtbox hurtbox;

        public override void _Ready()
        {
            hurtbox.Hit += OnHit;

        }

        private void OnHit(Hitbox hitbox)
        {
            //TODO: maybe deal damage?
            GD.Print($"Projectile {Name} hit {hitbox.Name}");
            QueueFree();
        }
    }
}