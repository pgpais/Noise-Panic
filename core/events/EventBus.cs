using Godot;
using System;

public partial class EventBus : Node
{
	[Signal] public delegate void EnemyDiedEventHandler();
	[Signal] public delegate void PlayerDiedEventHandler();

	[Signal] public delegate void ItemExtractedEventHandler(string itemName);

	public static EventBus Instance { get; private set; }

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}
}
