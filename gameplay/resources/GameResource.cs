using Godot;

namespace WeWereDumb.Gameplay.Resources
{
    public partial class GameResource : Node3D
    {
        GameResourceData data;

        public void SetData(GameResourceData data)
        {
            this.data = data;
        }
    }
}