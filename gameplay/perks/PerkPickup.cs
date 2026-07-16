using Godot;
using System;
using WeWereDumb.Gameplay.Actors;

public partial class PerkPickup : Area3D
{
	[Export] Perk perk;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Player player)
		{
			player.PickupPerk();
			QueueFree();
		}
	}
}
