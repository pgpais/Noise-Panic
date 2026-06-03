using Godot;
using System;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.EnemySpawning;

public partial class EnemySpawner : Node3D
{
	[Export] string poolName = "enemies";
	[Export] bool automaticSpawn = false;
	[Export] float spawnTimer = 3f;

	[Export] PackedScene enemyScene;

	Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ObjectPooler.Instance.CreatePool<Enemy>(poolName, enemyScene);

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

	public Enemy Spawn(string name = null, Node parent = null)
	{
		var enemy = ObjectPooler.Instance.RequestObject<Enemy>(poolName);
		enemy.GlobalPosition = GlobalPosition;

		if (name != null)
		{
			enemy.Name = name;
		}

		return enemy;
	}
}
