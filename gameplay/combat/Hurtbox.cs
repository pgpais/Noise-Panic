using Godot;

[GlobalClass]
public partial class Hurtbox : Area3D
{
	[Export] int damage = 2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area3D area)
	{
		if (area is Hitbox hitbox)
		{
			hitbox.TakeHit(damage);
		}
	}
}
