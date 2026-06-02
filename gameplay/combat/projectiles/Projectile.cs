using System;
using Godot;
using WeWereDumb.Core.Pooling;

namespace WeWereDumb.Gameplay.Combat.Projectiles
{
    public partial class Projectile : RigidBody3D, IPooledObject
    {
        [Export] float timeToLive = 2f;
        [Export] Hurtbox hurtbox;

        private ObjectPool pool;
        SceneTreeTimer timer;


        public override void _EnterTree()
        {
            base._EnterTree();
            timer = GetTree().CreateTimer(timeToLive);
            timer.Timeout += Despawn;
            Show();
        }

        public override void _Ready()
        {
            hurtbox.Hit += OnHit;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            // stop timer
            timer.Timeout -= Despawn;
            CallDeferred(MethodName.ResetPhysics);
            Hide();
        }

        private void OnHit(Hitbox hitbox)
        {
            //TODO: maybe deal damage?
            Despawn();
        }

        private void Despawn()
        {
            ObjectPooler.Instance.ReturnObject(pool.Name, this);
        }

        public void SetPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public ObjectPool GetPool()
        {
            return pool;
        }

        private void ResetPhysics()
        {
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;
        }
    }
}