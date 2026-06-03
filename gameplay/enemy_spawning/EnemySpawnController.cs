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

        Timer timer;

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
                Autostart = true
            };
            timer.Timeout += OnTimerTimeout;
            AddChild(timer);
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