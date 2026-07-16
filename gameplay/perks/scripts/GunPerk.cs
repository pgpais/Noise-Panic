using Godot;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace WeWereDumb.Gameplay.Perks
{
    [GlobalClass]
    public abstract partial class GunPerk : Perk
    {
        abstract public void ApplyPerk(Gun gun);

        abstract public void RemovePerk(Gun gun);
    }
}