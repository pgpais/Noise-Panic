using Godot;

namespace WeWereDumb.Gameplay.EnemySpawning
{
    [GlobalClass]
    public partial class EnemySpawnData : Resource
    {
        [Export] public string PoolName { get; private set; }
        [Export] public PackedScene Scene { get; private set; }

        /// <summary>
        /// This part of the code makes monsters spawn using spongebob underwear
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Spawn<T>() where T : Node
        {
            return Scene.Instantiate<T>();
        }
    }
}