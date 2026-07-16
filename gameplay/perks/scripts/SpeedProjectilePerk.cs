using Godot;
using WeWereDumb.Gameplay.Combat.Projectiles;

namespace WeWereDumb.Gameplay.Perks
{
    [GlobalClass]
    public partial class SpeedProjectilePerk : ProjectilePerk
    {
        [Export] public float BonusSpeed { get; private set; }

        override public void ApplyPerk(Projectile projectile)
        {
            projectile.AddSpeed(BonusSpeed);
        }

        override public void RemovePerk(Projectile projectile)
        {
            projectile.AddSpeed(-BonusSpeed);
        }
    }
}