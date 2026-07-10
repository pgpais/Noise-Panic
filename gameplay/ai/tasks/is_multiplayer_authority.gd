#*
#* is_multiplayer_authority.gd
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


# Called to generate a display name for the task.
func _generate_name() -> String:
	return "IsMultiplayerAuthority"


# Called to initialize the task.
func _setup() -> void:
	pass

# Called when the task is executed.
func _tick(_delta: float) -> Status:
	if agent.is_multiplayer_authority():
		return SUCCESS
	else:
		return FAILURE
