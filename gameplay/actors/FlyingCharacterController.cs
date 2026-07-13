using Godot;
using Godot.Collections;

namespace Gameplay.Actors
{
    [GlobalClass]
    public partial class FlyingCharacterController : Node
    {

        [Export] float defaultSpeed = 2.5f;
        [Export] bool faceMovement = false;

        [Export] float flyHeight = 10f;
        [Export] float heightFixVelocity = 2.5f;

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

            var velocity = body.Velocity;

            velocity = velocity.MoveToward(MovInput * defaultSpeed, movementDamping);
            if (CheckFloor(out var result))
            {
                Vector3 collisionPoint = result["position"].As<Vector3>();
                var targetYPosition = collisionPoint.Y + flyHeight;

                var yDiff = targetYPosition - body.GlobalPosition.Y;
                if (yDiff > 0)
                {
                    velocity.Y += heightFixVelocity;
                }
                else if (yDiff < 0)
                {
                    velocity.Y -= heightFixVelocity;
                }
            }

            body.Velocity = velocity;

            body.MoveAndSlide();

            if (faceMovement)
            {
                if (MovInput != Vector3.Zero)
                    LookAt(body.GlobalPosition + body.Velocity);
            }

            MovInput = Vector3.Zero;

            Rpc(MethodName.SyncMovement, body.GlobalPosition, velocity);
        }

        public void LookAt(Vector3 lookAtPos)
        {
            if (!IsMultiplayerAuthority()) return;

            body.LookAt(lookAtPos, Vector3.Up);
            Rpc(MethodName.SyncLookAt, lookAtPos);
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

        private bool CheckFloor(out Dictionary result)
        {
            var spaceState = body.GetWorld3D().DirectSpaceState;

            // use global coordinates, not local to node
            var query = PhysicsRayQueryParameters3D.Create(body.GlobalPosition, body.GlobalPosition - new Vector3(0, flyHeight, 0));
            result = spaceState.IntersectRay(query);

            return result.Count > 0;
        }

        private void PerformDash()
        {
            var dir = -body.Transform.Basis.Z;
            var velocity = dir * _speed;
            body.Velocity = velocity;
            body.MoveAndSlide();

            Rpc(MethodName.SyncPerformDash, body.GlobalPosition, velocity);
        }

        [Rpc()]
        private void SyncPerformDash(Vector3 position, Vector3 velocity)
        {
            body.GlobalPosition = position;
            body.Velocity = velocity;
            body.MoveAndSlide();
        }

        private void StopDash()
        {
            isDashing = false;
            _speed = defaultSpeed;
        }

        [Rpc()]
        private void SyncMovement(Vector3 position, Vector3 velocity)
        {
            body.GlobalPosition = position;
            body.Velocity = velocity;

            if (faceMovement && velocity != Vector3.Zero)
            {
                Vector3 lookAtPos = body.GlobalPosition + velocity.Normalized();
                body.LookAt(lookAtPos, Vector3.Up);
            }
        }

        [Rpc()]
        private void SyncLookAt(Vector3 lookAtPos) => body.LookAt(lookAtPos, Vector3.Up);
    }
}