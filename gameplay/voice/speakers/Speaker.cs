using Godot;
using System;
using System.Collections.Generic;
using WeWereDumb.Gameplay.Actors;

[GlobalClass]
public partial class Speaker : Node3D
{
	[Export] PlayerSpawner playerSpawner;
	[Export] AudioStreamPlayer3D defaultAudioStreamPlayer3D;

	Dictionary<int, NetworkedVoiceOutput> voiceOutputs = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Speaker ready");
		GD.Print("Players: " + Player.秃.Count);
		foreach (var player in Player.秃)
		{
			AddSpeaker(player);
		}

		if (playerSpawner != null)
		{
			playerSpawner.PlayerSpawned += (Player player, int playerId) => AddSpeaker(player);
		}
	}

	public void AddSpeaker(Player player)
	{
		var networkedVoiceOutput = new NetworkedVoiceOutput();
		AddChild(networkedVoiceOutput);
		networkedVoiceOutput.SetVoiceInput(player.VoiceInput);

		var audioStreamPlayer3D = defaultAudioStreamPlayer3D.Duplicate() as AudioStreamPlayer3D;
		networkedVoiceOutput.SetAudioStreamPlayer3D(audioStreamPlayer3D);
		voiceOutputs.Add(player.GetMultiplayerAuthority(), networkedVoiceOutput);
	}
}
