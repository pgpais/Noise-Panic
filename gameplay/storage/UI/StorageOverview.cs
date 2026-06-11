using Godot;
using System;
using System.Collections.Generic;
using WeWereDumb.Gameplay.Resources;
using WeWereDumb.Gameplay.Storage;

namespace WeWereDumb.Gameplay.Storage.UI;

public partial class StorageOverview : Control
{
	[Export] ResourceStorage storage;
	[Export] PackedScene ItemQuantityViewScene;

	[Export] Container container;

	Dictionary<string, ItemQuantityView> itemQuantityViews = new Dictionary<string, ItemQuantityView>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var child in container.GetChildren())
		{
			child.QueueFree();
		}

		Initialize(storage);
	}

	public void Initialize(ResourceStorage storage)
	{
		this.storage = storage;
		if (storage != null)
		{
			storage.ResourcesChanged += OnResourcesChanged;
		}
	}

	private void OnResourcesChanged(string resourceId, int newAmount)
	{
		if (itemQuantityViews.ContainsKey(resourceId))
		{
			if (newAmount == 0)
			{
				itemQuantityViews[resourceId].QueueFree();
			}
			else
			{
				itemQuantityViews[resourceId].SetQuantity(newAmount);
			}
		}
		else
		{
			CreateNewEntry(resourceId, newAmount);
		}
	}

	public void CreateNewEntry(string resourceId, int amount = 0)
	{
		ItemQuantityView itemQuantityView = ItemQuantityViewScene.Instantiate<ItemQuantityView>();
		GameResourceData resourceData = GameResourceRegistry.Instance.GetResourceById(resourceId);
		itemQuantityView.SetResource(resourceData);
		itemQuantityView.SetQuantity(amount);
		container.AddChild(itemQuantityView);
	}
}
