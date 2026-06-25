using System;
using Godot;
using WeWereDumb.Gameplay.Interaction;

namespace Gameplay.Crafting;

public partial class CraftingMachine : Node
{
	[Signal] public delegate void CraftingStartedEventHandler(CraftingRecipeData recipe);
	[Signal] public delegate void CraftingStoppedEventHandler();
	
	public CraftingRecipeData CurrentRecipe { get; private set; }
	public float ElapsedTime => _elapsedTime;

	[Export] Node3D _resultSpawnPosition;
	
	[ExportGroup("Debug")]
	[Export] CraftingRecipeData _debugRecipe;
	
	float _elapsedTime = 0f;
	bool _isCrafting = false;
	
	public void StartCrafting(CraftingRecipeData recipe)
	{
		CurrentRecipe = recipe;
		_elapsedTime = 0f;
		_isCrafting = true;
		
		EmitSignal(SignalName.CraftingStarted, recipe);
	}

	public void CancelCrafting()
	{
		EmitSignal(SignalName.CraftingStopped);
		_isCrafting = false;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_isCrafting)
		{
			CraftingTick(delta);
		}
	}

	public void Debugstartcraft()
	{
		StartCrafting(_debugRecipe);
	}

	private void RefundCosts()
	{
		throw new NotImplementedException();
	}
	
	private void CraftingTick(double delta)
	{
		_elapsedTime += (float)delta;
		if (IsCraftingDone())
		{
			FinishCraft();
		}
	}

	private void FinishCraft()
	{
		_isCrafting = false;
		ReceiveCraftRewards();
		
		CurrentRecipe = null;
		
		EmitSignal(SignalName.CraftingStopped);
	}

	private void ReceiveCraftRewards()
	{
		GD.Print($"[CraftingMachine] {Multiplayer.GetUniqueId()}: {CurrentRecipe.Name} completed");
		var resource = CurrentRecipe.Result.SpawnResource();
		resource.GlobalPosition = _resultSpawnPosition.GlobalPosition;
		GetTree().CurrentScene.AddChild(resource, true);
	}
	
	private bool IsCraftingDone() => _elapsedTime >= CurrentRecipe.RecipeTime;
}