using Godot;
using System;

public partial class ShipController : Node
{
	[Export] RigidBody3D _shipBody;
	[Export] bool enabled = true;
	[Export] float thrust = 10.0f;
	[Export] float turnSpeed = 2.0f;

	public Vector2 InputDirection { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	override public void _PhysicsProcess(double delta)
	{
		if (!enabled) return;

		Vector3 forward = _shipBody.Transform.Basis.Z;

		if (InputDirection != Vector2.Zero)
		{
			Vector3 moveDirection = forward * InputDirection.Y;
			_shipBody.ApplyCentralForce(moveDirection * thrust);

			float turnDirection = -InputDirection.X;
			_shipBody.ApplyTorque(new Vector3(0, turnDirection * turnSpeed, 0));
		}

		if (IsMultiplayerAuthority())
		{
			Rpc(MethodName.SyncMovement, _shipBody.Transform.Origin, new Quaternion(_shipBody.Transform.Basis), _shipBody.LinearVelocity, _shipBody.AngularVelocity);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void SyncMovement(Vector3 position, Quaternion rotation, Vector3 linearVelocity, Vector3 angularVelocity)
	{
		//TODO: we probably want interpolation or physics prediction

		// Transform3D transform = _shipBody.Transform;
		// transform.Basis = Basis.Identity;
		// transform.Origin = position;
		// transform.Basis = new Basis(rotation);

		_shipBody.LinearVelocity = linearVelocity;
		_shipBody.AngularVelocity = angularVelocity;

		// _shipBody.Transform = transform;
	}

	public void Enable()
	{
		enabled = true;
	}

	public void Disable()
	{
		enabled = false;
	}

	public void SetBody(RigidBody3D body)
	{
		_shipBody = body;
	}
}
