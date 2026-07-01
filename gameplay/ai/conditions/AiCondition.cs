using Godot;

namespace Gameplay.AI;

[GlobalClass]
public abstract partial class AiCondition : Node3D
{
    [Signal] public delegate void ConditionMetEventHandler();

    [Export] private bool evaluateEveryFrame = true;

    public override void _Ready()
    {
        base._Ready();

        if (!evaluateEveryFrame)
        {
            ProcessMode = ProcessModeEnum.Disabled;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._Process(delta);

        Evaluate();
    }

    public abstract bool Evaluate();
}