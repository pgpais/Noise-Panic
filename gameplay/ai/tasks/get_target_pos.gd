#*
#* get_target_pos.gd
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
## Stores the first node in the [member group] on the blackboard, returning [code]SUCCESS[/code]. [br]
## Returns [code]FAILURE[/code] if the group contains 0 nodes.

## Blackboard variable in which the task will store the acquired node.
@export var output_var: StringName = &"target_pos"

## Blackboard variable that holds the target (expecting Node3D).
@export var target_var: StringName = &"target"


func _generate_name() -> String:
	return "GetTargetPos ➜ %s" % [
		LimboUtility.decorate_var(output_var)
		]

func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var, null)
	if not is_instance_valid(target):
		return FAILURE
	
	blackboard.set_var(output_var, target.global_position)
	return SUCCESS
