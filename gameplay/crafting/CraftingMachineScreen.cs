using Gameplay.Crafting;
using Gameplay.Crafting.UI;
using Godot;

namespace Gameplay.Crafting.UI;

public partial class CraftingMachineScreen : Control
{
	[Export] CraftingMachine _craftingMachine;
	[Export] RecipeSelector _recipeSelector;

	[ExportGroup("References")]
	[Export] CraftingMachineProgressUI _progressUi;
	[Export] RecipeView _recipeView;

	public override void _Ready()
	{
		_progressUi.Hide();
		
		_progressUi.SetMachine(_craftingMachine);

		_recipeView.SetRecipe(_recipeSelector.CurrentRecipe);
		_recipeSelector.RecipeSelectionChanged += _recipeView.SetRecipe;

		_craftingMachine.CraftingStarted += OnCraftingStarted;
		_craftingMachine.CraftingStopped += OnCraftingStopped;
	}

	private void OnCraftingStarted(CraftingRecipeData recipe)
	{
		_progressUi.Show();
	}

	private void OnCraftingStopped()
	{
		_progressUi.Hide();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_craftingMachine.IsCrafting)
		{
			_progressUi.Show();
		}
		else
		{
			_progressUi.Hide();
		}
	}
}