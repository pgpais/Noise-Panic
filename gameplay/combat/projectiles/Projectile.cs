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
        Timer timer;

        public Projectile()
        {
            timer = new Timer
            {
                WaitTime = timeToLive,
                OneShot = true,
                Autostart = true
            };

            AddChild(timer);
        }

        public override void _Ready()
        {
            hurtbox.Hit += OnHit;
        }

        private void OnHit(Hitbox hitbox)
        {
            //TODO: maybe deal damage?
            Despawn();
        }

        private void Despawn()
        {
            ObjectPooler.Instance.CallDeferred(ObjectPooler.MethodName.ReturnObject, pool.Name, this);
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

        public void OnSpawn()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            Show();

            timer.Autostart = true;
            timer.Timeout += Despawn;
        }

        public void OnDespawn()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            Hide();

            if (timer.IsConnected(Timer.SignalName.Timeout, Callable.From(Despawn)))
            {
                timer.Timeout -= Despawn;
            }
            if (timer.IsStopped()) timer.Stop();

            CallDeferred(MethodName.ResetPhysics);
        }
    }
}