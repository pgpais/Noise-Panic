using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;
using WeWereDumb.Gameplay.Resources;

public partial class ResourceNode : StaticBody3D
{
	[Export] GameResourceData resource;

	[Export] Interactable interactable;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		interactable.InteractionStarted += OnInteractionStarted;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		var instance = resource.Scene.Instantiate<GameResource>();
		instance.SetData(resource);
		interactor.InteractActor.GetComponent<ResourceHolder>().AddResource(instance);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
