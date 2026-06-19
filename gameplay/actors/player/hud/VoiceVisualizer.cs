using System;
using Godot;

public partial class VoiceVisualizer : TextureRect
{
	[Export] NetworkedVoiceInput voiceInput;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();

		voiceInput.VoiceRecordingStateChanged += OnVoiceRecordingStateChanged;
	}

	private void OnVoiceRecordingStateChanged(bool state)
	{
		if (state)
		{
			OnVoiceRecordingStarted();
		}
		else
		{
			OnVoiceRecordingStopped();
		}
	}

	private void OnVoiceRecordingStarted()
	{
		Show();
	}

	private void OnVoiceRecordingStopped()
	{
		Hide();
	}
}
