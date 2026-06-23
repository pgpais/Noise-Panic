using Godot;
using System;

[GlobalClass]
public partial class CraftingRecipeData : Resource
{
	[Export] public string Name { get; private set; }
	[Export] public string Description { get; private set; }
}
