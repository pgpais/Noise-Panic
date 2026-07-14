using Godot;
using System;
using WeWereDumb.Core.Pooling;
using WeWereDumb.Gameplay.Actors;

namespace WeWereDumb.Gameplay.EnemySpawning;

[GlobalClass]
public partial class EnemySpawner : Node3D
{
	[Export] bool startSpawning = true;
	[Export] bool automaticSpawn = true;

	[Export] float timeBetweenSpawns = 3f;

	/// <summary>
	/// Maximum distance to player to start spawning
	/// </summary>
	[Export] float distanceToPlayer = 10f;

	/// <summary>
	/// Distance around spawn point to spawn enemies
	/// </summary>
	[Export] float spawnDistance = 5f;

	[Export] int spawnsPerWave = 1;
	[Export] int numberOfWaves;
	[Export] int maxSimultaneousEnemies;

	[Export] string poolName = "enemies";
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
				WaitTime = timeBetweenSpawns,
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

	public void Spawn(Enemy enemy, Vector3? position = default)
	{
		Vector3 spawnPos = position.HasValue ? position.Value : GetRandomSpawnPosition();
		enemy.GlobalPosition = spawnPos;
	}

	public Vector3 GetRandomSpawnPosition()
	{
		return GlobalPosition + new Vector3((float)GD.RandRange(-spawnDistance, spawnDistance), 0, (float)GD.RandRange(-spawnDistance, spawnDistance));
	}
}
