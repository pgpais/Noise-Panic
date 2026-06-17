using Godot;
using System;
using WeWereDumb.Gameplay.Resources;
using WeWereDumb.Gameplay.Storage;

public partial class GameRun : Node3D
{
	[Signal] public delegate void ObjectiveProgressChangedEventHandler(float progress);

	[Export] ResourceStorage storage;

	[ExportGroup("Run Parameters")]
	[Export] Timer countdownTimer;
	[Export] int itemExtractsRequired;
	[Export] GameResourceData itemRequired;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (IsMultiplayerAuthority())
		{
			StartCountdown();
		}

		countdownTimer.Timeout += OnCountdownComplete;
		EventBus.Instance.ItemExtracted += OnItemExtracted;
	}

	private void OnItemExtracted(string itemName)
	{
		GD.Print($"[GameRun] {Multiplayer.GetUniqueId()}: Item extracted: {itemName}");

		EmitSignal(SignalName.ObjectiveProgressChanged, storage.GetAmount(itemRequired) / (float)itemExtractsRequired);

		if (!IsMultiplayerAuthority()) return;

		if (storage.GetAmount(itemRequired) >= itemExtractsRequired)
		{
			Rpc(MethodName.EndRun, true);
		}
	}

	[Rpc(CallLocal = true)]
	public void StartCountdown()
	{
		countdownTimer.Start();
	}

	public void OnCountdownComplete()
	{
		GD.Print($"[GameRun] {Multiplayer.GetUniqueId()}: Countdown complete");
		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.EndRun, false);
		}
	}

	[Rpc(CallLocal = true)]
	public void EndRun(bool isVictory)
	{
		GD.Print($"[GameRun] {Multiplayer.GetUniqueId()}: End run, isVictory: {isVictory}");
		if (isVictory)
		{
			OnVictory();
		}
		else
		{
			OnDefeat();
		}
	}

	private void OnVictory()
	{
		GD.Print($"[GameRun] {Multiplayer.GetUniqueId()}: Victory!");
	}

	private void OnDefeat()
	{
		GD.Print($"[GameRun] {Multiplayer.GetUniqueId()}: Defeat!");
	}
}
