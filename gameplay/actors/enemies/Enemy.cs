using System;
using System.Collections.Generic;
using Godot;
using WeWereDumb.Core.Pooling;

namespace WeWereDumb.Gameplay.Actors;

public partial class Enemy : Actor, IPooledObject
{
	public static List<Enemy> enemies = new List<Enemy>();

	[Export] HealthComponent health;
	[Export] CharacterController controller;

	[ExportGroup("Attack")]
	[Export] Hurtbox hurtbox;
	[Export] int attackDamage = 5;

	ObjectPool pool;

	public override void _Ready()
	{
		base._Ready();

		health.HealthDepleted += OnHealthDepleted;
		if (hurtbox != null) hurtbox.HitHitbox += OnHit;
	}

	private void OnHit(Hitbox hitbox)
	{
		if (hitbox.Owner == this) return;
		hitbox.TakeHit(attackDamage);
	}

	private void OnHealthDepleted()
	{
		if (!IsMultiplayerAuthority()) return;

		Rpc(MethodName.Die);
	}

	private void Despawn()
	{
		//TODO:
		ObjectPooler.Instance.ReturnObject(pool.Name, this);
	}

	public void MoveTowards(Vector2 position2D)
	{
		Vector2 thisPos2D = new(GlobalPosition.X, GlobalPosition.Z);
		Vector2 dir = thisPos2D.DirectionTo(position2D);
		controller.SetAbsoluteMovementInput(dir);
	}

	public void SetPool(ObjectPool pool)
	{
		this.pool = pool;
	}

	public ObjectPool GetPool()
	{
		return pool;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	private void Die()
	{
		CallDeferred(MethodName.Despawn);
		EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, this);
	}

	public void OnSpawn()
	{
		ProcessMode = ProcessModeEnum.Inherit;
		Show();

		health.Reset();
		enemies.Add(this);
	}

	public void OnDespawn()
	{
		ProcessMode = ProcessModeEnum.Disabled;
		Hide();

		enemies.Remove(this);
	}

	public Player GetClosestPlayer()
	{
		Player closestPlayer = null;
		float closestDistance = float.MaxValue;

		foreach (var player in Player.秃)
		{
			float distance = player.GlobalPosition.DistanceTo(GlobalPosition);
			if (distance < closestDistance)
			{
				closestPlayer = player;
				closestDistance = distance;
			}
		}

		return closestPlayer;
	}
}
