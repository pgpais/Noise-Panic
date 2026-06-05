using System;
using Godot;
using Godot.Collections;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    public partial class EnemySpawnController : Node
    {
        [Export] Node spawnPath;
        [Export] Array<EnemySpawner> Spawners;
        [Export] float spawnTimer = 3f;
        [Export] float spawnStartInterval = 5f;

        Timer timer;

        bool isSpawning = false;

        public override void _Ready()
        {
            foreach (var spawner in Spawners)
            {
                spawner.DisableAutomaticSpawn();
            }

            if (!IsMultiplayerAuthority()) return;

            timer = new Timer
            {
                WaitTime = spawnTimer,
                OneShot = false,
                Autostart = false
            };
            AddChild(timer);

            GetTree().CreateTimer(spawnStartInterval).Timeout += StartSpawning;
            timer.Timeout += OnTimerTimeout;
        }

        public void StartSpawning()
        {
            isSpawning = true;
            timer.Start();
        }

        public void StopSpawning()
        {
            isSpawning = false;
            timer.Stop();
        }

        private void OnTimerTimeout()
        {
            Spawn();
        }

        private void Spawn()
        {
            if (!IsMultiplayerAuthority()) return;

            int index = GD.RandRange(0, Spawners.Count - 1);

            var enemy = Spawners[index].Spawn();
            Rpc(MethodName.SyncSpawn, index, enemy.Name);
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncSpawn(int index, string name)
        {
            Spawners[index].Spawn(name: name);
        }
    }
}