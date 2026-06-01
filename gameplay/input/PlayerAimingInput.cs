using Godot;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace WeWereDumb.Gameplay.Input
{
    [GlobalClass]
    public partial class PlayerAimingInput : Node
    {
        [Export] AimingController aimingController;
        [Export] Gun gun;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            HandleMouseAiming();

            if (Godot.Input.IsActionJustPressed("shoot"))
            {
                gun.Shoot();
            }
        }

        private void HandleMouseAiming()
        {
            var viewport = GetViewport();
            var camera = viewport.GetCamera3D();
            var mousePos = viewport.GetMousePosition();

            var rayNormal = camera.ProjectRayNormal(mousePos);
            var rayOrigin = camera.ProjectRayOrigin(mousePos);

            var parameters = new PhysicsRayQueryParameters3D
            {
                From = rayOrigin,
                To = rayOrigin + rayNormal * 1000
            };

            var hitDictionary = camera.GetWorld3D().DirectSpaceState.IntersectRay(parameters);

            if (hitDictionary.Count > 0)
            {
                var hitPosition = hitDictionary["position"].AsVector3();

                aimingController.AimAt(hitPosition);
            }
        }
    }
}