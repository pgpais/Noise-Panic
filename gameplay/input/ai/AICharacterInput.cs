using Godot;

[GlobalClass]
public partial class AICharacterInput : Node
{
    [Export] Node3D target;

    [Export] CharacterController controller;

    Node3D _owner;

    public override void _Ready()
    {
        base._Ready();

        _owner = GetOwner<Node3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var dir = (target.GlobalPosition - _owner.GlobalPosition).Normalized();

        controller.MovInput = new Vector2(dir.X, dir.Z);
    }

    public void SetTarget(Node3D target)
    {
        this.target = target;
    }
}
