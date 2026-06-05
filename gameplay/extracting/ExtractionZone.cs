using Godot;
using System;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;
using WeWereDumb.Gameplay.Resources;
using WeWereDumb.Gameplay.Storage;

public partial class ExtractionZone : StaticBody3D
{
	[Signal] public delegate void ResourceExtractedEventHandler(GameResource resource);

	[Export] Interactable interactable;
	[Export] ResourceStorage storage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		interactable.InteractionStarted += OnInteractionStarted;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		ResourceHolder holder = interactor.InteractActor.GetComponent<ResourceHolder>();

		if (holder != null && holder.HasResource())
		{
			var resource = holder.Resource;

		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public void ExtractResource(NodePath resourcePath)
	{
		var resource = GetNode<GameResource>(resourcePath);

		// storage.Add(resource.Data, 1);
		EmitSignal(SignalName.ResourceExtracted, resource);

		resource.Rpc(GameResource.MethodName.Destroy);
		// resource.QueueFree();
	}
}
