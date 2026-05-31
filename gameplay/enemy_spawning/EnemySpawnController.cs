using System;
using Godot;
using Godot.Collections;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    public partial class EnemySpawnController : Node
    {
        [Export] Array<EnemySpawner> Spawners;
        [Export] float spawnTimer = 3f;

        Timer timer;

        public override void _Ready()
        {
            foreach (var spawner in Spawners)
            {
                spawner.DisableAutomaticSpawn();
            }
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
            int index = GD.RandRange(0, Spawners.Count - 1);
            Spawners[index].Spawn();
        }
    }
}