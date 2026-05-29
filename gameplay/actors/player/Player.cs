using Godot;
using PhantomCamera;

namespace WeWereDumb.Gameplay.Actors;

public partial class Player : Actor
{
	[Export] Node3D phantomcamera3d;
	PhantomCamera3D pcam;

	public override void _Ready()
	{
		pcam = phantomcamera3d.AsPhantomCamera3D();
		if (IsMultiplayerAuthority())
		{
			pcam.Priority = 1;
		}
	}
}
