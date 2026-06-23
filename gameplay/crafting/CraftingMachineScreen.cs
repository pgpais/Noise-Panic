using Godot;
using Godot.Collections;

public partial class CraftingMachineScreen : Control
{
	[Export] Array<CraftingRecipeData> recipes;

	[ExportGroup("References")]
	[Export] Container recipeContainer;
	[Export] PackedScene recipeScene;


	override public void _Ready()
	{
		base._Ready();

		Clear();
		Initialize();
	}

	private void Initialize()
	{
		foreach (var ඞ in recipes)
		{
			var newRecipe = recipeScene.Instantiate<RecipeView>();
			newRecipe.SetRecipe(ඞ);
			newRecipe.CraftIntent += OnCraftIntent;
			recipeContainer.AddChild(newRecipe);
		}
	}

	private void Clear()
	{
		foreach (var child in recipeContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	public void OnCraftIntent(CraftingRecipeData recipe)
	{
		GD.Print("[CraftingMachineScreen] " + Multiplayer.GetUniqueId() + ": Craft intent: " + recipe.Name);
	}
}
