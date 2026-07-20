using System.Linq;
using Godot;
using Godot.Collections;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Combat.Projectiles.Components;
using WeWereDumb.Gameplay.Combat.Projectiles.Modifiers;

namespace WeWereDumb.Gameplay.Combat.Projectiles
{
    public partial class Projectile : Node3D, IPooledObject
    {
        [Signal] public delegate void HitEventHandler();
        [Signal] public delegate void HitboxHitEventHandler(Hitbox hitbox);

        public Node Shooter { get; private set; }

        [Export] int damage = 2;
        [Export] float timeToLive = 2f;
        [Export] float speed = 10f;

        [Export] Node3D visuals;
        [Export] Hurtbox hurtbox;
        [Export] GpuParticles3D particles;

        [ExportGroup("Modifiers")]
        [Export] Node modifiersNode;
        private Array<ProjectileModifier> baseModifiers;

        [ExportGroup("Components")]
        [Export] VelocityComponent velocityComponent;

        private ObjectPool pool;
        Timer timer;

        private Vector3 _velocity;

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
            particles.Emitting = false;

            hurtbox.Hit += OnHit;
            hurtbox.HitHitbox += OnHitboxHit;
            particles.Finished += Despawn;
        }

        public override void _PhysicsProcess(double delta)
        {
            base._Process(delta);


            _velocity = Vector3.Zero;

            var movementModifiers = baseModifiers.OfType<ProjectileMovementModifier>();
            var timeRatio = (timeToLive - timer.TimeLeft) / timeToLive;
            foreach (var movementModifier in movementModifiers)
            {
                _velocity += movementModifier.CalculateVelocity(this, _velocity, (float)timeRatio);
            }


            DoMovement(delta);
        }

        public void SetShooter(Node shooter)
        {
            Shooter = shooter;
        }

        public void OnSpawn()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            Show();

            GetModifiers();

            visuals.Show();
            CallDeferred(MethodName.MonitorContacts, true);

            timer.Start();
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
            if (!timer.IsStopped()) timer.Stop();

            CallDeferred(MethodName.ResetPhysics);
        }

        public void SetPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public ObjectPool GetPool()
        {
            return pool;
        }

        public void AddSpeed(float speed)
        {
            this.speed += speed;
        }

        public void AddDamage(int damage)
        {
            this.damage += damage;
        }

        private void GetModifiers()
        {
            baseModifiers = new Array<ProjectileModifier>();
            foreach (Node child in modifiersNode.GetChildren())
            {
                if (child is ProjectileModifier modifier)
                {
                    baseModifiers.Add(modifier);
                }
            }
        }

        private void DoMovement(double delta)
        {
            Position += _velocity * (float)delta;
        }

        private void ResetPhysics()
        {
            _velocity = Vector3.Zero;
        }

        private void MonitorContacts(bool enabled)
        {
            hurtbox.Monitoring = enabled;
            hurtbox.Monitorable = enabled;
        }

        private void EmitParticles()
        {
            particles.Restart();
        }

        private void DealDamage(Hitbox hitbox)
        {
            hitbox.TakeHit(damage);
        }

        private void Despawn()
        {
            GD.Print("Despawning projectile");
            ObjectPooler.Instance.CallDeferred(ObjectPooler.MethodName.ReturnObject, pool.Name, this);
        }

        private void OnHit()
        {
            CallDeferred(MethodName.MonitorContacts, false);

            ResetPhysics();
            visuals.Hide();
            EmitParticles();

            EmitSignal(SignalName.Hit);
        }

        private void OnHitboxHit(Hitbox hitbox)
        {
            if (hitbox.Owner == Shooter) return;
            DealDamage(hitbox);

            EmitSignal(SignalName.HitboxHit, hitbox);
        }
    }
}