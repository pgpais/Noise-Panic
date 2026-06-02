using Godot;
using System;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;
using WeWereDumb.Gameplay.Resources;

public partial class ExtractionZone : StaticBody3D
{
	[Signal] public delegate void ResourceExtractedEventHandler(GameResource resource);

	[Export] Interactable interactable;

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
			var resource = holder.DropResource();

			ExtractResource(resource);
		}
	}

	public void ExtractResource(GameResource resource)
	{
		//TODO:

		resource.QueueFree();
	}
}
