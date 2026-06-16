using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class NetworkedVoiceOutput : Node3D
{
	const string VoiceOutputGD = "uid://do5g5ejibuqs5";

	[Export] AudioStreamPlayer3D audioStreamPlayer3D;
	[Export] NetworkedVoiceInput voiceInput;

	[ExportGroup("Voice Settings")]
	[Export] private int opusChunkSize = 960;

	Node voiceOutput;

	public override void _EnterTree()
	{
		if (voiceOutput != null) return;

		GD.Print("Getting voice output script");
		var script = ResourceLoader.Load<GDScript>(VoiceOutputGD);
		GD.Print("audioStreamPlayer3D: " + audioStreamPlayer3D);
		if (audioStreamPlayer3D != null)
		{
			SetAudioStreamPlayer3D(audioStreamPlayer3D.Duplicate() as AudioStreamPlayer3D);
		}
		else
		{
			GD.Print("No audio stream player, creating voice output node");
			voiceOutput = (Node)(GodotObject)script.New();
			AddChild(voiceOutput, true);
		}
	}

	public override void _Ready()
	{
		if (voiceInput != null)
		{
			voiceInput.VoiceDataReceived += OnVoiceDataReceived;
		}
		GD.Print("NetworkedVoiceOutput ready");
	}

	private void OnVoiceDataReceived(Array<byte[]> buffer)
	{
		voiceOutput?.Call("play_voice_buffer", buffer);
	}

	override public void _Process(double delta)
	{
		voiceOutput.Call("process_playback");
	}

	public void SetAudioStreamPlayer3D(AudioStreamPlayer3D audioStreamPlayer3D)
	{
		var script = ResourceLoader.Load<GDScript>(VoiceOutputGD);
		audioStreamPlayer3D.SetScript(script);

		voiceOutput = audioStreamPlayer3D;
		GD.Print("Setting audio stream player script");

		AddChild(voiceOutput, true);
	}

	public void SetVoiceInput(NetworkedVoiceInput voiceInput)
	{
		this.voiceInput = voiceInput;
		voiceInput.VoiceDataReceived += OnVoiceDataReceived;
		GD.Print("NetworkedVoiceOutput ready");
	}
}
