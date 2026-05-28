using Godot;
using WeWereDumb.Gameplay.Actors;

public partial class EnemyController : Node
{
	[Export] Player player;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		CallDeferred(MethodName.ControlEnemies);
	}

	private void ControlEnemies()
	{
		if (player == null)
		{
			player = GetTree().GetFirstNodeInGroup("players") as Player;
		}
		Vector2 playerPos2D = new(player.GlobalPosition.X, player.GlobalPosition.Z);

		foreach (var enemy in Enemy.enemies)
		{
			Vector2 enemyPos2D = new(enemy.GlobalPosition.X, enemy.GlobalPosition.Z);
			var dir = enemyPos2D.DirectionTo(playerPos2D);
			enemy.MoveTowards(dir);
		}
	}
}
