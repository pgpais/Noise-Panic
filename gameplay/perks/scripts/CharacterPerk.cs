using Godot;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.Perks
{
    [GlobalClass]
    public abstract partial class CharacterPerk : Perk
    {
        abstract public void ApplyPerk(Actor character);

        abstract public void RemovePerk(Actor character);
    }
}