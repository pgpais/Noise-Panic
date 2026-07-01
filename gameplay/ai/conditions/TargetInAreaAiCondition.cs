using Godot;
using Godot.Collections;

namespace Gameplay.AI.Conditions
{
    [GlobalClass]
    public partial class TargetInAreaAiCondition : AiCondition
    {
        [Export] Area3D area;
        [Export] AiTargetAcquirer targetAcquirer;

        public override bool Evaluate()
        {
            Node3D target = targetAcquirer.GetTarget();
            Array<Node3D> overlappingBodies = area.GetOverlappingBodies();
            return overlappingBodies.Contains(target);
        }
    }
}