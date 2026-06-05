using Godot;

[GlobalClass]
public partial class CharacterController : Node
{
	[Export] AnimationTree animationTree;
	[Export] Node3D cameraAnchor;
	[Export] public float Speed { get; private set; }

	public bool MovementEnabled { get; set; } = true;
	public Vector2 MovInput { get; set; } = Vector2.Zero;
	float targetYRotation;

	Camera3D _camera;
	CharacterBody3D _body;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_body = GetOwner<CharacterBody3D>(); // necessary because of Godot's bug
		_camera = GetViewport().GetCamera3D();
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
		Vector3 direction = (_camera.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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

		HandleAnimations(velocity);

		// Add the gravity.
		if (!_body.IsOnFloor())
		{
			velocity += _body.GetGravity() * (float)delta;
		}

		_body.Velocity = velocity;
		_body.MoveAndSlide();

		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.SyncMovement, _body.GlobalPosition, velocity);
		}
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
			var velocity2D = new Vector2(velocity.X, velocity.Z);
			animationTree.Set("parameters/ArmsRunning/blend_position", velocity2D / Speed);
			animationTree.Set("parameters/BodyRunning/blend_position", velocity2D / Speed);
		}
	}
}
