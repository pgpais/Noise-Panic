
using Godot;
using Godot.Collections;

namespace WeWereDumb.Gameplay.Resources
{
    [GlobalClass]
    public partial class GameResourceDataCollection : Resource
    {
        [Export] Array<GameResourceData> resources = new Array<GameResourceData>();

        private Dictionary<string, GameResourceData> resourcesById;

        public GameResourceData GetResource(string id)
        {
            if (resourcesById == null)
            {
                Configure();
            }

            return resourcesById[id];
        }

        public void Configure()
        {
            resourcesById = new Dictionary<string, GameResourceData>();
            foreach (GameResourceData resource in resources)
            {
                resourcesById[resource.Id] = resource;
            }
        }
    }
}