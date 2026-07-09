using Godot;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace Gameplay.Actors
{
    [GlobalClass]
    public partial class LookAtAimingController : Node
    {
        [Export] public Node3D lookAt;

        [Export] Gun gun;

        public void SetGun(Gun newGun) => gun = newGun;

        public void AimAtPos(Vector3 pos) => lookAt.GlobalPosition = pos;

        public void Shoot() => gun.Shoot();

        public void Reload() => gun.Reload();
    }
}