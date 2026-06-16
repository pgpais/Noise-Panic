using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
public partial class NetworkedVoiceInput : Node3D
{
	[Signal]
	public delegate void VoiceDataReceivedEventHandler(Array<byte[]> buffer);

	[Export] Node voiceInput;

	Array<byte[]> incomingVoiceData;

	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;

		if (Input.IsActionPressed("voice"))
		{
			voiceInput.Call("process_record");
			var buffer = GetVoiceDataBuffer();
			Rpc(MethodName.SyncVoiceBuffer, buffer);
		}
	}

	public byte[] GetVoiceDataPacket()
	{
		return voiceInput.Call("get_voice_data_packet").As<byte[]>();
	}

	public Array<byte[]> GetVoiceDataBuffer()
	{
		var buffer = voiceInput.Call("get_voice_data_buffer");
		return buffer.As<Array<byte[]>>();
	}

	[Rpc]
	public void SyncVoiceBuffer(Array<byte[]> buffer)
	{
		SetVoiceBuffer(buffer);
	}

	private void SetVoiceBuffer(Array<byte[]> buffer)
	{
		incomingVoiceData = buffer;
		EmitSignal(SignalName.VoiceDataReceived, buffer);
	}

	public bool HasVoiceData()
	{
		return incomingVoiceData != null && incomingVoiceData.Count > 0;
	}
}
