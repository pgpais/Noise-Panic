using Godot;

namespace WeWereDumb.Gameplay.Resources
{
    [GlobalClass]
    public partial class GameResourceData : Resource
    {
        [Export] public string Name { get; set; }
        [Export] public string Description { get; set; }
        [Export] public PackedScene Scene { get; private set; }
    }
}