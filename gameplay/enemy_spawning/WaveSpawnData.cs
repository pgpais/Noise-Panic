using Godot;
using Godot.Collections;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    [GlobalClass]
    public partial class WaveSpawnData : Resource
    {
        [Export(PropertyHint.NodePathValidTypes, nameof(EnemySpawner))]
        public NodePath SpawnerPath { get; private set; }
        [Export]
        public Array<EnemySpawnData> EnemySpawns { get; private set; } = new Array<EnemySpawnData>();
    }
}