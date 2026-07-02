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

var character_controller: CharacterController

func _setup() -> void:
	character_controller = agent.get_node(character_controller_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "MoveTowards " + LimboUtility.decorate_var(target_var)

# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var)
	if not is_instance_valid(target):
		return FAILURE

	var dir = agent.global_position.direction_to(target.global_position)
	character_controller.SetAbsoluteMovementInput(Vector2(dir.x, dir.z).normalized())
	
	return SUCCESS
