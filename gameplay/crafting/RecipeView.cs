using Godot;
using System;

public partial class RecipeView : PanelContainer
{
	[Signal] public delegate void CraftIntentEventHandler(CraftingRecipeData recipe);

	[Export] Label recipeName;
	[Export] Button craftButton;

	CraftingRecipeData recipe;

	public override void _Ready()
	{
		base._Ready();

		craftButton.Pressed += () => EmitSignal(SignalName.CraftIntent, recipe);
	}

	public void SetRecipe(CraftingRecipeData recipe)
	{
		this.recipe = recipe;
		recipeName.Text = recipe.Name;
	}
}
