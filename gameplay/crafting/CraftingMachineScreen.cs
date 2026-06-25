using Gameplay.Crafting;
using Gameplay.Crafting.UI;
using Godot;
using Godot.Collections;

public partial class CraftingMachineScreen : Control
{
	[Export] CraftingMachine _craftingMachine;

	[ExportGroup("References")]
	[Export] CraftingMachineProgressUI _progressUi;

	public override void _Ready()
	{
		_progressUi.SetMachine(_craftingMachine);
	}
}
