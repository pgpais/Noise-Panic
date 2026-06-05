using Godot;
using WeWereDumb.Gameplay.Resources;

namespace WeWereDumb.Gameplay.Gathering
{
    public partial class ResourceHolder : Node3D
    {
        [Export] Node3D holdingPoint;

        public GameResource Resource => resource;

        private GameResource resource;


        public void AddResource(GameResource resource)
        {
            if (!IsMultiplayerAuthority())
            {
                RpcId(GetMultiplayerAuthority(), MethodName.RequestAddResource, resource.GetPath());
                return;
            }

            if (this.resource != null)
            {
                GD.PrintErr("ResourceHolder already has a resource");
                return;
            }

            SetResource(resource);

            Rpc(MethodName.SyncAddResource, resource.GetPath());
        }

        public bool HasResource() => resource != null;

        public bool TryToDropResource()
        {
            if (!IsMultiplayerAuthority()) return false;

            if (resource == null) return false;

            DropResource();
            Rpc(MethodName.SyncDropResource);

            return true;
        }


        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncDropResource()
        {
            DropResource();
        }

        private void DropResource()
        {
            GetTree().CurrentScene.AddChild(resource, true);
            resource = null;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void RequestAddResource(NodePath resourcePath)
        {
            if (!IsMultiplayerAuthority()) return;

            AddResource(GetNode<GameResource>(resourcePath));
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncAddResource(NodePath resourcePath)
        {
            var resource = GetNode<GameResource>(resourcePath);

            SetResource(resource);
        }

        private void SetResource(GameResource resource)
        {
            this.resource = resource;
            holdingPoint.AddChild(resource);
            resource.GlobalPosition = holdingPoint.GlobalPosition;
            resource.GlobalRotation = holdingPoint.GlobalRotation;
        }
    }
}