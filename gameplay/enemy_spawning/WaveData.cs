using Godot;
using Godot.Collections;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    [GlobalClass]
    public partial class WaveData : Resource
    {
        [Export] public Array<WaveSpawnData> WaveSpawns { get; private set; } = new Array<WaveSpawnData>();

        public Array<Enemy> Spawn(Node pathReferenceNode)
        {
            var enemies = new Array<Enemy>();
            foreach (var waveSpawn in WaveSpawns)
            {
                foreach (var enemySpawnData in waveSpawn.EnemySpawns)
                {
                    ObjectPool objectPool = ObjectPooler.Instance.GetPool(enemySpawnData.PoolName);
                    if (objectPool == null)
                    {
                        objectPool = ObjectPooler.Instance.CreatePool<Enemy>(enemySpawnData.PoolName, enemySpawnData.Scene);
                    }

                    var enemy = objectPool.RetrieveObject() as Enemy;
                    enemies.Add(enemy);

                    var spawner = pathReferenceNode.GetNode<EnemySpawner>(waveSpawn.SpawnerPath);

                    enemy.GlobalPosition = spawner.GetRandomSpawnPosition();
                    enemy.OnSpawn();
                }
            }

            return enemies;
        }
    }
}