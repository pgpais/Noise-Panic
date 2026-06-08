using System;
using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;

namespace WeWereDumb.Gameplay.Resources
{
    public partial class GameResource : Node3D
    {
        [Export] Interactable interactable;

        public GameResourceData Data => data;
        GameResourceData data;

        public override void _Ready()
        {
            base._Ready();

            interactable.InteractionStarted += OnInteractionStarted;
        }

        private void OnInteractionStarted(Interactor interactor)
        {
            interactor.InteractActor.GetComponent<ResourceHolder>().AddResource(this);
        }

        public void SetData(GameResourceData data)
        {
            this.data = data;
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void Destroy()
        {
            QueueFree();
        }
    }
}