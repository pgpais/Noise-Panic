using Godot;

namespace WeWereDumb.Gameplay.Resources
{
    public partial class GameResourceRegistry : Node
    {
        const string RESOURCE_COLLECTION_UID = "uid://dwsijsqoyfvq1";

        public static GameResourceRegistry Instance;

        GameResourceDataCollection _collection;

        public override void _Ready()
        {
            base._Ready();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                GD.PrintErr("GameResourceRegistry already exists");
                QueueFree();
            }

            _collection = ResourceLoader.Load<GameResourceDataCollection>(RESOURCE_COLLECTION_UID);
        }

        public GameResourceData GetResourceById(string id) => _collection.GetResource(id);
    }
}