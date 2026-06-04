using Godot;
using PhantomCamera;

namespace WeWereDumb.Gameplay.Actors
{
    [GlobalClass]
    public partial class AimingController : Node
    {
        [Export] Node3D aimPivot;
        [Export] Node3D cameraPivot;
        [Export] float mouseSensitivity = 10f;

        public Vector2 MouseMovementInput { get; set; }

        private float targetXRotation;
        private float targetYRotation;

        public override void _Ready()
        {
            base._Ready();

            targetXRotation = cameraPivot.RotationDegrees.X;
            targetYRotation = aimPivot.RotationDegrees.Y;

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
            // ctargetXRotation = Mathf.Clamp(cameraRotationDegrees.X, -90, 90);
            targetYRotation -= MouseMovementInput.X * mouseSensitivity;

            cameraPivot.RotationDegrees = new Vector3(targetXRotation, cameraPivot.RotationDegrees.Y, cameraPivot.RotationDegrees.Z);
            aimPivot.RotationDegrees = new Vector3(aimPivot.RotationDegrees.X, targetYRotation, aimPivot.RotationDegrees.Z);

            SyncRotation(cameraPivot.Rotation, aimPivot.Rotation);
            MouseMovementInput = Vector2.Zero;
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncRotation(Vector3 cameraPivotRotation, Vector3 aimPivotRotation)
        {
            cameraPivot.RotationDegrees = cameraPivotRotation;
            aimPivot.RotationDegrees = aimPivotRotation;
        }
    }
}