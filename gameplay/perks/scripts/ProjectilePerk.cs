using Godot;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Perks
{
    [GlobalClass]
    public abstract partial class ProjectilePerk : Perk
    {
        public abstract void ApplyPerk(Projectile projectile);

        public abstract void RemovePerk(Projectile projectile);
    }
}