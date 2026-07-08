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

@export var character_controller_path: NodePath
@export var nav_agent_path: NodePath

var character_controller: CharacterController
var nav_agent: NavigationAgent3D

func _setup() -> void:
	character_controller = agent.get_node(character_controller_path)
	nav_agent = agent.get_node(nav_agent_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "Pathfind"

# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	if nav_agent.is_navigation_finished():
		return FAILURE

	nav_agent.get_current_navigation_path()
	

	var next_path_position: Vector3 = nav_agent.get_next_path_position()

	var dir = agent.global_position.direction_to(next_path_position)
	var normalized_dir = Vector2(dir.x, dir.z).normalized()
	character_controller.SetAbsoluteMovementInput(normalized_dir)
	return SUCCESS
