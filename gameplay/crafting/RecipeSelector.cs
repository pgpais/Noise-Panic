using Godot;
using Godot.Collections;

namespace Gameplay.Crafting;

[GlobalClass]
public partial class RecipeSelector : Node
{
	[Signal] public delegate void RecipeSelectedEventHandler(CraftingRecipeData recipe);
	[Signal] public delegate void RecipeSelectionChangedEventHandler(CraftingRecipeData recipe);
	
	public CraftingRecipeData CurrentRecipe => _craftingRecipes[_currentRecipeIdx];
	
	[Export] Array<CraftingRecipeData> _craftingRecipes;
	
	[Export] CraftingMachine _craftingMachine;

	private int _currentRecipeIdx;

	public void NextRecipe()
	{
		GD.Print($"[RecipeSelector] Next recipe {_currentRecipeIdx}");
		_currentRecipeIdx = (_currentRecipeIdx + 1) % _craftingRecipes.Count;
		EmitSignal(SignalName.RecipeSelectionChanged, CurrentRecipe);
	}

	public void PreviousRecipe()
	{
		GD.Print($"[RecipeSelector] Previous recipe {_currentRecipeIdx}");
		_currentRecipeIdx = (_currentRecipeIdx - 1) % _craftingRecipes.Count;
		EmitSignal(SignalName.RecipeSelectionChanged, CurrentRecipe);
	}

	public void SubmitRecipe()
	{
		GD.Print($"[RecipeSelector] Submit recipe {_currentRecipeIdx}");
		EmitSignal(SignalName.RecipeSelected, CurrentRecipe);
		
		_craftingMachine.StartCrafting(CurrentRecipe);
	}
}