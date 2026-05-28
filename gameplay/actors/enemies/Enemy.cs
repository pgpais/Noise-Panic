using System.Collections.Generic;
using Godot;

namespace WeWereDumb.Gameplay.Actors;

public partial class Enemy : Actor
{
    public static List<Enemy> enemies = new List<Enemy>();

    [Export] HealthComponent health;
    [Export] CharacterController controller;

    public override void _EnterTree()
    {
        base._EnterTree();

        enemies.Add(this);
    }

    public override void _Ready()
    {
        base._Ready();

        health.HealthDepleted += QueueFree;
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
}