using Godot;

namespace WeWereDumb.Gameplay.Ship
{
    public partial class PlayerPodLauncher : Node3D
    {
        [Export] PlayerDetectionArea detectionArea;
        [Export] Node3D podLaunchPoint;

        public void TryLaunchPlayer()
        {
            if (!IsMultiplayerAuthority()) return;

            if (detectionArea.IsPlayerDetected)
            {
                Rpc(MethodName.LaunchPlayer);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        private void LaunchPlayer()
        {
            var player = detectionArea.CurrentPlayer;
            player.GlobalTransform = podLaunchPoint.GlobalTransform;
            player.Velocity = Vector3.Zero;
        }

    }
}