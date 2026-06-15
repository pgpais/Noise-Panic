using Godot;

[GlobalClass]
public partial class NetworkedVoiceOutput : Node3D
{
	const string VoiceOutputGD = "uid://do5g5ejibuqs5";

	[Export] NetworkedVoiceInput voiceInput;

	[ExportGroup("Voice Settings")]
	[Export] private int opusChunkSize = 960;

	Node voiceOutput;

	override public void _Ready()
	{
		GD.Print("Getting voice output script");
		var script = ResourceLoader.Load<GDScript>(VoiceOutputGD);
		voiceOutput = (Node)(GodotObject)script.New();
		GD.Print("Voice output script loaded");
		AddChild(voiceOutput);
		GD.Print("NetworkedVoiceOutput ready");
	}

	override public void _Process(double delta)
	{
		var incomingVoicePacket = voiceInput.GetVoiceDataPacket();
		if (incomingVoicePacket == null || incomingVoicePacket.Length == 0)
		{
			return;
		}
		voiceOutput.Call("play_voice_packet", incomingVoicePacket);
		// voiceOutput.Call("process_playback");
	}
}
