using Godot;
using System;

public partial class ObjectiveProgressBar : ProgressBar
{
	[Export] private GameRun gameRun;

	override public void _Ready()
	{
		base._Ready();
		gameRun.ObjectiveProgressChanged += OnObjectiveProgressChanged;

		MaxValue = 100;
		Value = 0;
	}

	public void SetProgress(float progress)
	{
		Value = progress;
	}

	private void OnObjectiveProgressChanged(float progress)
	{
		GD.Print(Multiplayer.GetUniqueId() + ": Goal progress: " + progress);
		SetProgress(progress * 100);
	}
}
