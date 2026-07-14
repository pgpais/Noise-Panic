using System;
using Godot;

[GlobalClass]
public partial class CharacterController : Node
{
	//@entega_: 1. Cozer o bacalhau e fritar as batatasColoque o bacalhau num tacho, cubra com água e leve a ferver.Coza por 5 minutos, retire o peixe e reserve o leite ou a água da cozedura para o molho.Limpe o bacalhau de peles e espinhas e desfie-o em lascas.Frite as batatas em cubos numa fritadeira convencional ou na air fryer até dourarem levemente (sem ficarem demasiado duras). Reserve.2. Fazer o refogadoNum tacho largo, aqueça um fio generoso de azeite.Adicione as cebolas, os alhos e a folha de louro.
	[Export] AnimationTree animationTree;
	[Export] Node3D cameraAnchor;

	[Export] public float RunningSpeed { get; private set; } = 10f;
	[Export] public float WalkingSpeed { get; private set; } = 5f;
	[Export] public float JumpVelocity { get; private set; } = 10f;
	[Export] public bool TurnToFaceMovement { get; set; } = true;

	[ExportGroup("Dashing Parameters")]
	[Export] public float DashSpeed { get; private set; } = 30f;
	[Export] public float DashDuration { get; private set; } = 0.2f;

	[ExportGroup("Navigation")]
	[Export] NavigationAgent3D navigationAgent;

	public bool MovementEnabled { get; set; } = true;
	public Vector2 MovInput { get; set; } = Vector2.Zero;
	public bool WantsToJump { get; set; }
	public bool WantsToRun { get; set; }
	public bool WantsToDash { get; set; }

	float targetYRotation;
	float currentSpeed;
	bool isDashing = false;

	CharacterBody3D _body;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_body = GetOwner<CharacterBody3D>(); // necessary because of Godot's bug

		if (navigationAgent != null)
		{
			navigationAgent.VelocityComputed += OnNavigationVelocityComputed;
		}
	}

	private void OnNavigationVelocityComputed(Vector3 safeVelocity)
	{
		_body.Velocity = safeVelocity;
		_body.MoveAndSlide();
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

	public void SetRelativeMovementInput(Vector2 inputDir)
	{
		// Using camera forward and right as local axis
		Vector3 forward = _body.GlobalTransform.Basis.Z * inputDir.Y;
		Vector3 right = _body.GlobalTransform.Basis.X * inputDir.X;
		Vector2 direction = new Vector2(forward.X, forward.Z) + new Vector2(right.X, right.Z);
		MovInput = direction.Normalized();
	}

	public void SetAbsoluteMovementInput(Vector2 inputDir)
	{
		MovInput = inputDir;
	}

	public void LookAt(Vector3 lookAtPos)
	{
		if (!IsMultiplayerAuthority()) return;

		_body.LookAt(lookAtPos, Vector3.Up);
		Rpc(MethodName.SyncLookAt, lookAtPos);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncLookAt(Vector3 lookAtPos) => _body.LookAt(lookAtPos, Vector3.Up);

	private void HandleMovement(double delta)
	{
		Vector3 velocity = _body.Velocity;
		var speed = WantsToRun ? RunningSpeed : WalkingSpeed;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = MovInput;
		Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y);

		if (TurnToFaceMovement && direction != Vector3.Zero)
		{
			Vector3 lookAtPos = _body.GlobalPosition + direction;
			LookAt(lookAtPos);
		}

		// Add the gravity.
		if (!_body.IsOnFloor())
		{
			velocity += _body.GetGravity() * (float)delta;
		}
		else
		{
			if (WantsToDash && !isDashing)
			{
				StartDash();
			}

			if (WantsToJump && !isDashing)
			{
				velocity.Y = JumpVelocity;
			}
		}

		if (isDashing)
		{
			PerformDash(direction);
		}
		else
		{
			PerformMovement(velocity, speed, direction);
		}

		WantsToJump = false;
		WantsToDash = false;
		WantsToRun = false;
	}

	private void PerformMovement(Vector3 velocity, float speed, Vector3 direction)
	{
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = 0;
			velocity.Z = 0;
		}

		if (navigationAgent != null && navigationAgent.AvoidanceEnabled && !navigationAgent.IsNavigationFinished())
		{
			navigationAgent.Velocity = velocity;
		}
		else
		{
			_body.Velocity = velocity;
			_body.MoveAndSlide();
		}

		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.SyncMovement, _body.GlobalPosition, velocity);
			HandleAnimations(velocity);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncMovement(Vector3 position, Vector3 velocity)
	{
		_body.GlobalPosition = position;
		_body.Velocity = velocity;

		if (TurnToFaceMovement && velocity != Vector3.Zero)
		{
			Vector3 lookAtPos = _body.GlobalPosition + velocity.Normalized();
			LookAt(lookAtPos);
		}

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
			animationTree.Set("parameters/MovementBlendSpace/blend_position", velocity2D / WalkingSpeed);
		}
	}

	public void StartDash()
	{
		isDashing = true;
		currentSpeed = DashSpeed;

		GetTree().CreateTimer(DashDuration).Timeout += StopDash;
	}

	private void PerformDash(Vector3 direction)
	{
		var velocity = direction * DashSpeed;
		_body.Velocity = velocity;
		_body.MoveAndSlide();

		Rpc(MethodName.SyncPerformDash, _body.GlobalPosition, velocity);
	}

	[Rpc()]
	private void SyncPerformDash(Vector3 position, Vector3 velocity)
	{
		_body.GlobalPosition = position;
		_body.Velocity = velocity;
		_body.MoveAndSlide();
	}

	private void StopDash()
	{
		isDashing = false;
		currentSpeed = WalkingSpeed;
	}
}
