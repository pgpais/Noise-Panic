using Godot;
using PhantomCamera;

namespace WeWereDumb.Gameplay.Actors
{
    [GlobalClass]
    public partial class AimingController : Node
    {
        [ExportGroup("Settings")]
        [Export] float mouseSensitivity = 0.001f;
        [Export] float aimXClamp = 60;

        [ExportGroup("References")]
        [Export] Node3D aimPivot;
        [Export] Node3D cameraPivot;

        public Vector2 MouseMovementInput { get; set; }

        private float targetXRotation;
        private float targetYRotation;

        public override void _Ready()
        {
            base._Ready();
            if (!IsMultiplayerAuthority()) return;

            targetXRotation = cameraPivot.Rotation.X;
            targetYRotation = aimPivot.Rotation.Y;

            Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (!IsMultiplayerAuthority()) return;

            if (Godot.Input.IsActionPressed("ui_cancel"))
            {
                Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Visible;
            }

            targetXRotation -= MouseMovementInput.Y * mouseSensitivity;
            GD.Print("targetXRotation: " + targetXRotation);
            targetXRotation = Mathf.Clamp(targetXRotation, -aimXClamp, aimXClamp);
            targetYRotation -= MouseMovementInput.X * mouseSensitivity;

            var targetCameraRotation = new Vector3(targetXRotation, cameraPivot.Rotation.Y, cameraPivot.Rotation.Z);
            var targetAimRotation = new Vector3(aimPivot.Rotation.X, targetYRotation, aimPivot.Rotation.Z);

            cameraPivot.Rotation = targetCameraRotation;
            aimPivot.Rotation = targetAimRotation;

            Rpc(MethodName.SyncRotation, targetCameraRotation, targetAimRotation);
            MouseMovementInput = Vector2.Zero;
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncRotation(Vector3 targetCameraRotation, Vector3 targetAimRotation)
        {
            cameraPivot.Rotation = targetCameraRotation;
            aimPivot.Rotation = targetAimRotation;
        }
    }
}