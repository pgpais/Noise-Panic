using Godot;

namespace Gameplay.AI.Actions
{
    [GlobalClass]
    public partial class MoveToTargetAiAction : AiAction
    {
        [Export] CharacterController characterController;
        [Export] AiTargetAcquirer targetAcquirer;

        Node3D owner3D;

        public override void _Ready()
        {
            base._Ready();

            owner3D = GetOwner<Node3D>();
        }

        public override void Execute()
        {
            var target = targetAcquirer.GetTarget();
            var direction = owner3D.GlobalPosition.DirectionTo(target.GlobalPosition);
            characterController.SetAbsoluteMovementInput(new Vector2(direction.X, direction.Z));
        }
    }
}