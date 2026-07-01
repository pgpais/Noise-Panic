using Godot;

namespace Gameplay.AI;

[GlobalClass]
public abstract partial class AiAction : Node3D
{
    public abstract void Execute();
}