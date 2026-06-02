using Godot;
using WeWereDumb.Gameplay.Resources;

namespace WeWereDumb.Gameplay.Gathering
{
    public partial class ResourceHolder : Node3D
    {
        [Export] Node3D holdingPoint;

        private GameResource resource;

        public void AddResource(GameResource resource)
        {
            if (this.resource != null)
            {
                GD.PrintErr("ResourceHolder already has a resource");
                return;
            }

            this.resource = resource;
            holdingPoint.AddChild(resource);
            resource.GlobalPosition = holdingPoint.GlobalPosition;
            resource.GlobalRotation = holdingPoint.GlobalRotation;
        }

        public void DropResource(Vector3 position, Vector3 rotation)
        {
            holdingPoint.RemoveChild(resource);
            resource.GlobalPosition = position;
            resource.GlobalRotation = rotation;
            resource = null;
        }
    }
}