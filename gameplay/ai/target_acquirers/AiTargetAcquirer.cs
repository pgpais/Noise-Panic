using Godot;

namespace Gameplay.AI;

[GlobalClass]
public abstract partial class AiTargetAcquirer : Node3D
{
    public abstract Node3D GetTarget();
}