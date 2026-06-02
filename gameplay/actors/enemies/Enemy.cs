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

    public override void _EnterTree()
    {
        base._EnterTree();

        health.Reset();
        enemies.Add(this);
    }

    public override void _Ready()
    {
        base._Ready();

        health.HealthDepleted += Despawn;
    }

    private void Despawn()
    {
        ObjectPooler.Instance.ReturnObject(pool.Name, this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        enemies.Remove(this);
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
}