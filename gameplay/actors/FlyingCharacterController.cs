using Godot;

namespace Gameplay.Actors
{
    [GlobalClass]
    public partial class FlyingCharacterController : Node
    {

        [Export] float defaultSpeed = 2.5f;
        [Export] bool faceMovement = false;

        [Export] float movementDamping = 0.1f;

        [ExportGroup("References")]
        [Export] CharacterBody3D body;

        public Vector3 MovInput { get; private set; }

        float _speed;

        bool isDashing;

        public override void _Ready()
        {
            base._Ready();

            _speed = defaultSpeed;
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            if (!IsMultiplayerAuthority()) return;

            if (isDashing)
            {
                PerformDash();
                return;
            }

            if (faceMovement)
            {
                if (MovInput != Vector3.Zero)
                    body.LookAt(body.GlobalPosition + MovInput, Vector3.Up);
            }

            body.Velocity = body.Velocity.MoveToward(MovInput * defaultSpeed, movementDamping);

            body.MoveAndSlide();

            MovInput = Vector3.Zero;
        }

        public void SetAbsoluteMovementInput(Vector3 inputDir)
        {
            MovInput = inputDir;
        }

        public void StartDash(float dashSpeed, float dashDuration)
        {
            isDashing = true;
            _speed = dashSpeed;

            GetTree().CreateTimer(dashDuration).Timeout += StopDash;
        }

        private void PerformDash()
        {
            var dir = -body.Transform.Basis.Z;
            body.Velocity = dir * _speed;
            body.MoveAndSlide();
        }

        private void StopDash()
        {
            isDashing = false;
            _speed = defaultSpeed;
        }
    }
}