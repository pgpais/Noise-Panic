using Godot;

namespace Gameplay.Crafting.UI;

[GlobalClass]
public partial class CraftingMachineProgressUI: ProgressBar
{
    [Export] private CraftingMachine _craftingMachine;

    public override void _Ready()
    {
        base._Ready();
        if(_craftingMachine != null)
        {
            SetMachine(_craftingMachine);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        Value = _craftingMachine.ElapsedTime * 100;
    }

    public void SetMachine(CraftingMachine craftingMachine)
    {
        _craftingMachine = craftingMachine;
        
        craftingMachine.CraftingStarted += OnCraftingStarted;
    }

    private void OnCraftingStarted(CraftingRecipeData recipe)
    {
        MaxValue = recipe.RecipeTime * 100;
        Value = _craftingMachine.ElapsedTime * 100;
    }
}