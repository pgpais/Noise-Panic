using System.Linq;
using Godot;
using Godot.Collections;
using WeWereDumb.Gameplay.Actors;

public partial class EnemyController : Node
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        ControlEnemies();
    }

    private void ControlEnemies()
    {
        foreach (var enemy in Enemy.enemies)
        {
            Player closestPlayer = enemy.GetClosestPlayer();
            Vector2 playerPos2D = new(closestPlayer.GlobalPosition.X, closestPlayer.GlobalPosition.Z);
            Vector2 enemyPos2D = new(enemy.GlobalPosition.X, enemy.GlobalPosition.Z);
            var dir = enemyPos2D.DirectionTo(playerPos2D);
            enemy.MoveTowards(dir);
        }
    }
}
