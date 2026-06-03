using Godot;

namespace WeWereDumb.Gameplay.Actors
{
    [GlobalClass]
    public partial class AimingController : Node
    {
        [Export] Node3D aimPivot;

        public void AimAt(Vector3 target)
        {
            Vector3 leveledTargetPos = new Vector3(target.X, aimPivot.GlobalPosition.Y, target.Z);

            aimPivot.LookAt(leveledTargetPos, Vector3.Up);

            Rpc(MethodName.SyncRotation, aimPivot.Rotation);
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncRotation(Vector3 rotation)
        {
            aimPivot.Rotation = rotation;
        }
    }
}