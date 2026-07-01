using Godot;
using Godot.Collections;

namespace Gameplay.AI;

[GlobalClass]
public partial class AiState : Node3D
{
	[Export] Node conditionsParent;
	[Export] Node actionsParent;

	Array<AiCondition> _conditions = new();
	Array<AiAction> _actions = new();

	public override void _Ready()
	{
		base._Ready();

		foreach (var child in conditionsParent.GetChildren())
		{
			if (child is AiCondition condition)
			{
				_conditions.Add(condition);
			}
		}

		foreach (var child in actionsParent.GetChildren())
		{
			if (child is AiAction action)
			{
				_actions.Add(action);
			}
		}
	}

	public bool CheckConditions()
	{
		foreach (var condition in _conditions)
		{
			if (!condition.Evaluate())
			{
				return false;
			}
		}
		return true;
	}

	public void ExecuteActions()
	{
		foreach (var action in _actions)
		{
			action.Execute();
		}
	}
}