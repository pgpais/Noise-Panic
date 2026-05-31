using Godot;
using System;

namespace WeWereDumb.Gameplay.EnemySpawning;

public partial class EnemySpawner : Node3D
{
	[Export] bool automaticSpawn = false;
	[Export] float spawnTimer = 3f;

	[Export] PackedScene enemyScene;

	Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (automaticSpawn)
		{
			timer = new Timer
			{
				WaitTime = spawnTimer,
				OneShot = false,
				Autostart = true
			};
			timer.Timeout += OnTimerTimeout;
			AddChild(timer);
		}
	}

	public void DisableAutomaticSpawn()
	{
		automaticSpawn = false;
		timer.Stop();
		timer.Timeout -= OnTimerTimeout;
	}

	private void OnTimerTimeout()
	{
		Spawn();
	}

	public void Spawn()
	{
		var enemy = enemyScene.Instantiate<Node3D>();
		enemy.GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.AddChild(enemy);
	}
}
