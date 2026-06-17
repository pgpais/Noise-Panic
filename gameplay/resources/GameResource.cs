using System;
using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;

namespace WeWereDumb.Gameplay.Resources
{
    public partial class GameResource : StaticBody3D
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
            var resourceHolder = interactor.InteractActor.GetComponent<ResourceHolder>();
            if (resourceHolder.HasResource()) return;

            resourceHolder.AddResource(this);
            interactor.CancelInteraction();
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

        public void Enable()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            interactable.Enable();
        }

        public void Disable()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            interactable.HideHighlight();
            interactable.Disable();
        }
    }
}