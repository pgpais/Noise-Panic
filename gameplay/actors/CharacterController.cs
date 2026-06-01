using Godot;

[GlobalClass]
public partial class CharacterController : Node
{
	[Export] AnimationPlayer animationPlayer;
	[Export] Node3D cameraAnchor;
	[Export] public float Speed { get; private set; }

	public bool MovementEnabled { get; set; } = true;
	public Vector2 MovInput { get; set; } = Vector2.Zero;
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

	public void RotateCamera(float angleRads)
	{
		GD.Print("Rotating by " + angleRads);
		float targetRotation = cameraAnchor.Rotation.Y + angleRads;
		Tween t = GetTree().CreateTween();
		t.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Linear);
		t.TweenMethod(Callable.From((float x) => SetCameraYRotation(x)), cameraAnchor.Rotation.Y, targetRotation, 0.1f);
	}

	private void SetCameraYRotation(float angleRads)
	{
		var rotation = _body.Rotation;
		rotation.Y = angleRads;
		cameraAnchor.Rotation = rotation;
	}

	private void HandleMovement(double delta)
	{
		Vector3 velocity = _body.Velocity;

		// Add the gravity.
		if (!_body.IsOnFloor())
		{
			velocity += _body.GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = MovInput;
		Vector3 direction = (_body.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			if (animationPlayer != null)
			{

				animationPlayer.Play("Running");
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(_body.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(_body.Velocity.Z, 0, Speed);
			if (animationPlayer != null)
			{
				animationPlayer.Play("Iddle"); //TODO: fix typo
			}
		}

		_body.Velocity = velocity;
		_body.MoveAndSlide();

		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.SyncMovement, _body.GlobalPosition);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncMovement(Vector3 position)
	{
		_body.GlobalPosition = position;
	}
}
