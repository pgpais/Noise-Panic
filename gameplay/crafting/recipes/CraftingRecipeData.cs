using Godot;
using System;
using WeWereDumb.Gameplay.Resources;

[GlobalClass]
public partial class CraftingRecipeData : Resource
{
	[Export] public string Name { get; private set; }
	[Export] public string Description { get; private set; }
	[Export] public float RecipeTime { get; private set; } = 1f;
	[Export] public GameResourceData Result { get; private set; }
}
