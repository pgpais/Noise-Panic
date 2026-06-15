using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
public partial class NetworkedVoiceInput : Node3D
{
	[Export] Node voiceInput;

	byte[] incomingVoiceData;

	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;

		if (Input.IsActionPressed("voice"))
		{
			voiceInput.Call("process_record");
		}
	}

	public byte[] GetVoiceDataPacket()
	{
		return voiceInput.Call("get_voice_data_packet").As<byte[]>();
	}

	public bool HasVoiceData()
	{
		return voiceInput.Call("has_voice_data").As<bool>();
	}
}
