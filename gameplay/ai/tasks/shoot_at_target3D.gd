#*
#* face_target3D.gd
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
## Flips the agent to face the target, returning [code]SUCCESS[/code]. [br]
## Returns [code]FAILURE[/code] if [member target_var] is not a valid [Node2D] instance.

## Blackboard variable that stores our target (expecting Node2D).
@export var look_at_aiming_controller_path: NodePath

var look_at_aiming_controller: LookAtAimingController

func _setup() -> void:
	look_at_aiming_controller = agent.get_node(look_at_aiming_controller_path)

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "Shoot"

# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	look_at_aiming_controller.Shoot()
	
	return SUCCESS
