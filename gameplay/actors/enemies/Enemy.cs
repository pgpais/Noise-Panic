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

    ObjectPool pool;

    public override void _Ready()
    {
        base._Ready();

        health.HealthDepleted += OnHealthDepleted;
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

    public void MoveTowards(Vector2 moveDir)
    {
        controller.MovInput = moveDir;
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

        foreach (var player in Player.players)
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
