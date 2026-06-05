using System.Collections.Generic;
using Godot;
using WeWereDumb.Gameplay.Resources;

namespace WeWereDumb.Gameplay.Storage
{
    [GlobalClass]
    public partial class ResourceStorage : Node
    {
        /// <summary>Fired on all peers whenever a resource amount changes.</summary>
        [Signal] public delegate void ResourcesChangedEventHandler(string resourceId, int newAmount);

        private readonly Dictionary<string, int> _resourceAmounts = new();

        /// <summary>Returns how many units of <paramref name="resourceId"/> are in storage.</summary>
        public int GetAmount(string resourceId)
        {
            if (resourceId == null) return 0;
            if (!_resourceAmounts.ContainsKey(resourceId)) return 0;

            return _resourceAmounts[resourceId];
        }
        public int GetAmount(GameResourceData resource) => GetAmount(resource.Id);

        /// <summary>
        /// Adds <paramref name="amount"/> units to the pool. Server-only; synced to all clients.
        /// </summary>
        public void Add(string resourceId, int amount)
        {
            if (!IsMultiplayerAuthority()) return;
            if (resourceId == null || amount <= 0) return;

            int currentAmount = GetAmount(resourceId);

            int resultAmount = currentAmount + amount;
            _resourceAmounts[resourceId] = resultAmount;

            EmitSignal(SignalName.ResourcesChanged, resourceId, resultAmount);
            Rpc(MethodName.SyncAmount, resourceId, resultAmount);
            GD.Print($"ResourceStorage: +{amount} {resourceId} → {resultAmount}");
        }
        public void Add(GameResourceData resource, int amount) => Add(resource.Id, amount);

        /// <summary>
        /// Deducts <paramref name="amount"/> if enough is in stock. Returns true on success.
        /// Server-only; synced to all clients.
        /// </summary>
        public bool TrySpend(string resourceId, int amount)
        {
            if (!IsMultiplayerAuthority()) return false;
            if (resourceId == null || amount <= 0) return false;

            int current = GetAmount(resourceId);
            if (current < amount) return false;

            int resultAmount = current - amount;
            _resourceAmounts[resourceId] = resultAmount;

            EmitSignal(SignalName.ResourcesChanged, resourceId, resultAmount);
            Rpc(MethodName.SyncAmount, resourceId, resultAmount);
            GD.Print($"ResourceStorage: -{amount} {resourceId} → {resultAmount}");
            return true;
        }
        public bool TrySpend(GameResourceData resource, int amount) => TrySpend(resource.Id, amount);

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void SyncAmount(string resourceId, int newAmount)
        {
            _resourceAmounts[resourceId] = newAmount;
            EmitSignal(SignalName.ResourcesChanged, resourceId, newAmount);
        }
    }
}