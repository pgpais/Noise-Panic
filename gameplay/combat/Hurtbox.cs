using Godot;

[GlobalClass]
public partial class Hurtbox : Area3D
{
	[Signal] public delegate void HitHitboxEventHandler(Hitbox hitbox);
	[Signal] public delegate void HitEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	private void OnBodyEntered(Node3D body)
	{
		GD.Print("No Hitbox entered");
		EmitSignal(SignalName.Hit, null);
	}


	private void OnAreaEntered(Area3D area)
	{
		EmitSignal(SignalName.Hit, null);
		
		if (area is not Hitbox hitbox) return;
		GD.Print("Hitbox entered");
		EmitSignal(SignalName.HitHitbox, hitbox);
	}
	
	
}
