using Godot;
using Godot.Collections;
using WeWereDumb.Core.Pooling;
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

            CreatePools();
        }

        public void SpawnWave()
        {
            if (!IsMultiplayerAuthority()) return;
            if (currentWave >= waveSpawns.Count) return;

            currentWaveEnemiesKilled = new Dictionary<Enemy, bool>();

            var wave = waveSpawns[currentWave];

            GD.Print("Spawning wave " + currentWave);
            foreach (var waveData in wave.WaveSpawns)
            {
                var spawner = GetNode<EnemySpawner>(waveData.SpawnerPath);
                foreach (var enemySpawnData in waveData.EnemySpawns)
                {
                    SpawnEnemy(spawner, enemySpawnData);
                }
            }

            currentWave++;
        }

        public void AddWave(WaveData waveData)
        {
            waveSpawns.Add(waveData);
        }

        public void SetWaves(Array<WaveData> waves)
        {
            waveSpawns = waves;
        }

        public void ClearWaves()
        {
            waveSpawns.Clear();
        }

        private void SpawnEnemy(EnemySpawner spawner, EnemySpawnData enemySpawnData)
        {
            GD.Print("Spawning enemy");
            var enemy = ObjectPooler.Instance.RequestObject<Enemy>(enemySpawnData.PoolName);

            spawner.Spawn(enemy);
            GD.Print("Syncing spawn");
            Rpc(MethodName.SyncSpawn, spawner.GetPath(), enemySpawnData.PoolName, enemy.GetPath(), enemy.GlobalPosition);

            currentWaveEnemiesKilled.Add(enemy, false);
            enemy.EnemyDied += () => OnEnemyDied(enemy);
        }

        [Rpc()]
        private void SyncSpawn(NodePath spawnerPath, string poolName, NodePath enemyPath, Vector3 position)
        {
            GD.Print("Received spawn");
            var enemy = GetNode<Enemy>(enemyPath);
            var spawner = GetNode<EnemySpawner>(spawnerPath);

            enemy = ObjectPooler.Instance.RequestSpecificObject(enemy, poolName);
            spawner.Spawn(enemy, position);

            enemy.GlobalPosition = position;
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

        private void CreatePools()
        {
            foreach (var waveData in waveSpawns)
            {
                foreach (var waveSpawn in waveData.WaveSpawns)
                {
                    foreach (var enemySpawn in waveSpawn.EnemySpawns)
                    {
                        ObjectPooler.Instance.CreatePool<Enemy>(enemySpawn.PoolName, enemySpawn.Scene);
                    }
                }
            }
        }
    }
}