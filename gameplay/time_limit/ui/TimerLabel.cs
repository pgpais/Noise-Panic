using Godot;
using System;

public partial class TimerLabel : Label
{
	[Export] Timer timeCountdown;

	override public void _Process(double delta)
	{
		SetTime((int)timeCountdown.TimeLeft);
	}

	public void SetTime(int seconds)
	{
		var minutes = seconds / 60;
		var remainingSeconds = seconds % 60;
		Text = $"{minutes:00}:{remainingSeconds:00}";
	}
}
