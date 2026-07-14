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
    }
}