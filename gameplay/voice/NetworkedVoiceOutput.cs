using Godot;

[GlobalClass]
public partial class NetworkedVoiceOutput : Node3D
{
	const string VoiceOutputGD = "uid://do5g5ejibuqs5";

	[Export] NetworkedVoiceInput voiceInput;

	[ExportGroup("Voice Settings")]
	[Export] private int opusChunkSize = 960;

	Node voiceOutput;

	public NetworkedVoiceOutput()
	{
		GD.Print("Getting voice output script");
		var script = ResourceLoader.Load<GDScript>(VoiceOutputGD);
		voiceOutput = (Node)(GodotObject)script.New();
		GD.Print("Voice output script loaded");
		AddChild(voiceOutput);
		GD.Print("NetworkedVoiceOutput ready");
	}

	public void SetVoiceInput(NetworkedVoiceInput voiceInput)
	{
		this.voiceInput = voiceInput;
	}

	override public void _Process(double delta)
	{
		bool hasAvailableFrames = voiceOutput.Call("playback_has_available_frames").As<bool>();
		while (hasAvailableFrames && voiceInput.HasVoiceData())
		{
			var incomingVoicePacket = voiceInput.GetVoiceDataPacket();
			voiceOutput.Call("play_voice_packet", incomingVoicePacket);
		}
	}
}
