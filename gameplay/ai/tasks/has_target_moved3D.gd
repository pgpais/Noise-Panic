#*
#* in_range.gd
#* =============================================================================
#* Copyright (c) 2023-present Serhii Snitsaruk and the LimboAI contributors.
#*
#* Use of this source code is governed by an MIT-style
#* license that can be found in the LICENSE file or at
#* https://opensource.org/licenses/MIT.
#* =============================================================================
#*
@tool
extends BTCondition
## InRange condition checks if the agent is within a range of target,
## defined by [member distance_min] and [member distance_max]. [br]
## Returns [code]SUCCESS[/code] if the agent is within the given range;
## otherwise, returns [code]FAILURE[/code].

## Minimum position change to trigger.
@export var position_change_min: float


## Blackboard variable that holds the target (expecting Node3D).
@export var target_var: StringName = &"target"

## Blackboard variable that holds the target position
@export var target_pos_var: StringName = &"target_pos"

var _position_change_min_squared: float


# Called to generate a display name for the task.
func _generate_name() -> String:
	return "HasTargetMoved from %s at least %dm - %s" % [LimboUtility.decorate_var(target_pos_var), position_change_min,
		LimboUtility.decorate_var(target_var)]


# Called to initialize the task.
func _setup() -> void:
	## Small performance optimization
	_position_change_min_squared = position_change_min * position_change_min


# Called when the task is executed.
func _tick(_delta: float) -> Status:
	var target: Node3D = blackboard.get_var(target_var, null)
	if not is_instance_valid(target):
		return FAILURE
	var old_pos: Vector3 = blackboard.get_var(target_pos_var, Vector3.ZERO, false)

	var dist_sq: float = old_pos.distance_squared_to(target.global_position)
	if dist_sq >= _position_change_min_squared:
		return SUCCESS
	else:
		return FAILURE
