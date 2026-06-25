using Gameplay.Crafting;
using Gameplay.Crafting.UI;
using Godot;
using Godot.Collections;

public partial class CraftingMachineScreen : Control
{
	[Export] CraftingMachine _craftingMachine;

	[ExportGroup("References")]
	[Export] CraftingMachineProgressUI _progressUi;

	public void OnCraftIntent(CraftingRecipeData recipe)
	{
		GD.Print("[CraftingMachineScreen] " + Multiplayer.GetUniqueId() + ": Craft intent: " + recipe.Name);
		_craftingMachine.Debugstartcraft();
	}
}
