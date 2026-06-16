using Godot;

[GlobalClass]
public partial class CharacterController : Node
{
	[Export] AnimationTree animationTree;
	[Export] Node3D cameraAnchor;
	[Export] public float Speed { get; private set; }
	[Export] public float JumpVelocity { get; private set; } = 10f;
	[Export] public bool TurnToFaceMovement { get; set; } = true;

	public bool MovementEnabled { get; set; } = true;
	public Vector2 MovInput { get; set; } = Vector2.Zero;
	public bool WantsToJump { get; set; }

	float targetYRotation;

	CharacterBody3D _body;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_body = GetOwner<CharacterBody3D>(); // necessary because of Godot's bug
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (MovementEnabled)
		{
			HandleMovement(delta);
		}

		MovInput = Vector2.Zero;
	}

	private void HandleMovement(double delta)
	{
		Vector3 velocity = _body.Velocity;


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = MovInput;

		// Using camera forward and right as local axis
		Vector3 forward = _body.GlobalTransform.Basis.Z * inputDir.Y;
		Vector3 right = _body.GlobalTransform.Basis.X * inputDir.X;
		Vector3 direction = forward + right;
		direction.Y = 0;
		direction = direction.Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(_body.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(_body.Velocity.Z, 0, Speed);
		}

		if (TurnToFaceMovement)
		{
			//TODO:
			// Vector3 lookAtPos = _body.GlobalPosition + direction;
			// _body.LookAt(lookAtPos, Vector3.Up);
		}


		// Add the gravity.
		if (!_body.IsOnFloor())
		{
			velocity += _body.GetGravity() * (float)delta;
		}
		if (WantsToJump)
		{
			velocity.Y = JumpVelocity;
		}

		_body.Velocity = velocity;
		_body.MoveAndSlide();

		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.SyncMovement, _body.GlobalPosition, velocity);
			HandleAnimations(velocity);
		}

		WantsToJump = false;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncMovement(Vector3 position, Vector3 velocity)
	{
		_body.GlobalPosition = position;
		_body.Velocity = velocity;
		HandleAnimations(velocity);
	}

	private void HandleAnimations(Vector3 velocity)
	{
		if (animationTree != null)
		{
			bool isMoving = velocity.X != 0 || velocity.Z != 0;
			var velocity2D = new Vector2(velocity.X, velocity.Z);
			animationTree.Set("parameters/conditions/has_jumped", velocity.Y > 0);
			animationTree.Set("parameters/conditions/is_falling", velocity.Y < 0);
			animationTree.Set("parameters/conditions/is_on_floor", _body.IsOnFloor());
			animationTree.Set("parameters/conditions/is_moving", isMoving);
			animationTree.Set("parameters/conditions/is_not_moving", !isMoving);
			animationTree.Set("parameters/MovementBlendSpace/blend_position", velocity2D / Speed);
		}
	}
}
