using Godot;
using System;

public partial class GameHud : Control
{
	public override void _Ready()
	{
		base._Ready();

		EventBus.Instance.RunEnded += OnRunEnded;
	}

	private void OnRunEnded(bool success)
	{
		Hide();
	}
}
