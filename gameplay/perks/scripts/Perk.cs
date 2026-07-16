using Godot;
using System;

/// <summary>
/// Represents a perk.
/// 
/// Contains generic description about the perk.
/// </summary>
[GlobalClass]
public abstract partial class Perk : Resource
{
    [Export] public Texture2D Image { get; private set; }
    [Export] public string Name { get; private set; }
    [Export] public string Description { get; private set; }
}
