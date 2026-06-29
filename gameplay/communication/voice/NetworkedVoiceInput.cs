using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using WeWereDumb.Gameplay.Input;

[GlobalClass]
public partial class NetworkedVoiceInput : Node3D, IInput
{
	[Signal] public delegate void VoiceRecordingStateChangedEventHandler(bool state);
	[Signal] public delegate void VoiceDataReceivedEventHandler(Array<byte[]> buffer);

	[Export] Node voiceInput;

	Array<byte[]> incomingVoiceData;

	bool isEnabled = true;

	public override void _Process(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		if (!isEnabled) return;

		if (Input.IsActionPressed("voice"))
		{
			Rpc(MethodName.SyncVoiceRecordingState, true);
			voiceInput.Call("process_record");
			var buffer = GetVoiceDataBuffer();
			Rpc(MethodName.SyncVoiceBuffer, buffer);
		}

		if (Input.IsActionJustReleased("voice"))
		{
			Rpc(MethodName.SyncVoiceRecordingState, false);
		}
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

	public bool HasVoiceData()
	{
		return incomingVoiceData != null && incomingVoiceData.Count > 0;
	}

	public void Enable()
	{
		isEnabled = true;
	}

	public void Disable()
	{
		isEnabled = false;
	}

	[Rpc(CallLocal = true)]
	private void SyncVoiceRecordingState(bool state)
	{
		EmitSignal(SignalName.VoiceRecordingStateChanged, state);
	}

	private void SetVoiceBuffer(Array<byte[]> buffer)
	{
		incomingVoiceData = buffer;
		EmitSignal(SignalName.VoiceDataReceived, buffer);
	}
}
