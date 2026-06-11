using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] Button resumeButton;
	[Export] Button quitButton;

	bool isPaused = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resumeButton.Pressed += Unpause;
		quitButton.Pressed += () => { GetTree().Quit(); };

		Unpause();
	}

	public void Pause()
	{
		isPaused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Show();
	}

	public void Unpause()
	{
		isPaused = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Hide();
	}

	override public void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
		{
			if (isPaused)
			{
				Unpause();
			}
			else
			{
				Pause();
			}
		}
	}
}
