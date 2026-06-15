using Godot;
using System;
using System.Collections.Generic;
using WeWereDumb.Gameplay.Actors;

[GlobalClass]
public partial class Speaker : Node3D
{
	Dictionary<int, NetworkedVoiceOutput> voiceOutputs = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Speaker ready");
		GD.Print("Players: " + Player.players.Count);
		foreach (var player in Player.players)
		{
			AddSpeaker(player);
		}
	}

	override public void _Process(double delta)
	{
		foreach (var player in Player.players)
		{
			if (!voiceOutputs.ContainsKey(player.GetMultiplayerAuthority()))
			{
				AddSpeaker(player);
			}
		}
	}

	public void AddSpeaker(Player player)
	{
		var voiceOutput = new NetworkedVoiceOutput();
		voiceOutputs.Add(player.GetMultiplayerAuthority(), voiceOutput);
		AddChild(voiceOutput, true);
		voiceOutput.SetVoiceInput(player.VoiceInput);
	}
}
