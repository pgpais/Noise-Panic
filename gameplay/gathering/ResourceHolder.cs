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

            Rpc(MethodName.SyncAddResource, resource.GetPath());
            SetResource(resource);
        }

        public bool HasResource() => resource != null;

        public bool TryToDropResource()
        {
            if (!IsMultiplayerAuthority()) return false;

            if (resource == null) return false;

            CallDeferred(MethodName.DropResource);
            Rpc(MethodName.SyncDropResource);

            return true;
        }


        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncDropResource()
        {
            CallDeferred(MethodName.DropResource);
        }

        private void DropResource()
        {
            resource.Reparent(GetTree().CurrentScene);
            resource.Enable();
            GetDropPosition(out var position, out var normal);
            var angle = Mathf.Acos(resource.Transform.Basis.Y.Dot(normal));
            var axis = normal.Cross(resource.Transform.Basis.Y);
            resource.Rotate(axis, angle);

            resource.GlobalPosition = position;

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
            GD.Print("Synced Add resource " + resourcePath);

            var resource = GetNode<GameResource>(resourcePath);

            SetResource(resource);
        }

        private void SetResource(GameResource resource)
        {
            this.resource = resource;
            resource.Disable();
            resource.Reparent(holdingPoint);
            resource.GlobalPosition = holdingPoint.GlobalPosition;
            resource.GlobalRotation = holdingPoint.GlobalRotation;
        }

        private void GetDropPosition(out Vector3 position, out Vector3 normal)
        {
            var query = PhysicsRayQueryParameters3D.Create(holdingPoint.GlobalPosition, holdingPoint.GlobalPosition - Vector3.Up * 100);
            query.CollideWithAreas = false;
            query.CollideWithBodies = true;
            var result = GetWorld3D().DirectSpaceState.IntersectRay(query);
            if (result.Count == 0)
            {
                normal = Vector3.Zero;
                position = Vector3.Zero;
                return;
            }
            // Align Y axis with normal for rotation
            normal = result["normal"].As<Vector3>();
            position = result["position"].As<Vector3>();
        }
    }
}