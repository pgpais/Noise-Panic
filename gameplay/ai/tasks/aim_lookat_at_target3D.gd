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
@export var target_var: StringName = &"target"
@export var look_at_aiming_controller_path: NodePath

var look_at_aiming_controller: LookAtAimingController

func _setup() -> void:
	look_at_aiming_controller = agent.get_node(look_at_aiming_controller_path)
	

# Display a customized name (requires @tool).
func _generate_name() -> String:
	return "LookAt Target " + LimboUtility.decorate_var(target_var)


# Called each time this task is ticked (aka executed).
func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var)
	if not is_instance_valid(target):
		return FAILURE
	
	#agent.face_dir(dir)
	look_at_aiming_controller.AimAtPos(target.global_position)
	
	
	return SUCCESS
