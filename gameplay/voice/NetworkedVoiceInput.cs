using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
public partial class NetworkedVoiceInput : Node3D
{
	const string VoiceInputGD = "uid://djd16o5tc6iqh";

	Node voiceInput;

	byte[] incomingVoiceData;

	public override void _Ready()
	{
		GD.Print("Getting voice input script");
		var script = ResourceLoader.Load<GDScript>(VoiceInputGD);
		voiceInput = (Node)(GodotObject)script.New();
		GD.Print("Voice input script loaded");
		AddChild(voiceInput);
		GD.Print("NetworkedVoiceInput ready");
	}

	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;

		if (Input.IsActionPressed("voice"))
		{
			voiceInput.Call("process_record");

			// GD.Print(voiceInput.Get("voice_data"));
			// GD.Print(voiceInput.Get("opus_packets_buffer"));
			var voiceData = voiceInput.Get("voice_data").As<byte[]>();
			if (voiceData == null || voiceData.Length == 0)
			{
				GD.PushError("voice_data is null");
				return;
			}
			SendVoiceData(voiceData);
		}
	}

	public byte[] GetVoiceDataPacket()
	{
		return voiceInput.Call("get_voice_data").As<byte[]>();
	}

	private void SendVoiceData(byte[] voiceData)
	{
		Rpc(nameof(SyncVoiceData), voiceData);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void SyncVoiceData(byte[] voiceData)
	{
		GD.Print("SyncVoiceData called");
		voiceInput.Call("add_voice_data", voiceData);
		GD.Print("voiceData size: " + voiceData.Length);
	}
}
