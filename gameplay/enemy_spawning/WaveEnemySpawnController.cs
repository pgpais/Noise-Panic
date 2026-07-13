using System;
using Godot;
using Godot.Collections;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    public partial class WaveEnemySpawnController : Node
    {
        [Export] Array<WaveData> waveSpawns;

        Dictionary<Enemy, bool> currentWaveEnemiesKilled;
        int currentWave = 0;

        public override void _Ready()
        {
            base._Ready();

            SpawnWave();
        }

        public void SpawnWave()
        {
            var wave = waveSpawns[currentWave];

            currentWaveEnemiesKilled = new Dictionary<Enemy, bool>();
            foreach (var enemy in wave.Spawn(this))
            {
                currentWaveEnemiesKilled.Add(enemy, false);
                enemy.EnemyDied += () => OnEnemyDied(enemy);
            }
            currentWave++;
        }

        private void OnEnemyDied(Enemy enemy)
        {
            enemy.EnemyDied -= () => OnEnemyDied(enemy);

            currentWaveEnemiesKilled[enemy] = true;
            bool allEnemiesKilled = true;
            foreach (var waveEnemy in currentWaveEnemiesKilled)
            {
                if (!waveEnemy.Value)
                {
                    allEnemiesKilled = false;
                    break;
                }
            }

            if (allEnemiesKilled)
            {
                GD.Print("Wave completed");
                EventBus.Instance.EmitSignal(EventBus.SignalName.WaveDefeated);
            }
        }
    }
}