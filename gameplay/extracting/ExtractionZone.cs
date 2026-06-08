using Godot;
using System;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;
using WeWereDumb.Gameplay.Resources;
using WeWereDumb.Gameplay.Storage;

public partial class ExtractionZone : Area3D
{
	[Signal] public delegate void ResourceExtractedEventHandler(GameResource resource);

	[Export] ResourceStorage storage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is GameResource resource)
		{
			ExtractResource(resource);
			Rpc(MethodName.ExtractResource, resource.GetPath());
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void ExtractResource(NodePath resourcePath)
	{
		var resource = GetNode<GameResource>(resourcePath);

		ExtractResource(resource);
	}

	public void ExtractResource(GameResource resource)
	{
		storage.Add(resource.Data, 1);
		EmitSignal(SignalName.ResourceExtracted, resource);
		resource.QueueFree();
	}
}
