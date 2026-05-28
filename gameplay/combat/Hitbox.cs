using System;
using Godot;

[GlobalClass]
public partial class Hitbox : Area3D
{
	[Signal] public delegate void HitTakenEventHandler(int damage);


	public void TakeHit(int damage)
	{
		EmitSignal(SignalName.HitTaken, damage);
	}
}
