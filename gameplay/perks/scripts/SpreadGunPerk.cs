using System;
using Godot;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace WeWereDumb.Gameplay.Perks
{
    [GlobalClass]
    public partial class SpreadGunPerk : GunPerk
    {
        [Export] public int BonusSpread { get; set; }

        override public void ApplyPerk(Gun gun)
        {
            throw new NotImplementedException();
        }

        override public void RemovePerk(Gun gun)
        {
            throw new NotImplementedException();
        }
    }
}