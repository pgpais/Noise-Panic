using Godot;

namespace Gameplay.AI.Conditions
{
    [GlobalClass]
    public partial class HasTargetAiCondition : AiCondition
    {
        [Export] public AiTargetAcquirer targetAcquirer;

        public override bool Evaluate()
        {
            return targetAcquirer.HasTarget();
        }
    }
}