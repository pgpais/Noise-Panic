using Godot;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace Gameplay.Actors
{
    [GlobalClass]
    public partial class LookAtAimingController : Node
    {
        [Export] public Node3D lookAt;

        [Export] Gun gun;

        public void SetGun(Gun newGun)
        {
            //TODO: Sync gun set
            gun = newGun;
        }

        public void AimAtPos(Vector3 pos)
        {
            if (!IsMultiplayerAuthority()) return;

            lookAt.GlobalPosition = pos;
            Rpc(MethodName.SyncAiming, pos);
        }

        [Rpc()]
        private void SyncAiming(Vector3 pos) => lookAt.GlobalPosition = pos;

        public void Shoot()
        {
            if (!IsMultiplayerAuthority()) return;

            gun.Shoot();
            Rpc(MethodName.SyncShoot);
        }

        [Rpc()]
        private void SyncShoot() => gun.Shoot();

        public void Reload()
        {
            if (!IsMultiplayerAuthority()) return;

            gun.Reload();
        }

        [Rpc()]
        private void SyncReload()
        {
            gun.Reload();
            Rpc(MethodName.SyncReload);
        }
    }
}