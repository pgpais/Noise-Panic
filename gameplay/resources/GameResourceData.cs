using Godot;

namespace WeWereDumb.Gameplay.Resources
{
    [GlobalClass]
    public partial class GameResourceData : Resource
    {
        [Export] public string Id { get; private set; }
        [Export] public string Name { get; private set; }
        [Export] public string Description { get; private set; }
        [Export] public PackedScene Scene { get; private set; }
    }
}