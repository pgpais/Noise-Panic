#*
#* move_forward.gd
#* =============================================================================
#* Copyright (c) 2023-present Serhii Snitsaruk and the LimboAI contributors.
#*
#* Use of this source code is governed by an MIT-style
#* license that can be found in the LICENSE file or at
#* https://opensource.org/licenses/MIT.
#* =============================================================================
#*
@tool
extends BTAction
## Applies velocity in the direction the agent is facing on each tick
## until the [member duration] is exceeded. [br]
## Returns [code]SUCCESS[/code] if the elapsed time exceeds [member duration]. [br]
## Returns [code]RUNNING[/code] if the elapsed time does not exceed [member duration]. [br]

@export var target_var: StringName = &"target"
@export var character_controller_path: NodePath
@export var nav_agent_path: NodePath

var nav_agent: NavigationAgent3D

func _setup() -> void:
	nav_agent = agent.get_node(nav_agent_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "PathfindTowards " + LimboUtility.decorate_var(target_var)

# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var)
	if not is_instance_valid(target):
		return FAILURE

	# nav_agent.set_target_position(target.global_position)

	nav_agent.target_position = target.global_position
	print(agent.name, " calculated path to ", target.name)
	return SUCCESS
