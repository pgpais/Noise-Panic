using Godot;
using System;
using WeWereDumb.Gameplay.Storage;
using WeWereDumb.Gameplay.Storage.UI;

public partial class ResourceStorageScreen : Node3D
{
	[Export] StorageOverview storageOverview;
	[Export] ResourceStorage storage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		storageOverview.Initialize(storage);
	}
}
