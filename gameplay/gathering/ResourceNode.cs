using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;
using WeWereDumb.Gameplay.Resources;

public partial class ResourceNode : StaticBody3D
{
	[Export] GameResourceData resource;

	[Export] Interactable interactable;

	[Export] Node3D dropPoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		interactable.InteractionStarted += OnInteractionStarted;
	}

	private void OnInteractionStarted(Interactor interactor)
	{
		var instance = resource.Scene.Instantiate<GameResource>();
		instance.SetData(resource);
		GetTree().CurrentScene.AddChild(instance, true);

		instance.GlobalPosition = dropPoint.GlobalPosition;
		instance.GlobalRotation = dropPoint.GlobalRotation;


		Rpc(MethodName.CreateResource, instance.Name);
	}

	//TODO: should this be called only by authority
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void CreateResource(string instanceName)
	{
		//TODO: create and drop item on floor, let player pick it up
		var instance = resource.Scene.Instantiate<GameResource>();
		instance.SetData(resource);
		GetTree().CurrentScene.AddChild(instance, true);
		instance.Name = instanceName;

		instance.GlobalPosition = dropPoint.GlobalPosition;
		instance.GlobalRotation = dropPoint.GlobalRotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
