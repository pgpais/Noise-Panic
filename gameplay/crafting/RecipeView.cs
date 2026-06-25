using Godot;
using System;

public partial class RecipeView : PanelContainer
{
	[Export] Label recipeName;

	CraftingRecipeData recipe;

	public void SetRecipe(CraftingRecipeData recipe)
	{
		this.recipe = recipe;
		recipeName.Text = recipe.Name;
	}
}
