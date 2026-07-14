using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Input;
using WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class PlayerCharacterInput : Node, IInput
{
	[Export] CharacterController controller;
	[Export] Interactor interactor;
	[Export] ResourceHolder resourceHolder;

	bool isEnabled = true;

	public void Disable()
	{
		isEnabled = false;
	}

	public void Enable()
	{
		isEnabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		if (!isEnabled) return;

		HandleMovementInput();

		if (Input.IsActionJustPressed("interact"))
		{
			if (interactor.IsInteracting)
			{
				interactor.StopInteracting();
			}
			else
			{
				interactor.StartInteracting();
			}
		}

		if (Input.IsActionJustPressed("drop"))
		{
			resourceHolder.TryToDropResource();
		}

		if (Input.IsActionJustPressed("jump"))
		{
			controller.WantsToJump = true;
		}

		if (Input.IsActionPressed("run"))
		{
			controller.WantsToRun = true;
		}

		if (Input.IsActionJustPressed("dash"))
		{
			controller.WantsToDash = true;
		}
	}

	private void HandleMovementInput()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		controller.SetRelativeMovementInput(inputDir);
	}
}
