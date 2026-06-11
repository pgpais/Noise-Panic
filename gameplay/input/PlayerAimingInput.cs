using Godot;
using WeWereDumb.Gameplay.Actors;
using WeWereDumb.Gameplay.Combat.Weapons;

namespace WeWereDumb.Gameplay.Input
{
	[GlobalClass]
	public partial class PlayerAimingInput : Node
	{
		[Export] AimingController aimingController;
		[Export] Gun gun;

		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			if (!IsMultiplayerAuthority()) return;

			if (Godot.Input.IsActionJustPressed("shoot"))
			{
				gun.Shoot();
			}
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			base._UnhandledInput(@event);

			if (@event is InputEventMouseMotion mouseMotion)
			{
				var mouseMovement = mouseMotion.ScreenRelative;
				aimingController.MouseMovementInput = mouseMovement;
			}
		}


		public void Enable()
		{
			ProcessMode = ProcessModeEnum.Inherit;
		}

		public void Disable()
		{
			ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}
