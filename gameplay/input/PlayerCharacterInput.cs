using Godot;
using WeWereDumb.Gameplay.Gathering;
using WeWereDumb.Gameplay.Interaction;

[GlobalClass]
public partial class PlayerCharacterInput : Node
{
	[Export] CharacterController controller;
	[Export] Interactor interactor;
	[Export] ResourceHolder resourceHolder;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		HandleMovementInput();

		if (Input.IsActionJustPressed("interact"))
		{
			interactor.StartInteracting();
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			interactor.StopInteracting();
		}

		if (Input.IsActionJustPressed("drop"))
		{
			resourceHolder.TryToDropResource();
		}
	}

	private void HandleMovementInput()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		controller.MovInput = inputDir;
	}
}