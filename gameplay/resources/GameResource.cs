using Godot;

namespace WeWereDumb.Gameplay.Resources
{
    public partial class GameResource : Node3D
    {
        public GameResourceData Data => data;
        GameResourceData data;

        public void SetData(GameResourceData data)
        {
            this.data = data;
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void Destroy()
        {
            QueueFree();
        }
    }
}