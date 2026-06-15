using Godot;

[GlobalClass]
public partial class NetworkedVoiceOutput : Node3D
{
	[Export] Node voiceOutput;
	[Export] NetworkedVoiceInput voiceInput;

	[ExportGroup("Voice Settings")]
	[Export] private int opusChunkSize = 960;

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
