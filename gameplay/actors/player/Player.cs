using Godot;
using PhantomCamera;

public partial class Player : CharacterBody3D
{
	PhantomCamera3D cam;

	public override void _Ready()
	{
		cam = GetNode<Node3D>("PhantomCamera3D").AsPhantomCamera3D();
		if (IsMultiplayerAuthority())
		{
			cam.FollowTarget = this;
			cam.LookAtTarget = this;
			cam.Priority = 1;
		}
	}
}
