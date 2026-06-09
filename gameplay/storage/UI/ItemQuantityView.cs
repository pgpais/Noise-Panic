using Godot;
using System;
using WeWereDumb.Gameplay.Resources;

namespace WeWereDumb.Gameplay.Storage.UI;

public partial class ItemQuantityView : HBoxContainer
{
	[ExportGroup("References")]
	[Export] Label _quantityLabel;
	[Export] Label _itemNameLabel;
	[Export] TextureRect _itemIcon;

	GameResourceData _resource;

	public void SetResource(GameResourceData resource)
	{
		_resource = resource;
		_itemNameLabel.Text = _resource.Name;
		// _itemIcon.Texture = _resource.Icon;
	}

	public void SetQuantity(int quantity)
	{
		_quantityLabel.Text = "x" + quantity.ToString();
	}
}
